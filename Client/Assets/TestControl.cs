using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TestControl : MonoBehaviour {

    // Use this for initialization
    public TSRigidBody tsRigidBody;

    public int owerId = 0;

    public int FrameCount = 0;
    void Start()
    {
        tsRigidBody = GetComponent<TSRigidBody>();
        //tsRigidBody.position = new TrueSync.TSVector(-2, 1, 16);
        //StartCoroutine(UpdateCorutine());
    }

    void simulation()
    {
        if (FrameCount > 3900)
        {
            tsRigidBody.AddForce(new TSVector(0.0, 0.0, -1.2), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 3800)
        {
            tsRigidBody.AddForce(new TSVector(0.0, 0.0, 1.2), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 2800)
        {
            tsRigidBody.AddForce(new TSVector(1.2, 0.0, 0.0), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 2400)
        {
            tsRigidBody.AddForce(new TSVector(-1.2, 0.0, 0.0), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 1400)
        {
            tsRigidBody.AddForce(new TSVector(-1.2, 0.0, -1.2), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 0)
        {
            tsRigidBody.AddForce(new TSVector(0.0, 0.0, 0.0), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
    }

    public void simulation1()
    {
        FrameCount++;

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

        string Snapshot = "id: " + owerId
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
        //CBFrame.Utils.Logger.Debug(owerId.ToString(), Snapshot);

    }




    IEnumerator UpdateCorutine()
    {      
        while(true)
        {
            if(FrameCount > 3900)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                simulation();
            }
            else if(FrameCount > 3000)
            {
                yield return new WaitForSecondsRealtime(0.08f);
                simulation();
            }
            else if (FrameCount > 2500)
            {
                yield return new WaitForSecondsRealtime(0.05f);
                simulation();
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.03f);
                simulation();
            }
            
        }
    }
}
