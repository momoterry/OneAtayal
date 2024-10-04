using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_RoomDoorTrigger : MR_Node
{
    override public void OnSetupByRoom(MazeGameManagerBase.RoomInfo room)
    {
        print("MR_RoomDoorTrigger.OnSetupByRoom");
    }

    public void OnTG(GameObject whoTG)
    {
        print("MR_RoomDoorTrigger.OnTG");
    }

}
