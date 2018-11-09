using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour {

    public Actions actions;
    public PlayerController controller;

    public int RollIndex = 0;
    public List<string> weapon_list = new List<string>();

    public string state = "Stay";
    // Use this for initialization

    void Start()
    {
        actions = GetComponent<Actions>();
        controller = GetComponent<PlayerController>();

        foreach (PlayerController.Arsenal a in controller.arsenal)
        {
            weapon_list.Add(a.name);
        }
    }

    void StateChange(string s)
    {
 //       if(state != s)
        {
            state = s;
            actions.SendMessage(state, SendMessageOptions.DontRequireReceiver);
        }
    }

    void ChangeWeapon()
    {
        RollIndex = (++RollIndex) % weapon_list.Count;
        string name = weapon_list[RollIndex];
        controller.SetArsenal(name);
    }

    // Update is called once per frame
    void Update ()
    {
		if(Input.GetKey(KeyCode.W)&& Input.GetKey(KeyCode.LeftShift))
        {
            StateChange("Run");        
        }
        else if(Input.GetKey(KeyCode.W))
        {
            StateChange("Walk");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StateChange("Jump");
        }
        else if (Input.GetMouseButton(0))
        {
            StateChange("Attack");
        }
        else if (Input.GetMouseButton(1))
        {
            StateChange("Aiming");
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            StateChange("Sitting");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            StateChange("Death");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            StateChange("Damage");
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeWeapon();
        }
        else
        {
            //StateChange("Stay");
        }
    }
}
