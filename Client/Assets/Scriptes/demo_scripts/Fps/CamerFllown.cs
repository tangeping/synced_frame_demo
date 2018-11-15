using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
public class CamerFllown : MonoBehaviour {


    private Transform target = null;
    // Use this for initialization
    private Vector3 offset = Vector3.zero;


	
    public void AttachTarget(Transform t)
    {
        target = t;

        offset = target.position - transform.position;
    }
	// Update is called once per frame
	void Update () {
		
        if(target != null)
        {
//             transform.position = Vector3.Lerp(transform.position, target.position - offset, Time.deltaTime * 5);
//             Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
//             transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3f);
        }
	}
}
