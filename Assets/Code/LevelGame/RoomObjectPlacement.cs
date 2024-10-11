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
            PlaceHoriEdge(room.vCenter, room.doorHeight / 2, cWidth / 2, cWidth / 2 + pathLengthX, false);
            PlaceHoriEdge(room.vCenter, -room.doorHeight / 2, cWidth / 2, cWidth / 2 + pathLengthX, true);

            PlaceVertiEdge(room.vCenter, cWidth / 2, -cHeight / 2, - room.doorHeight / 2 / 2, false);
            PlaceVertiEdge(room.vCenter, cWidth / 2, cHeight / 2, room.doorHeight / 2 / 2, false);
        }
        else
        {
            PlaceVertiEdge(room.vCenter, cWidth / 2, -cHeight / 2, cHeight / 2, false);
        }
        if (room.cell.L)
        {
            PlaceHoriEdge(room.vCenter, room.doorHeight / 2, -cWidth / 2 - pathLengthX, -cWidth / 2, false);
            PlaceHoriEdge(room.vCenter, -room.doorHeight / 2, -cWidth / 2 - pathLengthX, -cWidth / 2, true);

            PlaceVertiEdge(room.vCenter, -cWidth / 2, -cHeight / 2, -room.doorHeight / 2 / 2, true);
            PlaceVertiEdge(room.vCenter, -cWidth / 2, cHeight / 2, room.doorHeight / 2 / 2, true);
        }
        else
        {
            PlaceVertiEdge(room.vCenter, -cWidth / 2, -cHeight / 2, cHeight / 2, true);
        }
        if (room.cell.U)
        {
            PlaceVertiEdge(room.vCenter, room.doorWidth / 2, cHeight / 2, cHeight / 2 + pathLengthY, false);
            PlaceVertiEdge(room.vCenter, -room.doorWidth / 2, cHeight / 2, cHeight / 2 + pathLengthY, true);

            PlaceHoriEdge(room.vCenter, cHeight / 2, -cWidth / 2, -room.doorWidth / 2, false);
            PlaceHoriEdge(room.vCenter, cHeight / 2, cWidth / 2, room.doorWidth / 2, false);
        }
        else
        {
            PlaceHoriEdge(room.vCenter, cHeight / 2, -cWidth / 2, cWidth / 2, false);
        }
        if (room.cell.D)
        {
            PlaceVertiEdge(room.vCenter, room.doorWidth / 2, -cHeight / 2 - pathLengthY, -cHeight / 2, false);
            PlaceVertiEdge(room.vCenter, -room.doorWidth / 2, -cHeight / 2 - pathLengthY, -cHeight / 2, true);

            PlaceHoriEdge(room.vCenter, -cHeight / 2, -cWidth / 2, -room.doorWidth / 2, true);
            PlaceHoriEdge(room.vCenter, -cHeight / 2, cWidth / 2, room.doorWidth / 2, true);
        }
        else
        {
            PlaceHoriEdge(room.vCenter, -cHeight / 2, -cWidth / 2, cWidth / 2, true);
        }

    }

    protected void PlaceVertiEdge(Vector3 vCenter, float x, float y1, float y2, bool leftEdge, float width = 2)
    {
        float xShift = leftEdge ? width / 2 : -width / 2;
        PlaceArea(vCenter + new Vector3(x + xShift, 0, (y1 + y2)/2), width, Mathf.Abs(y2 - y1));
    }
    protected void PlaceHoriEdge(Vector3 vCenter, float y, float x1, float x2, bool donwEdge, float width = 2)
    {
        float yShift = donwEdge ? width / 2 : -width / 2;
        PlaceArea(vCenter + new Vector3((x2 + x1) / 2, 0, y + yShift), Mathf.Abs(x2 - x1), width);
    }
    protected void PlaceArea(Vector3 vCenter, float width, float height)
    {
        for (int i=0;i< (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
                //if (Random.Range(0.0f, 100.0f) < placePercent)
                theCounter.Add();
                GameObject objRef = GetRandomGameObject();
                if (objRef)
                {
                    Vector3 vGridCenter = new Vector3(-width / 2 + 0.5f + (i * 1.0f), 0, -height / 2 + 0.5f + (j * 1.0f));
                    BattleSystem.SpawnGameObj(objRef, vGridCenter + vCenter);
                }
            }
        }
    }

    private void OnDestroy()
    {
        theCounter.Reset();
    }

    private void Update()
    {
        theCounter.Show();
    }

    class MyCounter
    {
        public int count = 0;
        bool show = false;
        public void Add() { count++; }
        public void Reset() { count = 0; show = false; }
        public void Show()
        {
            if (!show)
            {
                print("RoomObjectPlacement Block Count: " + count);
                show = true;
            }
        }
    }
    static MyCounter theCounter = new MyCounter();

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
