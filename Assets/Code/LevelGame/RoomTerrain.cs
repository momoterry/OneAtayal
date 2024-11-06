using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTerrain : RoomGameplayBase
{
    public override void BuildLayout(MazeGameManagerBase.RoomInfo room, OneMap oMap)
    {
        base.BuildLayout(room, oMap);

        print("Room: " + room.width + ", " + room.height);
        print("Rect: " + room.mapRect);

        oMap.FillValue(room.mapRect.x, room.mapRect.y, room.mapRect.width, room.mapRect.height, (int)MG_MazeOneBase.MAP_TYPE.GROUND);
    }
}
