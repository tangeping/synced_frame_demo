namespace KBEngine
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using SyncFrame;

    public class Avatar : AvatarBase
    {
        public Avatar()
        {
            
        }

        public override void __init__()
        {
            if (isPlayer())
            {
                GameData.Instance.localPlayer = this;
                Event.registerIn("reqFrameChange", this, "reqFrameChange");
                Event.fireOut("AvatarReady", new object[] { this});
            }
            
        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        public virtual void reqFrameChange(ENTITY_DATA operation)
        {
            operation.entityid = id;
            cellEntityCall.reqFrameChange(operation);
            //Debug.Log("Avatar::reqFrameChange:" + operation);
        }
        public override void onNameChanged(string old)
        {
            // Dbg.DEBUG_MSG(className + "::set_name: " + old + " => " + v); 
            Event.fireOut("set_name", new object[] { this, this.name });
        }

        public override void onLevelChanged(UInt16 old)
        {
            // Dbg.DEBUG_MSG(className + "::set_level: " + old + " => " + v); 
            Event.fireOut("set_level", new object[] { this, this.level });
        }



        public override void onRspFrameMessage(FRAME_DATA framedata)
        {
            //Event.fireOut("onRecieveFrame", new object[] { framedata });
            GameData.Instance.frameList.Enqueue(framedata);          
//            Debug.Log("--onRspFrameMessage tick : " + DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString() + ",frameid:"+ framedata.frameid);
        }

        public override void onGameBegine(ulong randSeed)
        {
            Event.fireOut("onGameBegine", new object[] { randSeed });
        }
    }
}
