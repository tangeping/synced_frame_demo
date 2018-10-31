using System.Collections;
using System.Collections.Generic;
using TrueSync;
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
                GameObject box = TestManager.SyncedInstantiate(this.boxPrefab, TSVector.zero, TSQuaternion.identity);
                TSRigidBody body = box.GetComponent<TSRigidBody>();
                body.position = new TrueSync.TSVector(i * 2 - 5, 1, j * 2);
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
