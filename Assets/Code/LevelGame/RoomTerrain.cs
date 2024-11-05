using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTerrain : RoomGameplayBase
{
    public override void BuildLayout(MazeGameManagerBase.RoomInfo room, OneMap oMap, RectInt roomRect)
    {
        base.BuildLayout(room, oMap, roomRect);

        print("Room: " + room.cell.x + ", " + room.cell.y);
        print("Rect: " + roomRect);
    }
}
