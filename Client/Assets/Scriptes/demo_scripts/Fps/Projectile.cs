using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using KBEngine;
using SyncFrame;


public class Projectile : FrameSyncBehaviour
{

    //public KBEngine.Avatar owner;

    private FP speed = 5;

    private FP destroyTime = 3;

    public TSVector direction = TSVector.zero;

    private bool OnceForce = true;
    /**
     *  @brief Returns the {@link FPTransform} attached.
     */



    public override void OnSyncedUpdate()
    {
        if (destroyTime <= 0)
        {
            //FPS_Manager.SyncedDestroy(this.gameObject);
        }
        if(OnceForce)
        {
            OnceForce = false;
            FPRigidBody.AddForce(direction * speed, ForceMode.Impulse);
        }
        
        destroyTime -= FPS_Manager.instance.Config.lockedTimeStep;

        //Debug.Log("Projectile:OnSyncedUpdate");
    }

    public void OnSyncedTriggerEnter(FPCollision other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerBehaviour hitPlayer = other.gameObject.GetComponent<PlayerBehaviour>();
            //if (hitPlayer.owner.id != owner.id)
            {
                //FPS_Manager.SyncedDestroy(this.gameObject);
                //hitPlayer.Respawn();
            }
        }
    }
}
