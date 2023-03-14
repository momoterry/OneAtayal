using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class MG_PerlinEnemy : MG_PerlinNoise
{
    public GameObject stoneSpawnerRef;
    //public Image MiniMapImageTest;
    public MiniMap theMiniMap;  //TODO: 改用 Global 呼叫的方式取得
    protected override void FillTiles()
    {
        base.FillTiles();

        //for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        //{
        //    for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
        //    {
        //        if (theCellMap.GetValue(x, y) == (int)MY_VALUE.LOW)
        //        {
        //            if (Random.value < 0.1f) 
        //            {
        //                Vector2Int c = theCellMap.GetCellCenterCoord(x, y);
        //                Vector3 pos = new Vector3(c.x, 0, c.y);
        //                BattleSystem.SpawnGameObj(stoneSpawnerRef, pos);
        //            }
        //        }
        //    }
        //}


        if (theMiniMap)
        {
            theMiniMap.CreateMinMap(theCellMap.GetOneMap(), MyGetColorCB);
        }
    }

    public Color MyGetColorCB(int value)
    {
        switch (value)
        {
            case 1:
                return new Color(0.5f, 0.8f, 0.30f);
            case 2:
                return new Color(0.7f, 0.65f, 0.33f);
            case 3:
            case 4:
            case 5:
            case 6:
                return new Color(0.1f, 0.2f, 0.1f);
        }
        return Color.black;
    }

}
