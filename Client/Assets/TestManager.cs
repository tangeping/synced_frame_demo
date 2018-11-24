using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

public class TestManager : MonoBehaviour
{

    public FrameSyncConfig Config;

    public GameObject playerPerfab;

    private void Awake()
    {
        PhysicsManager.New(Config);
        PhysicsManager.instance.LockedTimeStep = Config.lockedTimeStep;
        PhysicsManager.instance.Init();
    }
    // Use this for initialization
    void Start()
    {
        //GameObject me = SyncedInstantiate(playerPerfab, new TSVector(-2, 1, 16), new TSQuaternion(0, 0, 0, 1));
        //me.AddComponent<TestControl>().owerId = 3;

//         GameObject other = SyncedInstantiate(playerPerfab, new TSVector(6, 1.0, 16.0), new TSQuaternion(0, 0, 0, 1));
//         other.AddComponent<TestControl>().owerId = 7;

    }

    // Update is called once per frame
    void Update()
    {
        PhysicsManager.instance.UpdateStep();

        TestControl[] Controls = GameObject.FindObjectsOfType<TestControl>();
        
        for(int i = 0; i< Controls.Length;i++)
        {
            if (Controls[i].isActiveAndEnabled)
            {
                Controls[i].simulation1();
            }
        }
    }

    private void FixedUpdate()
    {

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
