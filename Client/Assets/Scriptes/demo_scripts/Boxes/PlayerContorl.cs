using UnityEngine;
using TrueSync;
using SyncFrame;
using CBFrame.Sys;
using KBEngine;
using CBFrame.Core;
using System;

/**
* @brief Manages player's ball behavior.        
**/
public class PlayerContorl : MonoBehaviour
{

    /**
    * @brief Key to set/get horizontal position from {@link TrueSyncInput}.
    **/
    private const byte INPUT_KEY_HORIZONTAL = 0;

    /**
    * @brief Key to set/get vertical position from {@link TrueSyncInput}.
    **/
    private const byte INPUT_KEY_VERTICAL = 1;

    /**
    * @brief Key to set/get jump state from {@link TrueSyncInput}.
    **/
    private const byte INPUT_KEY_CREATE = 2;

    /**
    * @brief It is true if the ball is not dynamically instantiated.
    **/
    public bool createdRuntime;

    /**
    * @brief Ball's prefab.
    **/
    public GameObject prefab;

    /**
    * @brief Represents the last jump state.
    **/
    [AddTracking]
    private bool lastCreateState = false;

    public TSRigidBody tsRigidBody;

    public TSTransform tsTransform;

    public KBEngine.Avatar owner;

    public UInt32 FrameCount = 0;

