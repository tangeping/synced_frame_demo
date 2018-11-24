using SyncFrame;
using System.Collections;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

public class BoxSyncManager : MonoBehaviour {

    /**
* @brief Box's prefab.
**/
    public GameObject boxPrefab;


    /**
* @brief Number of boxes to be place in X axis.
**/
    public int numberOfBoxesX;

    /**
    * @brief Number of boxes to be place in Z axis.
    **/
    public int numberOfBoxesZ;

    /**
    * @brief Initial setup when game is started.
    **/

    void CreateBoxes()
    {
        for (int i = 0; i < numberOfBoxesX; i++)
        {
            for (int j = 0; j < numberOfBoxesZ; j++)
            {
                GameObject box = FPS_Manager.SyncedInstantiate(this.boxPrefab, TSVector.zero, TSQuaternion.identity);
                FPRigidBody body = box.GetComponent<FPRigidBody>();
                //body.position = new KBEngine.TSVector(i * 2 - 5, 1, j * 2);
                body.position = new KBEngine.TSVector(-i * 0.4f + 1, 0.6f, -2-j * 0.4f);
            }
        }
    }

    // Use this for initialization
    void Start () {
        CreateBoxes();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
