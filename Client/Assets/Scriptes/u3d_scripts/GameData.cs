using KBEngine;
using SyncFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : CBSingleton<GameData> {

    public KBEngine.Avatar localPlayer = null;
    public string AccountName;
    public SPACE_INFO CurrentRoom;
    public List<KBEngine.Avatar> RoomPlayers = new List<KBEngine.Avatar>();
    public Dictionary<Int32, bool> PlayerReady = new Dictionary<int, bool>();

    public Queue<FRAME_DATA> frameList = new Queue<FRAME_DATA>();
    public Int32 RoundTripTime = 0;

    public bool IsCreater()
    {
        if(CurrentRoom != null && CurrentRoom.space_creater == localPlayer.id)
        {
            return true;
        }

        return false;
    }
}
