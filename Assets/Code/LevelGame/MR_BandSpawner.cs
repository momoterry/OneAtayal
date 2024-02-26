using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MR_BandSpawner : MR_NodeBase
{
    public GameObject objRef;
    protected float Width = 10.0f;
    protected float Height = 10.0f;
    public int TotalNum = 10;
    public float BandWidth = 1.0f;
    public float BandBuffer = 0.25f;    // 不生成的緩衝距離

    protected List<Vector3> points;

    public void OnTG(GameObject whoTG)
    {
        points = OneUtility.Get3DRandomPointsInRectBand(transform.position, Width - BandBuffer - BandBuffer, Height - BandBuffer - BandBuffer, BandWidth, TotalNum);
        foreach (Vector3 pos in points)
        {
            BattleSystem.SpawnGameObj(objRef, pos);
        }
    }

    public override void OnSetupByRoom(MazeGameManager.RoomInfo room)
    {
        print("收到收到 !!" + name);
        base.OnSetupByRoom(room);
        Width = room.width;
        Height = room.height;
    }
}