    public TSVector force = new TSVector();
    /**
    * @brief Initial setup when game is started.
    **/
    public  void Start()
    {
        
        //CBGlobalEventDispatcher.Instance.AddEventListener((int)EVENT_ID.EVENT_FRAME_TICK, OnSyncedUpdate);
        //KBEngine.Event.registerOut("recieveFrameTick", this, "OnSyncedUpdate");


        tsRigidBody = GetComponent<TSRigidBody>();

        tsTransform = GetComponent<TSTransform>();

  
        Debug.Log("PlayerContorl::start:" + tsRigidBody + ",tsTransform:" + tsTransform);

        // if is first player then changes ball's color to black
    }
    private void OnGUI()
    {

        if (owner.isPlayer())
        {
            GUI.contentColor = Color.green;
            GUI.Label(new Rect(0, Screen.height - 20, 400, 100), "id: " + owner.id);
            GUI.Label(new Rect(0, Screen.height - 35, 400, 100), "frameCount: " + FrameCount);
            GUI.Label(new Rect(0, Screen.height - 50, 400, 100), "position: " + tsRigidBody.position);
            GUI.Label(new Rect(0, Screen.height - 65, 400, 100), "velocity: " + tsRigidBody.velocity);
            GUI.Label(new Rect(0, Screen.height - 80, 400, 100), "angularVelocity: " + tsRigidBody.angularVelocity);
            GUI.Label(new Rect(0, Screen.height - 95, 400, 100), "force: " + force);
        }
        else
        {
            GUI.contentColor = Color.yellow;
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 20, 400, 100), "id: " + owner.id);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 400, 100), "frameCount: " + FrameCount);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 50, 400, 100), "position: " + tsRigidBody.position);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 65, 400, 100), "velocity: " + tsRigidBody.velocity);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 80, 400, 100), "angularVelocity: " + tsRigidBody.angularVelocity);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 95, 400, 100), "force: " + force);
        }

    }
    private void FixedUpdate()
    {
//        OnSyncedInput();
    }
    /**
    * @brief Sets player inputs.
    **/
    public  void OnSyncedInput()
    {
        if(!owner.isPlayer())
        {
            return;
        }

        FP hor = Input.GetAxis("Horizontal");
        FP ver = Input.GetAxis("Vertical");
        bool space = Input.GetKey(KeyCode.Space);

        KBEngine.Event.fireIn("reqFrameChange",
            FrameProto.encode(
                new FrameBox(CMD.BOX,new TSVector(hor,0 , ver), space)
                ));

    }

    void simulation()
    {
        if (FrameCount < 1000)
        {
            tsRigidBody.AddForce(new TSVector(0.0, 0.0, -2.2), ForceMode.Impulse);
        }
        else if (FrameCount < 2000)
        {
            tsRigidBody.AddForce(new TSVector(0.0, 0.0, 2.2), ForceMode.Impulse);
        }
        else if (FrameCount < 2400)
        {
            tsRigidBody.AddForce(new TSVector(2.2, 0.0, 0.0), ForceMode.Impulse);
        }
        else if (FrameCount < 2800)
        {
            tsRigidBody.AddForce(new TSVector(-2.2, 0.0, 0.0), ForceMode.Impulse);
        }
        else if (FrameCount < 3000)
        {
            tsRigidBody.AddForce(new TSVector(-2.2, 0.0, -2.2), ForceMode.Impulse);
        }
        else
        {
            tsRigidBody.AddForce(new TSVector(0.0, 0.0, 0.0), ForceMode.Impulse);
        }
    }

    /**
    * @brief Updates ball's movements and instantiates new ball objects when player press space.
    **/
    public  void OnSyncedUpdate(/*FRAME_DATA frameMsg*/)
    {
//        OnSyncedInput();

        //FrameCount = frameMsg.frameid;
        FrameCount++;

        simulation();

        string Snapshot = "id: " + owner.id
            + ",frameCount: " + FrameCount
//             + ",mass: " + tsRigidBody.mass
//             + ",useGravity: " + tsRigidBody.useGravity
//             + ",isKinematic: " + tsRigidBody.isKinematic
//             + ",drag: " + tsRigidBody.drag
//             + ",angularDrag: " + tsRigidBody.angularDrag
            + ",position: " + tsRigidBody.position
            + ",rotation: " + tsRigidBody.rotation
            + ",velocity: " + tsRigidBody.velocity
            + ",angularVelocity: " + tsRigidBody.angularVelocity
            + ",LinearVelocity:" + ((TrueSync.Physics3D.RigidBody)tsRigidBody.tsCollider.Body).LinearVelocity
            + ",IsParticle:" + ((TrueSync.Physics3D.RigidBody)tsRigidBody.tsCollider.Body).IsParticle
            + ",force: " + ((TrueSync.Physics3D.RigidBody)tsRigidBody.tsCollider.Body).Force;
        

        CBFrame.Utils.Logger.Debug(owner.id.ToString(),Snapshot);

        //         TSVector movement = TSVector.zero;
        //         bool space = false;
        //         bool emptyFrame = true;
        // 
        //         for (int i = 0; i < frameMsg.operation.Count; i++)
        //         {
        //             var item = frameMsg.operation[i];
        //             //           Debug.Log("operation id:" + oper.entityid +"entity:" + entity);
        // 
        //             if (item.entityid != owner.id || item.cmd_type != (UInt32)CMD.BOX)
        //             {
        //                 continue;
        //             }
        // 
        //             FrameBox msg = FrameProto.decode(item) as FrameBox;
        //             movement = msg.movement;
        //             space = msg.space;
        //             emptyFrame = false;
        //         }
        //         if(emptyFrame)
        //         {
        //             return;
        //         }
        // 
        //         force += movement;
        /*        tsRigidBody.AddForce(movement, ForceMode.Impulse);*/

    }

    /**
    * @brief Tints box's material with gray color when it collides with the ball.
    **/
    public void OnSyncedCollisionEnter(TSCollision other)
    {
        
        if (other.gameObject.name == "Box(Clone)")
        {
            Debug.Log("OnSyncedCollisionEnter:other:" + other.transform.name);
            other.gameObject.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    /**
    * @brief Increases box's local scale by 1% while collision with a ball remains active.
    **/
    public void OnSyncedCollisionStay(TSCollision other)
    {
        
        if (other.gameObject.name == "Box(Clone)")
        {
            Debug.Log("OnSyncedCollisionStay:other:" + other.transform.name);
            other.gameObject.transform.localScale *= 1.01f;
        }
    }

    /**
    * @brief Resets changes in box's properties when there is no more collision with the ball.
    **/
    public void OnSyncedCollisionExit(TSCollision other)
    {
        
        if (other.gameObject.name == "Box(Clone)")
        {
            Debug.Log("OnSyncedCollisionExit:other:" + other.transform.name);
            other.gameObject.transform.localScale = Vector3.one;
            other.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

}