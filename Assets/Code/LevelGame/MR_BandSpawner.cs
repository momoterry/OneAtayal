using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_BandSpawner : MonoBehaviour
{
    public GameObject objRef;
    protected float Width = 10.0f;
    protected float Height = 10.0f;
    public float BandWidth = 1.0f;
    public int TotalNum = 10;

    protected List<Vector3> points;

    public void OnTG(GameObject whoTG)
    {
        points = OneUtility.Get3DRandomPointsInRectBand(transform.position, Width, Height, BandWidth, TotalNum);
        foreach (Vector3 pos in points)
        {
            BattleSystem.SpawnGameObj(objRef, pos);
        }
    }
}
