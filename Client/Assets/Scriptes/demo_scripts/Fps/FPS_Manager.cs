using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBFrame.Core;
using CBFrame.Sys;
using KBEngine;
using System;



namespace SyncFrame
{
    public class FPS_Manager : MonoBehaviour
    {

        /**
         *  @brief Represents the simulated gravity.
         **/
        public FrameSyncConfig Config;
        /**
         * @brief Instance of the lockstep engine.
         **/
        private AbstractLockstep lockstep;

        public static FPS_Manager instance;

        public GameObject playerPerfab;

        public UInt32 readoffset = 0;

        public UInt32 currenFrameId = 0;

        public Camera[] cameras;

        public GameObject cameraTransform;

        public Dictionary<int, PlayerBehaviour> playerBehaviours = new Dictionary<int, PlayerBehaviour>();

        /**
         * @brief List of {@link FrameSyncBehaviour} that should be included next update.
         **/
        public List<FrameSyncManagedBehaviour> queuedBehaviours = new List<FrameSyncManagedBehaviour>();

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

                 TSVector position = new TSVector(player.position.x, player.position.y, player.position.z);
                 TSVector direciton = new TSVector(0, 180, 0);
                //Debug.Log("SyncManager::CreatePlayer.player.position:"+ player.position + ",direciton:"+ player.direction);

                GameObject perfab = SyncedInstantiate(playerPerfab, position, TSQuaternion.Euler(direciton));

                Debug.Log("SyncManager::CreatePlayer.player.renderObj:" + (perfab == null ? "Null" : perfab.name)
                    + ",position:" + perfab.transform.position +",direction:"+perfab.transform.eulerAngles);

                perfab.name = player.className + "_" + player.id;
                PlayerBehaviour playerScript = perfab.AddComponent<PlayerBehaviour>();
                playerScript.owner = player;
                player.renderObj = perfab;          
                playerBehaviours.Add(player.id, playerScript);

                if(player.isPlayer())
                {
                    cameraTransform.GetComponent<CamerFllown>().AttachTarget(perfab.transform);
                    cameraTransform.transform.parent = perfab.transform;
                }
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

            if (GUI.Button(new Rect(200, 40, 80, 30), "camra_A"))
            {
                foreach (Camera c in cameras)
                {
                    c.gameObject.SetActive(c.name == "A");
                }
            }
            else if (GUI.Button(new Rect(300, 40, 80, 30), "camra_B"))
            {
                foreach (Camera c in cameras)
                {
                    c.gameObject.SetActive(c.name == "B");
                }
            }
            else if (GUI.Button(new Rect(400, 40, 80, 30), "camra_C"))
            {
                foreach (Camera c in cameras)
                {
                    c.gameObject.SetActive(c.name == "C");
                }
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

        void Update()
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

                    if(framedata.operation.Count == 1 && framedata.operation[0].cmd_type == (Byte)SyncFrame.CMD.EMPTY)
                    {
                        for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
                        {
                            int entityId = GameData.Instance.RoomPlayers[i].id;
                            playerBehaviours[entityId].OnSyncedUpdate(currenFrameId, (new FrameFPS()).Serialize());
                        }                   
                    }
                    else
                    {
                        for (int i = 0; i < framedata.operation.Count; i++)
                        {
                            ENTITY_DATA oper = framedata.operation[i];
                            if (playerBehaviours.ContainsKey(oper.entityid))
                            {
                                playerBehaviours[oper.entityid].OnSyncedUpdate(currenFrameId, oper);
                            }

                        }
                    }

                    foreach (var item in instance.queuedBehaviours)
                    {
                        item.OnSyncedUpdate();
                    }
                    PhysicsManager.instance.UpdateStep();
                    
                    OnSyncedPlayerInput();
                }

                
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

        private FrameSyncManagedBehaviour NewManagedBehavior(IFrameSyncBehaviour FrameSyncBehavior)
        {
            FrameSyncManagedBehaviour result = new FrameSyncManagedBehaviour(FrameSyncBehavior);
            //mapBehaviorToManagedBehavior[FrameSyncBehavior] = result;

            return result;
        }

        private static void InitializeGameObject(GameObject go, TSVector position, TSQuaternion rotation)
        {

            MonoBehaviour[] monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>();
            for (int index = 0, length = monoBehaviours.Length; index < length; index++)
            {
                MonoBehaviour bh = monoBehaviours[index];

                if (bh is IFrameSyncBehaviour)
                {
                    instance.queuedBehaviours.Add(instance.NewManagedBehavior((IFrameSyncBehaviour)bh));
                }
            }


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
