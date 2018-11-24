using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

public class TestControl : MonoBehaviour {

    // Use this for initialization
    public FPRigidBody FPRigidBody;

    public int owerId = 0;

    public int FrameCount = 0;
    void Start()
    {
        FPRigidBody = GetComponent<FPRigidBody>();
        //FPRigidBody.position = new KBEngine.TSVector(-2, 1, 16);
        //StartCoroutine(UpdateCorutine());
    }

    void simulation()
    {
        if (FrameCount > 3900)
        {
            FPRigidBody.AddForce(new TSVector(0.0, 0.0, -1.2), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 3800)
        {
            FPRigidBody.AddForce(new TSVector(0.0, 0.0, 1.2), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 2800)
        {
            FPRigidBody.AddForce(new TSVector(1.2, 0.0, 0.0), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 2400)
        {
            FPRigidBody.AddForce(new TSVector(-1.2, 0.0, 0.0), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 1400)
        {
            FPRigidBody.AddForce(new TSVector(-1.2, 0.0, -1.2), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
        else if (FrameCount > 0)
        {
            FPRigidBody.AddForce(new TSVector(0.0, 0.0, 0.0), ForceMode.Impulse);
            PhysicsManager.instance.UpdateStep();
            FrameCount--;
        }
    }

    public void simulation1()
    {
        FrameCount++;

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

        string Snapshot = "id: " + owerId
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
