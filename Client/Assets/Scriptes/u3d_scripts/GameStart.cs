using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Application.runInBackground = true;

        // 打开登录面板
        PanelMgr.instance.OpenPanel<LoginPanel>("");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
