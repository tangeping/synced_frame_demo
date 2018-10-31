using UnityEngine;
using System.Collections;
using KBEngine;
using System;
using SyncFrame;

public class ShowFPS : MonoBehaviour {

    public float f_UpdateInterval = 0.5F;

    private float f_LastInterval;

    private int i_Frames = 0;

    private float f_Fps;

    private int i_TimeDelay = 0;

    private float f_RTT;

    const double d_pi = 1/3.0;

    public System.DateTime startTime;

    void Start()
    {
		//Application.targetFrameRate=60;

        f_LastInterval = Time.realtimeSinceStartup;

        i_Frames = 0;

        KBEngine.Event.registerOut("onNetworkDelay", this, "onNetworkDelay");

    }



    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 40, 1, 200, 200), f_Fps.ToString("f2"));
        GUI.Label(new Rect(Screen.width - 100, 1, 200, 200), f_RTT.ToString("f2"));
    }

    public void onNetworkDelay(KBEngine.Entity entity, int arg1)
    {
        if(arg1 == i_TimeDelay)
        {
            f_RTT = (System.DateTime.Now - startTime).Milliseconds / 1.0f;
        }
    }




    void Update()
    {
        ++i_Frames;

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
        {
            f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

            i_Frames = 0;

            f_LastInterval = Time.realtimeSinceStartup;
        }
    }
}