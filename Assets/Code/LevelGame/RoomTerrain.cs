using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTerrain : RoomGameplayBase
{
    public Rect[] blockRects;
    public override void BuildLayout(MazeGameManagerBase.RoomInfo room, OneMap oMap)
    {
        base.BuildLayout(room, oMap);

        //print("Room: " + room.width + ", " + room.height);
        //print("Rect: " + room.mapRect);

        int roomX1 = (room.mapRect.width - (int)room.width) / 2 + room.mapRect.x;
        int roomY1 = (room.mapRect.height - (int)room.height) / 2 + room.mapRect.y;

        //print("X1 Y1: " + roomX1 + ", " + roomY1);

        for (int i=0; i<blockRects.Length; i++)
        {
            int x = roomX1 + Mathf.RoundToInt((blockRects[i].x + 5.0f) * 0.1f * room.width);
            int y = roomY1 + Mathf.RoundToInt((blockRects[i].y + 5.0f) * 0.1f * room.height);
            int w = Mathf.RoundToInt(blockRects[i].width * 0.1f * room.width);
            int h = Mathf.RoundToInt(blockRects[i].height * 0.1f * room.height);
            //print("To Block :" + new RectInt(x, y, w, h));
            oMap.FillValue(x, y, w, h, (int)MG_MazeOneBase.MAP_TYPE.BLOCK);
        }

        //oMap.FillValue(room.mapRect.x, room.mapRect.y, room.mapRect.width, room.mapRect.height, (int)MG_MazeOneBase.MAP_TYPE.GROUND);
    }
}