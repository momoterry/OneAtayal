using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectPlacement : RoomGameplayBase
{
    public GameObject objRef;
    public float placePercent = 10.0f;

    public override void Build(MazeGameManager.RoomInfo room)
    {
        base.Build(room);

        float cWidth = room.width;
        float cHeight = room.height;
        float pathLengthX = room.wallWidth;
        float pathLengthY = room.wallHeight;
        if (room.cell.isPath)
        {
            cWidth = room.doorWidth;
            cHeight = room.doorHeight;
            pathLengthX += (room.width - room.doorWidth) / 2;
            pathLengthY += (room.height - room.doorHeight) / 2;
        }
        float xShift = (cWidth + pathLengthX) / 2;
        float yShift = (cHeight + pathLengthY) / 2;

        PlaceArea(room.vCenter, cWidth, cHeight);
        if (room.cell.R)
        {
            PlaceArea(room.vCenter + new Vector3(xShift, 0, 0), pathLengthX, room.doorHeight);
        }
        if (room.cell.L)
        {
            PlaceArea(room.vCenter + new Vector3(-xShift, 0, 0), pathLengthX, room.doorHeight);
        }
        if (room.cell.U)
        {
            PlaceArea(room.vCenter + new Vector3(0, 0, yShift), room.doorWidth, pathLengthY);
        }
        if (room.cell.D)
        {
            PlaceArea(room.vCenter + new Vector3(0, 0, -yShift), room.doorWidth, pathLengthY);
        }
    }


    protected void PlaceArea(Vector3 vCenter, float width, float height)
    {
        for (int i=0;i< (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
                if (Random.Range(0.0f, 100.0f) < placePercent)
                {
                    Vector3 vGridCenter = new Vector3(-width / 2 + 0.5f + (i * 1.0f), 0, -height / 2 + 0.5f + (j * 1.0f));
                    BattleSystem.SpawnGameObj(objRef, vGridCenter + vCenter);
                }
            }
        }
    }

}
