using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectPlacement : RoomGameplayBase
{
    [System.Serializable]
    public class ObjectInfo
    {
        public GameObject objRef;
        public float placePercent = 10.0f;
    }
    public ObjectInfo[] objs;

    //public override void Build(MazeGameManagerBase.RoomInfo room)
    //{
    //    base.Build(room);

    //    float cWidth = room.width;
    //    float cHeight = room.height;
    //    float pathLengthX = room.wallWidth;
    //    float pathLengthY = room.wallHeight;
    //    if (room.cell.isPath)
    //    {
    //        cWidth = room.doorWidth;
    //        cHeight = room.doorHeight;
    //        pathLengthX += (room.width - room.doorWidth) / 2;
    //        pathLengthY += (room.height - room.doorHeight) / 2;
    //    }
    //    float xShift = (cWidth + pathLengthX) / 2;
    //    float yShift = (cHeight + pathLengthY) / 2;

    //    PlaceArea(room.vCenter, cWidth, cHeight);
    //    if (room.cell.R)
    //    {
    //        PlaceArea(room.vCenter + new Vector3(xShift, 0, 0), pathLengthX, room.doorHeight);
    //    }
    //    if (room.cell.L)
    //    {
    //        PlaceArea(room.vCenter + new Vector3(-xShift, 0, 0), pathLengthX, room.doorHeight);
    //    }
    //    if (room.cell.U)
    //    {
    //        PlaceArea(room.vCenter + new Vector3(0, 0, yShift), room.doorWidth, pathLengthY);
    //    }
    //    if (room.cell.D)
    //    {
    //        PlaceArea(room.vCenter + new Vector3(0, 0, -yShift), room.doorWidth, pathLengthY);
    //    }
    //}

    public override void Build(MazeGameManagerBase.RoomInfo room)
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
        //float xShift = (cWidth + pathLengthX) / 2;
        //float yShift = (cHeight + pathLengthY) / 2;

        if (room.cell.R)
        { 
            //if (room.cell.isPath)
            {
                PlaceHoriEdge(room.vCenter, room.doorHeight / 2, cWidth / 2, cWidth / 2 + pathLengthX, false);
                PlaceHoriEdge(room.vCenter, -room.doorHeight / 2, cWidth / 2, cWidth / 2 + pathLengthX, true);
            }
        }
        else
        {
            PlaceVertiEdge(room.vCenter, cWidth / 2, -cHeight / 2, cHeight / 2, false);
        }
        if (room.cell.L)
        {
            //if (room.cell.isPath)
            {
                PlaceHoriEdge(room.vCenter, room.doorHeight / 2, -cWidth / 2 - pathLengthX, -cWidth / 2, false);
                PlaceHoriEdge(room.vCenter, -room.doorHeight / 2, -cWidth / 2 - pathLengthX, -cWidth / 2, true);
            }
        }
        else
        {
            PlaceVertiEdge(room.vCenter, -cWidth / 2, -cHeight / 2, cHeight / 2, true);
        }
        if (room.cell.U)
        {
            //if (room.cell.isPath)
            {
                PlaceVertiEdge(room.vCenter, room.doorWidth / 2, cHeight / 2, cHeight / 2 + pathLengthY, false);
                PlaceVertiEdge(room.vCenter, -room.doorWidth / 2, cHeight / 2, cHeight / 2 + pathLengthY, true);
            }
        }
        else
        {
            PlaceHoriEdge(room.vCenter, cHeight / 2, -cWidth / 2, cWidth / 2, false);
        }
        if (room.cell.D)
        {
            //if (room.cell.isPath)
            {
                PlaceVertiEdge(room.vCenter, room.doorWidth / 2, -cHeight / 2 - pathLengthY, -cHeight / 2, false);
                PlaceVertiEdge(room.vCenter, -room.doorWidth / 2, -cHeight / 2 - pathLengthY, -cHeight / 2, true);
            }
        }
        else
        {
            PlaceHoriEdge(room.vCenter, -cHeight / 2, -cWidth / 2, cWidth / 2, true);
        }

    }

    protected void PlaceVertiEdge(Vector3 vCenter, float x, float yMin, float yMax, bool leftEdge, float width = 2)
    {
        float xShift = leftEdge ? width / 2 : -width / 2;
        PlaceArea(vCenter + new Vector3(x + xShift, 0, (yMax + yMin)/2), width, yMax - yMin);
    }
    protected void PlaceHoriEdge(Vector3 vCenter, float y, float xMin, float xMax, bool donwEdge, float width = 2)
    {
        float yShift = donwEdge ? width / 2 : -width / 2;
        PlaceArea(vCenter + new Vector3((xMax + xMin) / 2, 0, y + yShift), xMax - xMin, width);
    }
    protected void PlaceArea(Vector3 vCenter, float width, float height)
    {
        for (int i=0;i< (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
                //if (Random.Range(0.0f, 100.0f) < placePercent)
                GameObject objRef = GetRandomGameObject();
                if (objRef)
                {
                    Vector3 vGridCenter = new Vector3(-width / 2 + 0.5f + (i * 1.0f), 0, -height / 2 + 0.5f + (j * 1.0f));
                    BattleSystem.SpawnGameObj(objRef, vGridCenter + vCenter);
                }
            }
        }
    }

    protected GameObject GetRandomGameObject()
    {
        float rd = Random.Range(0, 100.0f);
        float sum = 0;
        for (int i=0; i<objs.Length; i++)
        {
            sum += objs[i].placePercent;
            if (rd < sum)
                return objs[i].objRef;
        }
        return null;
    }

}
