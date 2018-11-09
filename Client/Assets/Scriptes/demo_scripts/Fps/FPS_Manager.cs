using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBFrame.Core;
using CBFrame.Sys;
using KBEngine;
using System;
using TrueSync;


namespace SyncFrame
{
    public class FPS_Manager : MonoBehaviour
    {

        /**
         *  @brief Represents the simulated gravity.
         **/
        public TrueSyncConfig Config;
        /**
         * @brief Instance of the lockstep engine.
         **/
        private AbstractLockstep lockstep;

        public static FPS_Manager instance;

        public GameObject playerPerfab;

        public UInt32 readoffset = 0;

        public UInt32 currenFrameId = 0;

        public Dictionary<int, PlayerBehaviour> playerBehaviours = new Dictionary<int, PlayerBehaviour>();
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

            for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
            {
                KBEngine.Avatar player = GameData.Instance.RoomPlayers[i];


                if (player.component1.isWathcher > 0)
                {
                    continue;
                }

                TSVector position = new TSVector(0f, 1f, 0f);
                TSVector direciton = new TSVector(0f, 180f, 0f);

                GameObject perfab = SyncedInstantiate(playerPerfab, position, TSQuaternion.Euler(direciton));

                Debug.Log("SyncManager::CreatePlayer.player.renderObj:" + (perfab == null ? "Null" : perfab.name)
                    + ",position:" + perfab.transform.position);

                PlayerBehaviour playerScript = perfab.AddComponent<PlayerBehaviour>();
                playerScript.owner = player;
                player.renderObj = perfab;

                playerBehaviours.Add(player.id, playerScript);
            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(Screen.width - 100, 1, 200, 200), "FrameID:" + currenFrameId.ToString());

            if (GUI.Button(new Rect(10, 40, 50, 30), "pause"))
            {
                KBEngine.Event.fireIn("reqGamePause");
            }
            else if (GUI.Button(new Rect(100, 40, 50, 30), "run"))
            {
                KBEngine.Event.fireIn("reqGameRunning");
            }
        }

        void Start()
        {
            instance = this;
            CreatePlayer();
            OnSyncedPlayerStart();
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

        void FixedUpdate()
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
                    for (int i = 0; i < framedata.operation.Count; i++)
                    {
                        ENTITY_DATA oper = framedata.operation[i];

                        if(playerBehaviours.ContainsKey(oper.entityid))
                        {
                            playerBehaviours[oper.entityid].OnSyncedUpdate(currenFrameId, oper);
                        }
                        
                    }
  
                    PhysicsManager.instance.UpdateStep();                  
                }

                OnSyncedPlayerInput();
            }
        }

        void OnSyncedPlayerStart()
        {
            for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
            {
                GameObject player = (GameObject)GameData.Instance.RoomPlayers[i].renderObj;

                if (player == null)
                {
                    continue;
                }

                PlayerBehaviour bh = player.GetComponent<PlayerBehaviour>();

                if (bh != null && bh.isActiveAndEnabled)
                {
                    bh.OnSyncedStart();
                }
            }
        }

        void OnSyncedPlayerInput()
        {
            for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
            {
                GameObject player = (GameObject)GameData.Instance.RoomPlayers[i].renderObj;

                if (player == null)
                {
                    continue;
                }

                PlayerBehaviour bh = player.GetComponent<PlayerBehaviour>();

                if (bh != null && bh.isActiveAndEnabled)
                {
                    bh.OnSyncedInput();
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

            TSTransform rootTSTransform = go.GetComponent<TSTransform>();
            if (rootTSTransform != null)
            {
                rootTSTransform.Initialize();

                rootTSTransform.position = position;
                rootTSTransform.rotation = rotation;
            }

            TSTransform[] tsTransforms = go.GetComponentsInChildren<TSTransform>();
            if (tsTransforms != null)
            {
                for (int index = 0, length = tsTransforms.Length; index < length; index++)
                {
                    TSTransform tsTransform = tsTransforms[index];

                    if (tsTransform != rootTSTransform)
                    {
                        tsTransform.Initialize();
                    }
                }
            }

            TSTransform2D rootTSTransform2D = go.GetComponent<TSTransform2D>();
            if (rootTSTransform2D != null)
            {
                rootTSTransform2D.Initialize();

                rootTSTransform2D.position = new TSVector2(position.x, position.y);
                rootTSTransform2D.rotation = rotation.ToQuaternion().eulerAngles.z;
            }

            TSTransform2D[] tsTransforms2D = go.GetComponentsInChildren<TSTransform2D>();
            if (tsTransforms2D != null)
            {
                for (int index = 0, length = tsTransforms2D.Length; index < length; index++)
                {
                    TSTransform2D tsTransform2D = tsTransforms2D[index];

                    if (tsTransform2D != rootTSTransform2D)
                    {
                        tsTransform2D.Initialize();
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
