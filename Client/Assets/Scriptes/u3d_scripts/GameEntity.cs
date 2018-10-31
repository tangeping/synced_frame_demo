using CBFrame.Core;
using CBFrame.Sys;
using KBEngine;
using SyncFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class GameEntity : MonoBehaviour {

    public bool isPlayer = false;
    public bool isAvatar = false;
    public bool entityEnabled = true;
    public  KBEngine.Entity entity;

    //-----------test Float--------
    public double d_recieve = 0;
    public FP fp_recieve;

    //----------------------------
    private UInt32 readFramePos = 0;

    public static FP playTime = 1 / 30.0f; // 33 s
    private FP FrameDuration = 0f;
    private TSVector destPosition = TSVector.zero;
    private FP  destDuration = 0.0f;
    private int thresholdFrame = 1;
    public FP emptyFramesTime = 0f;
    public int thresholdMaxFrame = 30;

    private FP Speed = 10.0f;
    //-------------------------
    private TSVector position;
    //----------------------------
    public class FrameData
    {
        public FP duration;
        public List<ENTITY_DATA> operation = new List<ENTITY_DATA>();
    }

    public Queue<KeyValuePair<UInt32, FrameData>> framePool = new Queue<KeyValuePair<UInt32, FrameData>>();

    public FrameData lastFrameData = null;
    private UInt32 curreFrameId = 0;

    public FP DestDuration
    {
        get
        {
            return destDuration;
        }

        set
        {
            destDuration = value;
        }
    }

    public int ThresholdFrame
    {
        get
        {
            return thresholdFrame;
        }

        set
        {
            if (value < 1)
            {
                thresholdFrame = 1;
            }
            else if (value > thresholdMaxFrame)
            {
                thresholdFrame = thresholdMaxFrame;
            }
            else
            {
                thresholdFrame = value;
            }
        }
    }

    public TSVector Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
            transform.position = new Vector3(position.x.AsFloat(), position.y.AsFloat(), position.z.AsFloat());
        }
    }

    //----------------------------
    public void entityEnable()
    {
        entityEnabled = true;
    }

    public void entityDisable()
    {
        entityEnabled = false;
    }

    // Use this for initialization
    void Start ()
    {
        KBEngine.Event.registerOut("onRecieveFrame", this, "onRecieveFrame");

 //       CBGlobalEventDispatcher.Instance.AddEventListener<FrameData>((int)EVENT_ID.EVENT_FRAME_TICK, onReadFrame);
 //       Debug.LogError("GameEntity.start." + transform.name);
    }



    void OnGUI()
    {
        if (Camera.main == null || transform.name == "")
            return;

        //根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标     
        Vector2 uiposition = Camera.main.WorldToScreenPoint(transform.position);

        //得到真实NPC头顶的2D坐标
        uiposition = new Vector2(uiposition.x, Screen.height - uiposition.y);

        //计算NPC名称的宽高
        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(transform.name));


        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;             //设置背景填充  
        fontStyle.normal.textColor = Color.yellow;      //设置字体颜色  
        fontStyle.fontSize = (int)(15.0 * gameObject.transform.localScale.x);
        fontStyle.alignment = TextAnchor.MiddleCenter;

        //绘制NPC名称
        GUI.Label(new Rect(uiposition.x - (nameSize.x / 4), uiposition.y - nameSize.y*4, nameSize.x, nameSize.y), transform.name, fontStyle);

        

        //         GUI.Label(new Rect(uiposition.x - (nameSize.x / 4), uiposition.y - nameSize.y * 4 -100, nameSize.x+100, nameSize.y+1000), "d:" + d_recieve.ToString("f15"));
        //         GUI.Label(new Rect(uiposition.x - (nameSize.x / 4), uiposition.y - nameSize.y * 4 - 80, nameSize.x + 100, nameSize.y + 1000), "fp:" + fp_recieve.AsDouble().ToString("f15"));

    }

    public void onRecieveFrame(FRAME_DATA frameMsg)
    {
        curreFrameId = frameMsg.frameid;
//        Debug.Log("id:"+ entity.id +",frameid:" + frameMsg.frameid + ",----------onRecieveFrame tick : " + DateTime.Now.TimeOfDay.ToString());

        bool isEmptyFrame = true;

        for (int i = 0; i < frameMsg.operation.Count; i++)
        {
            var oper = frameMsg.operation[i];
 //           Debug.Log("operation id:" + oper.entityid +"entity:" + entity);

            if (oper.entityid != entity.id)
            {
                continue;
            }

            isEmptyFrame = false;
            var data = new FrameData();
            data.duration = playTime;
            data.operation.Add(oper);

            framePool.Enqueue(new KeyValuePair<UInt32, FrameData>(curreFrameId, data));
            lastFrameData = data;
        }

        if (isEmptyFrame && lastFrameData != null)
        {
            framePool.Enqueue(new KeyValuePair<UInt32, FrameData>(curreFrameId, lastFrameData));
        }

    }

    // Update is called once per frame
    void UpdateOrigin () {

        if (!isAvatar)
            return;

        FP dis = TSVector.Distance(Position, destPosition);
        FP currSpeed = DestDuration <=0 ? Speed : (Speed * playTime / DestDuration);

        if(dis <= currSpeed * Time.deltaTime)
        {
            Position = destPosition;
//           Debug.LogError("----------diff time------------------:" + (playTime - FrameDuration));
        }
        else
        {
            TSVector tempDirection = destPosition - Position;

            Position += tempDirection.normalized * currSpeed * Time.deltaTime;
        }



        FrameDuration += Time.deltaTime;

        if (FrameDuration >= DestDuration)
        {
            Position = destPosition;

            if (framePool.Count > 0)
            {   
                DestDuration = playTime / (  framePool.Count <= ThresholdFrame ? 1: framePool.Count/ ThresholdFrame);

//                 if(framePool.Count > 8)
//                     Debug.LogError("framePool.Count too big:" + +framePool.Count);

                var framedata = framePool.Dequeue();

               

                emptyFramesTime = 0.0f;
 //               ThresholdFrame -= 1;

 //               Debug.Log("frame.id:"+ framedata.Key + " framePool.Count" + framePool.Count );

                TSVector movement = TSVector.zero;
                bool space = false;

                foreach (var item in framedata.Value.operation)
                {
                    if (item.cmd_type != (UInt32)CMD.BOX)
                    {
                        continue;
                    }

                    FrameBox msg = FrameProto.decode(item) as FrameBox;
                    movement = msg.movement;
                    space = msg.space;
                }

               
                //               Debug.Log("d_point:" + point);

                destPosition += Speed * movement * playTime;

                FrameDuration = 0.0f;
            }
            else if(lastFrameData != null)
            {
//                Debug.Log("emptyFramesTime," + emptyFramesTime);

                emptyFramesTime += Time.deltaTime;

                if(emptyFramesTime >= playTime)
                {
//                    ThresholdFrame = (int)(emptyFramesTime / playTime);

 //                   Debug.LogError("one frame time out,emptyFramesTime:" + emptyFramesTime + ",ThresholdFrame:"+ ThresholdFrame);
                }
            }
        }
    }

    private void Update()
    {
        if (!isAvatar)
            return;

        FrameDuration += Time.deltaTime;

        if (FrameDuration >= DestDuration)
        {
            FrameDuration = 0;

            if (framePool.Count > 0)
            {
                DestDuration = playTime / (framePool.Count <= ThresholdFrame ? 1 : framePool.Count / ThresholdFrame);

                var framedata = framePool.Dequeue();

                TSVector movement = TSVector.zero;
                bool space = false;

                foreach (var item in framedata.Value.operation)
                {
                    if (item.cmd_type != (UInt32)CMD.BOX)
                    {
                        continue;
                    }

                    FrameBox msg = FrameProto.decode(item) as FrameBox;
                    movement = msg.movement;
                    space = msg.space;
                }
                //KBEngine.Event.fireOut("recieveFrameTick", new object[] {entity.id, movement, space });
            }

        }
    }
}
