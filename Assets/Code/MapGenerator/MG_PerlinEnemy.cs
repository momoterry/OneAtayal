using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_PerlinEnemy : MG_PerlinNoise
{
    public GameObject stoneSpawnerRef;
    protected override void FillTiles()
    {
        base.FillTiles();

        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
            {
                if (theCellMap.GetValue(x, y) == (int)MY_VALUE.LOW)
                {
                    if (Random.value < 0.1f) 
                    {
                        Vector2Int c = theCellMap.GetCellCenterCoord(x, y);
                        Vector3 pos = new Vector3(c.x, 0, c.y);
                        BattleSystem.SpawnGameObj(stoneSpawnerRef, pos);
                    }
                }
            }
        }
    }
}
