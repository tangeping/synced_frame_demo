namespace KBEngine
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Operation : OperationBase
    {
        public Operation() : base()
        {

        }

        public override void onAttached(Entity ownerEntity)
        {
            if (ownerEntity.isPlayer())
            {
                KBEngine.Event.registerIn("reqSpaceList", this, "reqSpaceList");
                KBEngine.Event.registerIn("reqCreateSpace", this, "reqCreateSpace");
                KBEngine.Event.registerIn("reqRemoveSpace", this, "reqRemoveSpace");
                KBEngine.Event.registerIn("reqLoginSpace", this, "reqLoginSpace");
                KBEngine.Event.registerIn("reqLoginOutSpace", this, "reqLoginOutSpace");
                KBEngine.Event.registerIn("reqReady", this, "reqReady");
                KBEngine.Event.registerIn("reqGamePause", this, "reqGamePause");
                KBEngine.Event.registerIn("reqGameRunning", this, "reqGameRunning");
            }
        }

        public override void onStateChanged(SByte oldValue)
        {
            KBEngine.Event.fireOut("onReadyState", new object[] { state });
        }

        public override void onLossScoreChanged(Int32 oldValue)
        {

        }

        public override void onWinScoreChanged(Int32 oldValue)
        {

        }

        public override void onIsWathcherChanged(SByte oldValue)
        {

        }

        public override void onCreateSpaceResult(ulong spaceKey)
        {
            KBEngine.Event.fireOut("onCreateSpaceResult", new object[] { spaceKey });
        }

        public override void onReqSpaceList(SPACE_LIST spaceList)
        {
            KBEngine.Event.fireOut("onReqSpaceList", new object[] { spaceList });
        }

        public override void onLoginSpaceResult(ulong spaceKey)
        {
            KBEngine.Event.fireOut("onEnterSpaceResult", new object[] { spaceKey });
        }

        public void reqLoginOutSpace()
        {
            cellEntityCall.reqLoginOutSpace();
        }

        public void reqReady(sbyte ready)
        {
            Debug.Log("operation:ready:" + ready);
            cellEntityCall.reqReady(ready);
        }

        public void reqGamePause()
        {
            cellEntityCall.reqGamePause();
        }

        public void reqGameRunning()
        {
            cellEntityCall.reqGameRunning();
        }

        public void reqSpaceList()
        {
            baseEntityCall.reqSpaceList();
        }

        public void reqCreateSpace()
        {
            baseEntityCall.reqCreateSpace();
        }

        public void reqRemoveSpace(ulong spacekey)
        {
            baseEntityCall.reqRemoveSpace(spacekey);
        }

        public void reqLoginSpace(ulong spacekey)
        {
            baseEntityCall.reqLoginSpace(spacekey);
        }



        public override void onLoginOutSpaceResult(Int32 entityId, ulong spacekey)
        {
           KBEngine.Event.fireOut("onLoginOutSpaceResult",new object[] { entityId, spacekey });
        }

        public override void onGamePause(uint lastFrameid)
        {
            //KBEngine.Event.fireOut("onGamePause", new object[] { lastFrameid });
        }
    }
}
