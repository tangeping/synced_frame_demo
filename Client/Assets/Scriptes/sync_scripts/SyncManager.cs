using CBFrame.Core;
using CBFrame.Sys;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

namespace SyncFrame
{
    public class SyncManager : MonoBehaviour {

        /**
         *  @brief Represents the simulated gravity.
         **/
        public FrameSyncConfig Config;
        /**
         * @brief Instance of the lockstep engine.
         **/
        private AbstractLockstep lockstep;

        private static SyncManager instance;

        public GameObject playerPerfab;

        public UInt32 readoffset = 0;

        public UInt32 currenFrameId = 0;
        private void Awake()
        {
            PhysicsManager.New(Config);
            PhysicsManager.instance.LockedTimeStep = Config.lockedTimeStep;
            PhysicsManager.instance.Init();
           
        }
        // Use this for initialization

        public void LockStepUpate(KBEngine.Entity entity, FRAME_DATA framedata)
        {
            lockstep = AbstractLockstep.NewInstance(Config.lockedTimeStep, Config, PhysicsManager.instance);
        }
        void CreatePlayer()
        {
            Debug.Log("SyncManager::CreatePlayer.count:" + GameData.Instance.RoomPlayers.Count);

            for(int i = 0;i< GameData.Instance.RoomPlayers.Count;i++)
            {
                KBEngine.Avatar player = GameData.Instance.RoomPlayers[i];

                if(player.component1.isWathcher > 0)
                {
                    continue;
                }

                //TSVector bronPosition = new TSVector(-3 + (i - 1) * 4, 1, 16);
                TSVector position = new TSVector(0f, 1f, 0f);
                TSVector direciton = new TSVector(0f, 180f, 0f);

                GameObject perfab =  SyncedInstantiate(playerPerfab, position, TSQuaternion.Euler(direciton));
           
                Debug.Log("SyncManager::CreatePlayer.player.renderObj:" + (perfab == null? "Null": perfab.name)
                    +",position:" + perfab.transform.position );

                PlayerContorl playerScript =  perfab.AddComponent<PlayerContorl>();
                playerScript.owner = player;

//                 GameEntity gameEntity = perfab.AddComponent<GameEntity>();
//                 gameEntity.entity = player;
//                 gameEntity.name = player.className + "_" + player.id;
// 
//                 gameEntity.isPlayer = true;
//                 gameEntity.isAvatar = true;
                player.renderObj = perfab;

            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(Screen.width - 100, 1, 200, 200),"FrameID:"+ currenFrameId.ToString());

            if(GUI.Button(new Rect(10, 40, 50, 30), "pause"))
            {
                KBEngine.Event.fireIn("reqGamePause");
                PlayerBehaviourStop();
            }
            else if(GUI.Button(new Rect(100, 40, 50, 30), "run"))
            {
                KBEngine.Event.fireIn("reqGameRunning");
            }
        }

        void Start() {
            instance = this;
            CreatePlayer();
            //GameData.Instance.frameList.Clear(); //游戏还没开始之前的数据无效
        }

        private FP tsDeltaTime = 0;
        private FP tsDuration = 0;
        private int thresholdFrame = 1;
        public int thresholdMaxFrame = 30;

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

        void FixedUpdateOld()
        {
            tsDeltaTime += Time.deltaTime;

            if (tsDeltaTime >= tsDuration)
            {
                tsDeltaTime = 0;

                if (GameData.Instance.frameList.Count > 0)
                {
                    int count = GameData.Instance.frameList.Count;
                    tsDuration = Config.lockedTimeStep / (count <= ThresholdFrame ? 1 : count / ThresholdFrame);

                    FRAME_DATA framedata = GameData.Instance.frameList.Dequeue();
                    currenFrameId = framedata.frameid;
                    if (currenFrameId < 4000)
                    {
                        PhysicsManager.instance.UpdateStep();

                        KBEngine.Event.fireOut("recieveFrameTick", new object[] { framedata });
                    }

                    

                }

            }
        }

