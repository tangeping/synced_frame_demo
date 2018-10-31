using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMariter : MonoBehaviour {

    public Material bule,green,red,yellow;
    // Use this for initialization
    public Material myMaterial;
    private void Awake()
    {
//         bule = (Material)Resources.Load("Boxes/material/blue");
//         if (bule == null)
//         {
//             Debug.LogError("can not found Material  Boxes/material/blue !");
//         }
// 
//         green = (Material)Resources.Load("Boxes/material/green");
//         if (bule == null)
//         {
//             Debug.LogError("can not found Material  Boxes/material/green !");
//         }
// 
//         red = (Material)Resources.Load("Boxes/material/red");
//         if (bule == null)
//         {
//             Debug.LogError("can not found Material  Boxes/material/red !");
//         }
// 
//         yellow = (Material)Resources.Load("Boxes/material/yellow");
//         if (bule == null)
//         {
//             Debug.LogError("can not found Material  Boxes/material/yellow !");
//         }




    }
    void Start () {

        myMaterial = GetComponent<Renderer>().materials[0];

        if (myMaterial == null)
        {
            Debug.LogError("can not found myMaterial  !");
        }
    }
	
    void ReplaceMaterial(Material m)
    {
 //       if(m != GetComponent<Renderer>().materials[0])
        {
            GetComponent<Renderer>().materials[0] = m;
        }
    }
    // Update is called once per frame
    void Update () {
		
        if(Input.GetKeyDown(KeyCode.W))
        {
            ReplaceMaterial( bule);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            ReplaceMaterial(green);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ReplaceMaterial(red);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ReplaceMaterial(yellow);
        }
    }
}
