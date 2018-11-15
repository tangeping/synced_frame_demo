using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KBEngine {
    /**
     * @brief Manages creation of player prefabs and lockstep execution.
     **/
    public class FrameSyncManager : MonoBehaviour {

        private const float JitterTimeFactor = 0.001f;

        private const string serverSettingsAssetFile = "FrameSyncGlobalConfig";

        private enum StartState { BEHAVIOR_INITIALIZED, FIRST_UPDATE, STARTED };

        private StartState startState;

        public TSVector TestCenter;
        /** 
         * @brief Player prefabs to be instantiated in each machine.
         **/
        public GameObject[] playerPrefabs;

        public static FrameSyncConfig _FrameSyncGlobalConfig;

        public static FrameSyncConfig FrameSyncGlobalConfig {
            get {
                if (_FrameSyncGlobalConfig == null) {
                    _FrameSyncGlobalConfig = (FrameSyncConfig) Resources.Load(serverSettingsAssetFile, typeof(FrameSyncConfig));
                }

                return _FrameSyncGlobalConfig;
            }
        }

        public static FrameSyncConfig FrameSyncCustomConfig = null;

        public FrameSyncConfig customConfig;

        private Dictionary<int, List<GameObject>> gameOjectsSafeMap = new Dictionary<int, List<GameObject>>();

        /**
         * @brief Instance of the lockstep engine.
         **/
        private AbstractLockstep lockstep;

        private FP lockedTimeStep;

        /**
         * @brief A list of {@link FrameSyncBehaviour} not linked to any player.
         **/
        private List<FrameSyncManagedBehaviour> generalBehaviours = new List<FrameSyncManagedBehaviour>();

        /**
         * @brief A dictionary holding a list of {@link FrameSyncBehaviour} belonging to each player.
         **/
        private Dictionary<byte, List<FrameSyncManagedBehaviour>> behaviorsByPlayer;

        /**
         * @brief The coroutine scheduler.
         **/
        private CoroutineScheduler scheduler;

        /**
         * @brief List of {@link FrameSyncBehaviour} that should be included next update.
         **/
        private List<FrameSyncManagedBehaviour> queuedBehaviours = new List<FrameSyncManagedBehaviour>();

        private Dictionary<IFrameSyncBehaviour, FrameSyncManagedBehaviour> mapBehaviorToManagedBehavior = new Dictionary<IFrameSyncBehaviour, FrameSyncManagedBehaviour>();

        private FP time = 0;

        /**
         * @brief Returns the deltaTime between two simulation calls.
         **/
        public static FP DeltaTime {
            get {
                if (instance == null) {
                    return 0;
                }

                return instance.lockedTimeStep;
            }
        }

        /**
         * @brief Returns the time elapsed since the beginning of the simulation.
         **/
        public static FP Time {
            get {
                if (instance == null || instance.lockstep == null) {
                    return 0;
                }

                return instance.time;
            }
        }

        /**
         * @brief Returns the number of the last simulated tick.
         **/
        public static int Ticks {
            get {
                if (instance == null || instance.lockstep == null) {
                    return 0;
                }

                return instance.lockstep.Ticks;
            }
        }

        /**
         * @brief Returns the last safe simulated tick.
         **/
        public static int LastSafeTick {
            get {
                if (instance == null || instance.lockstep == null) {
                    return 0;
                }

                return instance.lockstep.LastSafeTick;
            }
        }

        /** 
         * @brief Returns the simulated gravity.
         **/
        public static TSVector Gravity {
            get {
                if (instance == null) {
                    return TSVector.zero;
                }

                return instance.ActiveConfig.gravity3D;
            }
        }

        /** 
         * @brief Returns the list of players connected.
         **/
        public static List<FPPlayerInfo> Players {
            get {
                if (instance == null || instance.lockstep == null) {
                    return null;
                }

                List<FPPlayerInfo> allPlayers = new List<FPPlayerInfo>();
                foreach (FPPlayer tsp in instance.lockstep.Players.Values) {
                    if (!tsp.dropped) {
                        allPlayers.Add(tsp.playerInfo);
                    }
                }

                return allPlayers;
            }
        }

        /** 
         * @brief Returns the local player.
         **/
        public static FPPlayerInfo LocalPlayer {
            get {
                if (instance == null || instance.lockstep == null) {
                    return null;
                }

                return instance.lockstep.LocalPlayer.playerInfo;
            }
        }

        /** 
         * @brief Returns the active {@link FrameSyncConfig} used by the {@link FrameSyncManager}.
         **/
        public static FrameSyncConfig Config {
            get {
                if (instance == null) {
                    return null;
                }

                return instance.ActiveConfig;
            }
        }

        private static FrameSyncManager instance;

        private FrameSyncConfig ActiveConfig {
            get {
                if (FrameSyncCustomConfig != null) {
                    customConfig = FrameSyncCustomConfig;
                    FrameSyncCustomConfig = null;
                }

                if (customConfig != null) {
                    return customConfig;
                }

                return FrameSyncGlobalConfig;
            }
        }

        void Awake() {
            FrameSyncConfig currentConfig = ActiveConfig;
            lockedTimeStep = currentConfig.lockedTimeStep;

            StateTracker.Init(currentConfig.rollbackWindow);

            // TODO: 随机数种子在这里指定，需要修改为由 Server 统一指定
            TSRandom.Init();

            if (currentConfig.physics2DEnabled || currentConfig.physics3DEnabled) {
                PhysicsManager.New(currentConfig);
                PhysicsManager.instance.LockedTimeStep = lockedTimeStep;
                PhysicsManager.instance.Init();
            }

            StateTracker.AddTracking(this, "time");
        }

        void Start() {
            instance = this;
            Application.runInBackground = true;

            ICommunicator communicator = null;

//            if (!PhotonNetwork.connected || !PhotonNetwork.inRoom) {
//                Debug.LogWarning("You are not connected to Photon. FrameSync will start in offline mode.");
//            } else {
//                communicator = new PhotonFrameSyncCommunicator(PhotonNetwork.networkingPeer);
//            }


//             if (NetMgr.Instance.srvConn.status == Connection.Status.None)
//             {
//                 Debug.LogWarning("You are not connected to Server. FrameSync will start in offline mode.");
//             }
//             else
//             {
//                 communicator = new RealSyncCommunicator();
//             }

            FrameSyncConfig activeConfig = ActiveConfig;

            lockstep = AbstractLockstep.NewInstance(
                lockedTimeStep.AsFloat(),
                communicator,
                PhysicsManager.instance,
                activeConfig.syncWindow,
                activeConfig.panicWindow,
                activeConfig.rollbackWindow,
                OnGameStarted,
                OnGamePaused,
                OnGameUnPaused,
                OnGameEnded,
                OnPlayerDisconnection,
                OnStepUpdate,
                GetLocalData,
                ProvideInputData
            );

            if (ReplayRecord.replayMode == ReplayMode.LOAD_REPLAY) {
 //               ReplayPicker.replayToLoad.Load();

                ReplayRecord replayRecord = ReplayRecord.replayToLoad;
                if (replayRecord == null) {
                    Debug.LogError("Replay Record can't be loaded");
                    gameObject.SetActive(false);
                    return;
                } else {
                    lockstep.ReplayRecord = replayRecord;
                }
            }

            if (activeConfig.showStats) {
                this.gameObject.AddComponent<FrameSyncStats>().Lockstep = lockstep;
            }

            scheduler = new CoroutineScheduler(lockstep);

            if (ReplayRecord.replayMode != ReplayMode.LOAD_REPLAY) {
                if (communicator == null) {
                    lockstep.AddPlayer(0, "Local_Player", true);
                } else
                {
                    //                    List<PhotonPlayer> players = new List<PhotonPlayer>(PhotonNetwork.playerList);
                    //                    players.Sort(UnityUtils.playerComparer);
                    //
                    //                    for (int index = 0, length = players.Count; index < length; index++) {
                    //                        PhotonPlayer p = players[index];
                    //                        lockstep.AddPlayer((byte) p.ID, p.NickName, p.IsLocal);
                    //                    }

//                     for (int i = 0; i < GameData.Instance.RoomPlayers.Count; i++)
//                     {
//                         KBEngine.Avatar player = GameData.Instance.RoomPlayers[i];
//                         lockstep.AddPlayer((byte)(i + 1), player.id.ToString(), player.isPlayer());
//                     }
                }
            }

            FrameSyncBehaviour[] behavioursArray = FindObjectsOfType<FrameSyncBehaviour>();
            for (int index = 0, length = behavioursArray.Length; index < length; index++) {
                generalBehaviours.Add(NewManagedBehavior(behavioursArray[index]));
            }

            initBehaviors();
            initGeneralBehaviors(generalBehaviours, false);

            PhysicsManager.instance.OnRemoveBody(OnRemovedRigidBody);

            startState = StartState.BEHAVIOR_INITIALIZED;
        }

        private FrameSyncManagedBehaviour NewManagedBehavior(IFrameSyncBehaviour FrameSyncBehavior) {
            FrameSyncManagedBehaviour result = new FrameSyncManagedBehaviour(FrameSyncBehavior);
            mapBehaviorToManagedBehavior[FrameSyncBehavior] = result;

            return result;
        }

        private void initBehaviors() {
            behaviorsByPlayer = new Dictionary<byte, List<FrameSyncManagedBehaviour>>();
            List<TSVector> playerPosition = new List<TSVector>();
            playerPosition.Add(new TSVector(-5.0f, 0, 0));
            playerPosition.Add(new TSVector(-5.0f, 0, 5.0f));
            int playerIndex = 0;

            var playersEnum = lockstep.Players.GetEnumerator();
            while (playersEnum.MoveNext()) {
                FPPlayer p = playersEnum.Current.Value;

                List<FrameSyncManagedBehaviour> behaviorsInstatiated = new List<FrameSyncManagedBehaviour>();

                for (int index = 0, length = playerPrefabs.Length; index < length; index++) {
                    GameObject prefab = playerPrefabs[index];

                    GameObject prefabInst = Instantiate(prefab);
                    prefabInst.transform.position = playerPosition[playerIndex].ToVector();
                    InitializeGameObject(prefabInst, prefabInst.transform.position.ToTSVector(), prefabInst.transform.rotation.ToTSQuaternion());

                    FrameSyncBehaviour[] behaviours = prefabInst.GetComponentsInChildren<FrameSyncBehaviour>();
                    for (int index2 = 0, length2 = behaviours.Length; index2 < length2; index2++) {
                        FrameSyncBehaviour behaviour = behaviours[index2];

                        behaviour.owner = p.playerInfo;
                        behaviour.localOwner = lockstep.LocalPlayer.playerInfo;
                        behaviour.numberOfPlayers = lockstep.Players.Count;

                        FrameSyncManagedBehaviour tsmb = NewManagedBehavior(behaviour);
                        tsmb.owner = behaviour.owner;
                        tsmb.localOwner = behaviour.localOwner;

                        behaviorsInstatiated.Add(tsmb);
                    }
                }

                behaviorsByPlayer.Add(p.ID, behaviorsInstatiated);

                playerIndex++;
            }
        }

        private void initGeneralBehaviors(IEnumerable<FrameSyncManagedBehaviour> behaviours, bool realOwnerId) {
            List<FPPlayer> playersList = new List<FPPlayer>(lockstep.Players.Values);
            List<FrameSyncManagedBehaviour> itemsToRemove = new List<FrameSyncManagedBehaviour>();

            var behavioursEnum = behaviours.GetEnumerator();
            while (behavioursEnum.MoveNext()) {
                FrameSyncManagedBehaviour tsmb = behavioursEnum.Current;

                if (!(tsmb.FrameSyncBehavior is FrameSyncBehaviour)) {
                    continue;
                }

                FrameSyncBehaviour bh = (FrameSyncBehaviour)tsmb.FrameSyncBehavior;

                if (realOwnerId) {
                    bh.ownerIndex = bh.owner.Id;
                } else {
                    if (bh.ownerIndex >= 0 && bh.ownerIndex < playersList.Count) {
                        bh.ownerIndex = playersList[bh.ownerIndex].ID;
                    }
                }

                if (behaviorsByPlayer.ContainsKey((byte)bh.ownerIndex)) {
                    bh.owner = lockstep.Players[(byte)bh.ownerIndex].playerInfo;

                    behaviorsByPlayer[(byte)bh.ownerIndex].Add(tsmb);
                    itemsToRemove.Add(tsmb);
                } else {
                    bh.ownerIndex = -1;
                }

                bh.localOwner = lockstep.LocalPlayer.playerInfo;
                bh.numberOfPlayers = lockstep.Players.Count;

                tsmb.owner = bh.owner;
                tsmb.localOwner = bh.localOwner;
            }

            for (int index = 0, length = itemsToRemove.Count; index < length; index++) {
                generalBehaviours.Remove(itemsToRemove[index]);
            }
        }

        private void CheckQueuedBehaviours() {
            if (queuedBehaviours.Count > 0) {
                generalBehaviours.AddRange(queuedBehaviours);
                initGeneralBehaviors(queuedBehaviours, true);

                for (int index = 0, length = queuedBehaviours.Count; index < length; index++) {
                    FrameSyncManagedBehaviour tsmb = queuedBehaviours[index];

                    tsmb.SetGameInfo(lockstep.LocalPlayer.playerInfo, lockstep.Players.Count);
                    tsmb.OnSyncedStart();
                }

                queuedBehaviours.Clear();
            }
        }

        void Update() {
            if (lockstep != null && startState != StartState.STARTED) {
                if (startState == StartState.BEHAVIOR_INITIALIZED) {
                    startState = StartState.FIRST_UPDATE;
                } else if (startState == StartState.FIRST_UPDATE) {
                    lockstep.RunSimulation(true);
                    startState = StartState.STARTED;
                }
            }
        }

        /**
         * @brief Run/Unpause the game simulation.
         **/
        public static void RunSimulation() {
            if (instance != null && instance.lockstep != null) {
                instance.lockstep.RunSimulation(false);
            }
        }

        /**
         * @brief Pauses the game simulation.
         **/
        public static void PauseSimulation() {
            if (instance != null && instance.lockstep != null) {
                instance.lockstep.PauseSimulation();
            }
        }

        /**
         * @brief End the game simulation.
         **/
        public static void EndSimulation() {
            if (instance != null && instance.lockstep != null) {
                instance.lockstep.EndSimulation();
            }
        }

        /**
         * @brief Update all coroutines created.
         **/
        public static void UpdateCoroutines() {
            if (instance != null && instance.lockstep != null) {
                instance.scheduler.UpdateAllCoroutines();
            }
        }

        /**
         * @brief Starts a new coroutine.
         * 
         * @param coroutine An IEnumerator that represents the coroutine.
         **/
        public static void SyncedStartCoroutine(IEnumerator coroutine) {
            if (instance != null && instance.lockstep != null) {
                instance.scheduler.StartCoroutine(coroutine);
            }
        }

        /**
         * @brief Instantiate a new prefab in a deterministic way.
         * 
         * @param prefab GameObject's prefab to instantiate.
         **/
        public static GameObject SyncedInstantiate(GameObject prefab) {
            return SyncedInstantiate(prefab, prefab.transform.position.ToTSVector(), prefab.transform.rotation.ToTSQuaternion());
        }

        /**
         * @brief Instantiates a new prefab in a deterministic way.
         * 
         * @param prefab GameObject's prefab to instantiate.
         * @param position Position to place the new GameObject.
         * @param rotation Rotation to set in the new GameObject.
         **/
        public static GameObject SyncedInstantiate(GameObject prefab, TSVector position, TSQuaternion rotation) {
            if (instance != null && instance.lockstep != null) {
                GameObject go = GameObject.Instantiate(prefab, position.ToVector(), rotation.ToQuaternion()) as GameObject;

                if (ReplayRecord.replayMode != ReplayMode.LOAD_REPLAY) {
                    AddGameObjectOnSafeMap(go);
                }

                MonoBehaviour[] monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>();
                for (int index = 0, length = monoBehaviours.Length; index < length; index++) {
                    MonoBehaviour bh = monoBehaviours[index];

                    if (bh is IFrameSyncBehaviour) {
                        instance.queuedBehaviours.Add(instance.NewManagedBehavior((IFrameSyncBehaviour)bh));
                    }
                }

                InitializeGameObject(go, position, rotation);

                return go;
            }

            return null;
        }

        private static void AddGameObjectOnSafeMap(GameObject go) {

            Dictionary<int, List<GameObject>> safeMap = instance.gameOjectsSafeMap;

            int currentTick = FrameSyncManager.Ticks + 1;
            if (!safeMap.ContainsKey(currentTick)) {
                safeMap.Add(currentTick, new List<GameObject>());
            }

            safeMap[currentTick].Add(go);
        }

        private static void CheckGameObjectsSafeMap() {
            Dictionary<int, List<GameObject>> safeMap = instance.gameOjectsSafeMap;

            int currentTick = FrameSyncManager.Ticks + 1;
            if (safeMap.ContainsKey(currentTick)) {
                List<GameObject> gos = safeMap[currentTick];
                for (int i = 0, l = gos.Count; i < l; i++) {
                    GameObject go = gos[i];
                    if (go != null) {
                        Renderer rend = go.GetComponent<Renderer>();
                        if (rend != null) {
                            rend.enabled = false;
                        }

                        GameObject.Destroy(go);
                    }
                }

                gos.Clear();
            }

            safeMap.Remove(FrameSyncManager.LastSafeTick);
        }

        private static void InitializeGameObject(GameObject go, TSVector position, TSQuaternion rotation) {
            ICollider[] tsColliders = go.GetComponentsInChildren<ICollider>();
            if (tsColliders != null) {
                for (int index = 0, length = tsColliders.Length; index < length; index++) {
                    PhysicsManager.instance.AddBody(tsColliders[index]);
                }
            }

            FPTransform rootFPTransform = go.GetComponent<FPTransform>();
            if (rootFPTransform != null) {
                rootFPTransform.Initialize();

                rootFPTransform.position = position;
                rootFPTransform.rotation = rotation;
            }

            FPTransform[] FPTransforms = go.GetComponentsInChildren<FPTransform>();
            if (FPTransforms != null) {
                for (int index = 0, length = FPTransforms.Length; index < length; index++) {
                    FPTransform FPTransform = FPTransforms[index];

                    if (FPTransform != rootFPTransform) {
                        FPTransform.Initialize();
                    }
                }
            }

            FPTransform2D rootFPTransform2D = go.GetComponent<FPTransform2D>();
            if (rootFPTransform2D != null) {
                rootFPTransform2D.Initialize();

                rootFPTransform2D.position = new TSVector2(position.x, position.y);
                rootFPTransform2D.rotation = rotation.ToQuaternion().eulerAngles.z;
            }

            FPTransform2D[] FPTransforms2D = go.GetComponentsInChildren<FPTransform2D>();
            if (FPTransforms2D != null) {
                for (int index = 0, length = FPTransforms2D.Length; index < length; index++) {
                    FPTransform2D FPTransform2D = FPTransforms2D[index];

                    if (FPTransform2D != rootFPTransform2D) {
                        FPTransform2D.Initialize();
                    }
                }
            }
        }

        /**
         * @brief Instantiates a new prefab in a deterministic way.
         * 
         * @param prefab GameObject's prefab to instantiate.
         * @param position Position to place the new GameObject.
         * @param rotation Rotation to set in the new GameObject.
         **/
        public static GameObject SyncedInstantiate(GameObject prefab, TSVector2 position, TSQuaternion rotation) {
            return SyncedInstantiate(prefab, new TSVector(position.x, position.y, 0), rotation);
        }

        /**
         * @brief Destroys a GameObject in a deterministic way.
         * 
         * The method {@link #DestroyFPRigidBody} is called and attached FrameSyncBehaviors are disabled.
         * 
         * @param rigidBody Instance of a {@link FPRigidBody}
         **/
        public static void SyncedDestroy(GameObject gameObject) {
            if (instance != null && instance.lockstep != null) {
                SyncedDisableBehaviour(gameObject);

                FPCollider[] tsColliders = gameObject.GetComponentsInChildren<FPCollider>();
                if (tsColliders != null) {
                    for (int index = 0, length = tsColliders.Length; index < length; index++) {
                        FPCollider tsCollider = tsColliders[index];
                        DestroyFPRigidBody(tsCollider.gameObject, tsCollider.Body);
                    }
                }

                FPCollider2D[] tsColliders2D = gameObject.GetComponentsInChildren<FPCollider2D>();
                if (tsColliders2D != null) {
                    for (int index = 0, length = tsColliders2D.Length; index < length; index++) {
                        FPCollider2D tsCollider2D = tsColliders2D[index];
                        DestroyFPRigidBody(tsCollider2D.gameObject, tsCollider2D.Body);
                    }
                }
            }
        }

        /**
         * @brief Disables 'OnSyncedInput' and 'OnSyncUpdate' calls to every {@link IFrameSyncBehaviour} attached.
         **/
        public static void SyncedDisableBehaviour(GameObject gameObject) {
            MonoBehaviour[] monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>();

            for (int index = 0, length = monoBehaviours.Length; index < length; index++) {
                MonoBehaviour tsb = monoBehaviours[index];

                if (tsb is IFrameSyncBehaviour && instance.mapBehaviorToManagedBehavior.ContainsKey((IFrameSyncBehaviour)tsb)) {
                    instance.mapBehaviorToManagedBehavior[(IFrameSyncBehaviour)tsb].disabled = true;
                }
            }
        }

        /**
         * @brief The related GameObject is firstly set to be inactive then in a safe moment it will be destroyed.
         * 
         * @param rigidBody Instance of a {@link FPRigidBody}
         **/
        private static void DestroyFPRigidBody(GameObject tsColliderGO, IBody body) {
            tsColliderGO.gameObject.SetActive(false);
            instance.lockstep.Destroy(body);
        }

        /**
         * @brief Registers an implementation of {@link IFrameSyncBehaviour} to be included in the simulation.
         * 
         * @param FrameSyncBehaviour Instance of an {@link IFrameSyncBehaviour}
         **/
        public static void RegisterIFrameSyncBehaviour(IFrameSyncBehaviour FrameSyncBehaviour) {
            if (instance != null && instance.lockstep != null) {
                instance.queuedBehaviours.Add(instance.NewManagedBehavior(FrameSyncBehaviour));
            }
        }

        /**
         * @brief Register a {@link FrameSyncIsReady} delegate to that returns true if the game can proceed or false otherwise.
         * 
         * @param IsReadyChecker A {@link FrameSyncIsReady} delegate
         **/
        public static void RegisterIsReadyChecker(FrameSyncIsReady IsReadyChecker) {
            if (instance != null && instance.lockstep != null) {
                instance.lockstep.GameIsReady += IsReadyChecker;
            }
        }

        /**
         * @brief Removes objets related to a provided player.
         * 
         * @param playerId Target player's id.
         **/
        public static void RemovePlayer(int playerId) {
            if (instance != null && instance.lockstep != null) {
                List<FrameSyncManagedBehaviour> behaviorsList = instance.behaviorsByPlayer[(byte)playerId];

                for (int index = 0, length = behaviorsList.Count; index < length; index++) {
                    FrameSyncManagedBehaviour tsmb = behaviorsList[index];
                    tsmb.disabled = true;

                    FPCollider[] tsColliders = ((FrameSyncBehaviour)tsmb.FrameSyncBehavior).gameObject.GetComponentsInChildren<FPCollider>();
                    if (tsColliders != null) {
                        for (int index2 = 0, length2 = tsColliders.Length; index2 < length2; index2++) {
                            FPCollider tsCollider = tsColliders[index2];

                            if (!tsCollider.Body.TSDisabled) {
                                DestroyFPRigidBody(tsCollider.gameObject, tsCollider.Body);
                            }
                        }
                    }

                    FPCollider2D[] tsCollider2Ds = ((FrameSyncBehaviour)tsmb.FrameSyncBehavior).gameObject.GetComponentsInChildren<FPCollider2D>();
                    if (tsCollider2Ds != null) {
                        for (int index2 = 0, length2 = tsCollider2Ds.Length; index2 < length2; index2++) {
                            FPCollider2D tsCollider2D = tsCollider2Ds[index2];

                            if (!tsCollider2D.Body.TSDisabled) {
                                DestroyFPRigidBody(tsCollider2D.gameObject, tsCollider2D.Body);
                            }
                        }
                    }
                }
            }
        }

        private FP tsDeltaTime = 0;

        void FixedUpdate() {
            if (lockstep != null) {
                tsDeltaTime += UnityEngine.Time.deltaTime;

                if (tsDeltaTime >= (lockedTimeStep - JitterTimeFactor)) {
                    tsDeltaTime = 0;

                    instance.scheduler.UpdateAllCoroutines();
                    lockstep.Update();
                }
            }
        }

        InputDataBase ProvideInputData() {
            return new InputData();
        }

        void GetLocalData(InputDataBase playerInputData) {
            FrameSyncInput.CurrentInputData = (InputData) playerInputData;

            if (behaviorsByPlayer.ContainsKey(playerInputData.ownerID)) {
                List<FrameSyncManagedBehaviour> managedBehavioursByPlayer = behaviorsByPlayer[playerInputData.ownerID];
                for (int index = 0, length = managedBehavioursByPlayer.Count; index < length; index++) {
                    FrameSyncManagedBehaviour bh = managedBehavioursByPlayer[index];

                    if (bh != null && !bh.disabled) {
                        bh.OnSyncedInput();
                    }
                }
            }

            FrameSyncInput.CurrentInputData = null;
        }

        void OnStepUpdate(List<InputDataBase> allInputData) {
            time += lockedTimeStep;

            if (ReplayRecord.replayMode != ReplayMode.LOAD_REPLAY) {
                CheckGameObjectsSafeMap();
            }

            FrameSyncInput.SetAllInputs(null);

            for (int index = 0, length = generalBehaviours.Count; index < length; index++) {
                FrameSyncManagedBehaviour bh = generalBehaviours[index];

                if (bh != null && !bh.disabled) {
                    bh.OnPreSyncedUpdate();
                    instance.scheduler.UpdateAllCoroutines();
                }
            }

            for (int index = 0, length = allInputData.Count; index < length; index++) {
                InputDataBase playerInputData = allInputData[index];

                if (behaviorsByPlayer.ContainsKey(playerInputData.ownerID)) {
                    List<FrameSyncManagedBehaviour> managedBehavioursByPlayer = behaviorsByPlayer[playerInputData.ownerID];
                    for (int index2 = 0, length2 = managedBehavioursByPlayer.Count; index2 < length2; index2++) {
                        FrameSyncManagedBehaviour bh = managedBehavioursByPlayer[index2];

                        if (bh != null && !bh.disabled) {
                            bh.OnPreSyncedUpdate();
                            instance.scheduler.UpdateAllCoroutines();
                        }
                    }
                }
            }

            FrameSyncInput.SetAllInputs(allInputData);

            FrameSyncInput.CurrentSimulationData = null;
            for (int index = 0, length = generalBehaviours.Count; index < length; index++) {
                FrameSyncManagedBehaviour bh = generalBehaviours[index];

                if (bh != null && !bh.disabled) {
                    bh.OnSyncedUpdate();
                    instance.scheduler.UpdateAllCoroutines();
                }
            }

            for (int index = 0, length = allInputData.Count; index < length; index++) {
                InputDataBase playerInputData = allInputData[index];

                if (behaviorsByPlayer.ContainsKey(playerInputData.ownerID)) {
                    FrameSyncInput.CurrentSimulationData = (InputData) playerInputData;

                    List<FrameSyncManagedBehaviour> managedBehavioursByPlayer = behaviorsByPlayer[playerInputData.ownerID];
                    for (int index2 = 0, length2 = managedBehavioursByPlayer.Count; index2 < length2; index2++) {
                        FrameSyncManagedBehaviour bh = managedBehavioursByPlayer[index2];

                        if (bh != null && !bh.disabled) {
                            bh.OnSyncedUpdate();
                            instance.scheduler.UpdateAllCoroutines();
                        }
                    }
                }

                FrameSyncInput.CurrentSimulationData = null;
            }

            CheckQueuedBehaviours();
        }

        private void OnRemovedRigidBody(IBody body) {
            GameObject go = PhysicsManager.instance.GetGameObject(body);

            if (go != null) {
                List<FrameSyncBehaviour> behavioursToRemove = new List<FrameSyncBehaviour>(go.GetComponentsInChildren<FrameSyncBehaviour>());
                RemoveFromTSMBList(queuedBehaviours, behavioursToRemove);
                RemoveFromTSMBList(generalBehaviours, behavioursToRemove);

                var behaviorsByPlayerEnum = behaviorsByPlayer.GetEnumerator();
                while (behaviorsByPlayerEnum.MoveNext()) {
                    List<FrameSyncManagedBehaviour> listBh = behaviorsByPlayerEnum.Current.Value;
                    RemoveFromTSMBList(listBh, behavioursToRemove);
                }
            }
        }

        private void RemoveFromTSMBList(List<FrameSyncManagedBehaviour> tsmbList, List<FrameSyncBehaviour> behaviours) {
            List<FrameSyncManagedBehaviour> toRemove = new List<FrameSyncManagedBehaviour>();
            for (int index = 0, length = tsmbList.Count; index < length; index++) {
                FrameSyncManagedBehaviour tsmb = tsmbList[index];

                if ((tsmb.FrameSyncBehavior is FrameSyncBehaviour) && behaviours.Contains((FrameSyncBehaviour)tsmb.FrameSyncBehavior)) {
                    toRemove.Add(tsmb);
                }
            }

            for (int index = 0, length = toRemove.Count; index < length; index++) {
                FrameSyncManagedBehaviour tsmb = toRemove[index];
                tsmbList.Remove(tsmb);
            }
        }

        /** 
         * @brief Clean up references to be collected by gc.
         **/
        public static void CleanUp() {
            ResourcePool.CleanUpAll();
            StateTracker.CleanUp();
            instance = null;
        }

        void OnPlayerDisconnection(byte playerId) {
            FrameSyncManagedBehaviour.OnPlayerDisconnection(generalBehaviours, behaviorsByPlayer, playerId);
        }

        void OnGameStarted() {
            FrameSyncManagedBehaviour.OnGameStarted(generalBehaviours, behaviorsByPlayer);
            instance.scheduler.UpdateAllCoroutines();

            CheckQueuedBehaviours();
        }

        void OnGamePaused() {
            FrameSyncManagedBehaviour.OnGamePaused(generalBehaviours, behaviorsByPlayer);
            instance.scheduler.UpdateAllCoroutines();
        }

        void OnGameUnPaused() {
            FrameSyncManagedBehaviour.OnGameUnPaused(generalBehaviours, behaviorsByPlayer);
            instance.scheduler.UpdateAllCoroutines();
        }

        void OnGameEnded() {
            FrameSyncManagedBehaviour.OnGameEnded(generalBehaviours, behaviorsByPlayer);
            instance.scheduler.UpdateAllCoroutines();
        }

        void OnApplicationQuit() {
            EndSimulation();
        }

    }

}