        void PlayerBehaviour()
        {
            for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
            {
                GameObject player = (GameObject)GameData.Instance.RoomPlayers[i].renderObj;

                if (player == null)
                {
                    continue;
                }

                PlayerContorl playerScript = player.GetComponent<PlayerContorl>();

                if (playerScript != null && playerScript.isActiveAndEnabled)
                {
                    playerScript.OnSyncedUpdate();
                }
            }
        }

        void PlayerBehaviourStop()
        {
            for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
            {
                GameObject player = (GameObject)GameData.Instance.RoomPlayers[i].renderObj;

                if (player == null)
                {
                    continue;
                }

                PlayerContorl playerScript = player.GetComponent<PlayerContorl>();

                if (playerScript != null && playerScript.isActiveAndEnabled)
                {
                    playerScript.OnSyncedGamePause();
                }
            }
        }

        private void Update()
        {
            if (GameData.Instance.frameList.Count > 0)
            {
                FRAME_DATA framedata = GameData.Instance.frameList.Dequeue();

                currenFrameId = framedata.frameid;
                if (currenFrameId < 4000)
                {
                    PhysicsManager.instance.UpdateStep();

                    PlayerBehaviour();

                    //KBEngine.Event.fireOut("recieveFrameTick", new object[] { framedata });

                }
 
            }
        }

        private static void InitializeGameObject(GameObject go, TSVector position, TSQuaternion rotation)
        {
            ICollider[] tsColliders = go.GetComponentsInChildren<ICollider>();
            if (tsColliders != null)
            {
                for (int index = 0, length = tsColliders.Length; index < length; index++)
                {
                    PhysicsManager.instance.AddBody(tsColliders[index]);
                }
            }

            FPTransform rootFPTransform = go.GetComponent<FPTransform>();
            if (rootFPTransform != null)
            {
                rootFPTransform.Initialize();

                rootFPTransform.position = position;
                rootFPTransform.rotation = rotation;
            }

            FPTransform[] FPTransforms = go.GetComponentsInChildren<FPTransform>();
            if (FPTransforms != null)
            {
                for (int index = 0, length = FPTransforms.Length; index < length; index++)
                {
                    FPTransform FPTransform = FPTransforms[index];

                    if (FPTransform != rootFPTransform)
                    {
                        FPTransform.Initialize();
                    }
                }
            }

            FPTransform2D rootFPTransform2D = go.GetComponent<FPTransform2D>();
            if (rootFPTransform2D != null)
            {
                rootFPTransform2D.Initialize();

                rootFPTransform2D.position = new TSVector2(position.x, position.y);
                rootFPTransform2D.rotation = rotation.ToQuaternion().eulerAngles.z;
            }

            FPTransform2D[] FPTransforms2D = go.GetComponentsInChildren<FPTransform2D>();
            if (FPTransforms2D != null)
            {
                for (int index = 0, length = FPTransforms2D.Length; index < length; index++)
                {
                    FPTransform2D FPTransform2D = FPTransforms2D[index];

                    if (FPTransform2D != rootFPTransform2D)
                    {
                        FPTransform2D.Initialize();
                    }
                }
            }
        }
        /**
        * @brief Instantiate a new prefab in a deterministic way.
        * 
        * @param prefab GameObject's prefab to instantiate.
        **/
        public static GameObject SyncedInstantiate(GameObject prefab)
        {
            return SyncedInstantiate(prefab, prefab.transform.position.ToTSVector(), prefab.transform.rotation.ToTSQuaternion());
        }

        /**
         * @brief Instantiates a new prefab in a deterministic way.
         * 
         * @param prefab GameObject's prefab to instantiate.
         * @param position Position to place the new GameObject.
         * @param rotation Rotation to set in the new GameObject.
         **/
        public static GameObject SyncedInstantiate(GameObject prefab, TSVector position, TSQuaternion rotation)
        {
            GameObject go = GameObject.Instantiate(prefab, position.ToVector(), rotation.ToQuaternion()) as GameObject;
            InitializeGameObject(go, position, rotation);
            return go;
        }
    }
}
