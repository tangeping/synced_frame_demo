using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KBEngine;
using System;

public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;
    private bool PannelInit = false;

    private int PlayerCount = 0;

    #region 生命周期

    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
    }

    public void Start()
    {
        KBEngine.Event.registerOut("onGameBegine", this, "onGameBegine");
        KBEngine.Event.registerOut("onReadyState", this, "onReadyState");
        KBEngine.Event.registerOut("onLoginOutSpaceResult", this, "onLoginOutSpaceResult");
    }

    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //组件
        for (int i = 0; i < 6; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(name);
            //prefab.gameObject.SetActive(false);
            prefabs.Add(prefab);
        }
        Debug.Log("player:" + GameData.Instance.localPlayer.id + ",creater:" + GameData.Instance.CurrentRoom.space_creater);
 //       if (GameData.Instance.IsCreater())
        {
            closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
            startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
            //按钮事件
            closeBtn.onClick.AddListener(OnCloseClick);
            startBtn.onClick.AddListener(OnStartClick);
        }
 
        
    }

    public override void OnClosing()
    {
        OnDestroy();
    }

    public void RecvGetRoomInfo()
    {
 //       Debug.Log(" RoomPanel::RecvGetRoomInfo." + GameData.Instance.RoomPlayers.Count);

        int i = 0;
        for(; i< GameData.Instance.RoomPlayers.Count;i++)
        {            
            KBEngine.Avatar avatar = GameData.Instance.RoomPlayers[i];       
            Int32 win = avatar.component1.winScore;
            Int32 fail = avatar.component1.lossScore;
 //           Debug.Log(" avatar." + avatar.id + ",isPlayer:" + avatar.isPlayer());

            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            string str = "名字：" + avatar.id + "\r\n";
            str += "阵营：" + (!avatar.isPlayer() ? "红" : "蓝") + "\r\n";
            str += "胜利：" + win.ToString() + "   ";
            str += "失败：" + fail.ToString() + "\r\n";
            if (avatar.isPlayer())
                str += "【我自己】";
            if (avatar.id == GameData.Instance.CurrentRoom.space_creater)
                str += "【房主】";
            text.text = str;

            if (avatar.isPlayer())
                trans.GetComponent<Image>().color = Color.blue;
            else
                trans.GetComponent<Image>().color = Color.red;

        }

        for (; i < 6; i++)
        {
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            text.text = "【等待玩家】";
            trans.GetComponent<Image>().color = Color.gray;
        }

    }

    public void OnCloseClick()
    {
        KBEngine.Event.fireIn("reqLoginOutSpace");
    }


    public void onLoginOutSpaceResult(Int32 entityId, ulong spaceKey)
    {
        //处理
        if (spaceKey > 0)
        {
            KBEngine.Entity entity =  KBEngine.KBEngineApp.app.findEntity(entityId);
            if (entity != null)
            {
                GameData.Instance.RoomPlayers.Remove((KBEngine.Avatar)entity);
                if(entity.id == GameData.Instance.localPlayer.id)
                {
                    PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功!");
                    PanelMgr.instance.OpenPanel<RoomListPanel>("");
                    Close();
                }
            }
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }


    public void OnStartClick()
    {
        sbyte ready = 1;
        KBEngine.Event.fireIn("reqReady", ready);
        Debug.Log(name + "::OnStartClick,ready:" + ready);
    }
    public void onReadyState(sbyte state)
    {
        Debug.Log(name + "::onReadyResult,state:" + state);

        if(state == 1)
        {
            startBtn.interactable = false;
        }
    }

    public void onGameBegine(ulong randSeed)
    {
        //处理
        //         if (result != 0)
        //         {
        //             PanelMgr.instance.OpenPanel<TipPanel>("", "开始游戏失败！两队至少都需要一名玩家，只有队长可以开始战斗！");
        //         }
        Debug.Log("onGameBegine.randSeed:" + randSeed);
        RecvFight();
    }

    /// <summary>
    /// 接收到‘开始战斗’消息
    /// </summary>

    public void RecvFight()
    {
        // 加载战斗场景
        SceneManager.LoadScene("battle");

        Close();
    }

    public void FixedUpdate()
    {
        if(PlayerCount != GameData.Instance.RoomPlayers.Count )
        {
            PlayerCount = GameData.Instance.RoomPlayers.Count;
            RecvGetRoomInfo();
        }
    }
    #endregion
}