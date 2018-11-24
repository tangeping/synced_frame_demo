using UnityEngine;
using KBEngine;
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
    * @brief Key to set/get horizontal position from {@link FrameSyncInput}.
    **/
    private const byte INPUT_KEY_HORIZONTAL = 0;

    /**
    * @brief Key to set/get vertical position from {@link FrameSyncInput}.
    **/
    private const byte INPUT_KEY_VERTICAL = 1;

    /**
    * @brief Key to set/get jump state from {@link FrameSyncInput}.
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

    public FPRigidBody FPRigidBody;

    public FPTransform FPTransform;

    public KBEngine.Avatar owner;

    public Animator anim;

    public UInt32 FrameCount = 0;

    public TSVector force = new TSVector();
    /**
    * @brief Initial setup when game is started.
    **/
    public  void Start()
    {
        
        //CBGlobalEventDispatcher.Instance.AddEventListener((int)EVENT_ID.EVENT_FRAME_TICK, OnSyncedUpdate);
        //KBEngine.Event.registerOut("recieveFrameTick", this, "OnSyncedUpdate");


        FPRigidBody = GetComponent<FPRigidBody>();

        FPTransform = GetComponent<FPTransform>();

        anim = GetComponent<Animator>();

        Debug.Log("PlayerContorl::start:" + FPRigidBody + ",FPTransform:" + FPTransform + ",anim:"+anim);

        // if is first player then changes ball's color to black
    }
    private void OnGUI()
    {

        if (owner.isPlayer())
        {
            GUI.contentColor = Color.green;
            GUI.Label(new Rect(0, Screen.height - 20, 400, 100), "id: " + owner.id);
            GUI.Label(new Rect(0, Screen.height - 35, 400, 100), "frameCount: " + FrameCount);
            GUI.Label(new Rect(0, Screen.height - 50, 400, 100), "position: " + FPRigidBody.position);
            GUI.Label(new Rect(0, Screen.height - 65, 400, 100), "velocity: " + FPRigidBody.velocity);
            GUI.Label(new Rect(0, Screen.height - 80, 400, 100), "angularVelocity: " + FPRigidBody.angularVelocity);
            GUI.Label(new Rect(0, Screen.height - 95, 400, 100), "force: " + force);
        }
        else
        {
            GUI.contentColor = Color.yellow;
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 20, 400, 100), "id: " + owner.id);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 400, 100), "frameCount: " + FrameCount);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 50, 400, 100), "position: " + FPRigidBody.position);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 65, 400, 100), "velocity: " + FPRigidBody.velocity);
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 80, 400, 100), "angularVelocity: " + FPRigidBody.angularVelocity);
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

    void simulation_Ball()
    {
        if (FrameCount < 1000)
        {
            FPRigidBody.AddForce(new TSVector(0.0, 0.0, -2.2), ForceMode.Impulse);
        }
        else if (FrameCount < 2000)
        {
            FPRigidBody.AddForce(new TSVector(0.0, 0.0, 2.2), ForceMode.Impulse);
        }
        else if (FrameCount < 2400)
        {
            FPRigidBody.AddForce(new TSVector(2.2, 0.0, 0.0), ForceMode.Impulse);
        }
        else if (FrameCount < 2800)
        {
            FPRigidBody.AddForce(new TSVector(-2.2, 0.0, 0.0), ForceMode.Impulse);
        }
        else if (FrameCount < 3000)
        {
            FPRigidBody.AddForce(new TSVector(-2.2, 0.0, -2.2), ForceMode.Impulse);
        }
        else
        {
            FPRigidBody.AddForce(new TSVector(0.0, 0.0, 0.0), ForceMode.Impulse);
        }
    }

    public void SimulationAnimator()
    {
        float h = 0.0f, v = 0.0f;

        if (FrameCount < 500)
        {
            h = -1.0f;
        }
        else if (FrameCount < 1000)
        {
            h = 1.0f;
        }
        else if (FrameCount < 1500)
        {
            v = -1.0f;
        }
        else if (FrameCount < 200)
        {
            v = 1.0f;
        }
        else 
        {
            h = 0.0f;
            v = 0.0f;
        }

        if (anim)
        {
            anim.SetFloat("MyBlend", h);
            anim.SetFloat("YouBlend", v);
            //Debug.Log("h:" + h + ",v:" + v);
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
 //       anim.Update(Time.deltaTime);

        if(anim.speed != 1)
        {
            anim.speed = 1;
        }

        SimulationAnimator();

        string Snapshot = "id: " + owner.id
            + ",frameCount: " + FrameCount
//             + ",mass: " + FPRigidBody.mass
//             + ",useGravity: " + FPRigidBody.useGravity
//             + ",isKinematic: " + FPRigidBody.isKinematic
//             + ",drag: " + FPRigidBody.drag
//             + ",angularDrag: " + FPRigidBody.angularDrag
            + ",position: " + FPRigidBody.position
            + ",rotation: " + FPRigidBody.rotation
            + ",velocity: " + FPRigidBody.velocity
            + ",angularVelocity: " + FPRigidBody.angularVelocity
            + ",LinearVelocity:" + ((KBEngine.Physics3D.RigidBody)FPRigidBody.tsCollider.Body).LinearVelocity
            + ",IsParticle:" + ((KBEngine.Physics3D.RigidBody)FPRigidBody.tsCollider.Body).IsParticle
            + ",force: " + ((KBEngine.Physics3D.RigidBody)FPRigidBody.tsCollider.Body).Force;
        

        //CBFrame.Utils.Logger.Debug(owner.id.ToString(),Snapshot);

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
        /*        FPRigidBody.AddForce(movement, ForceMode.Impulse);*/

    }

    public void OnSyncedGamePause()
    {
        if(anim.speed > 0)
        {
            anim.speed = 0;
            
        }

        Debug.Log("anim.speed:" + anim.speed);
    }
    /**
    * @brief Tints box's material with gray color when it collides with the ball.
    **/
    public void OnSyncedCollisionEnter(FPCollision other)
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
    public void OnSyncedCollisionStay(FPCollision other)
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
    public void OnSyncedCollisionExit(FPCollision other)
    {
        
        if (other.gameObject.name == "Box(Clone)")
        {
            Debug.Log("OnSyncedCollisionExit:other:" + other.transform.name);
            other.gameObject.transform.localScale = Vector3.one;
            other.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

}