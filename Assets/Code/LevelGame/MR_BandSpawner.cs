using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MR_BandSpawner : MR_Node
{
    public GameObject objRef;
    public float Width = ROOM_RELATIVE_SIZE;
    public float Height = ROOM_RELATIVE_SIZE;
    public int TotalNum = 10;
    public float BandWidth = 1.0f;
    public float BandBuffer = 0f;    // 不生成的緩衝距離
    public bool spawnOnStart = false;

    protected List<Vector3> points;

    private void Start()
    {
        if (spawnOnStart)
            OnTG(gameObject);
    }
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
        //print("收到收到 !!" + name);
        base.OnSetupByRoom(room);
        Width *= widthRatio;
        Height *= heightRatio;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        float Buffer = BandBuffer + BandBuffer;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width- Buffer, 2.0f, Height- Buffer));
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width - BandWidth - BandWidth - Buffer, 2.0f, Height -BandWidth - BandWidth - Buffer));

    }

}
