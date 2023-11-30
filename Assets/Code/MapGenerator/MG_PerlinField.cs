using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapSavePerlinField : MapSaveDataBase
{
    public int CellSize = 4;
    public int mapCellWidthH = 15;
    public int mapCellHeightH = 10;
    public int NoiseScaleOn256 = 5;
    public float highRatio = 0.35f;
    public float lowRatio = 0.35f;
    public float randomShiftX = -1;
    public float randomShiftY = -1;
}


public class MG_PerlinField : MG_PerlinNoise
{
    public string mapName;
    public DungeonEnemyManagerBase enemyManager;
    public MapDecadeGenerator decadeGenerator;
    public GameObject initGameplay;
    public int edgeWidth = 4;

    //隨機記錄的部份
    protected float randomShiftX = -1;
    protected float randomShiftY = -1;

    protected override int GetMapValue(float perlinValue)
    {
        if (perlinValue > 1.0f - highRatio)
        {
            return (int)MY_VALUE.HIGH;
        }
        else if (perlinValue < lowRatio)
        {
            return (int)MY_VALUE.LOW;
        }
        else
        {
            return (int)MY_VALUE.NORMAL;
        }
    }

    protected override void GenerateCellMap()
    {
        float saveMargin = 0.01f;
        List<Vector2Int> saveList = new List<Vector2Int>(); ;
        saveList.Add(Vector2Int.zero);

        float noiseScale = (float)NoiseScaleOn256 / 256.0f;
        float randomSscale = 10.0f;
        float xShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        float yShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        if (randomShiftX > 0 && randomShiftY > 0)
        {
            xShift = randomShiftX;
            yShift = randomShiftY;
        }
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
            {
                float rd = Mathf.PerlinNoise((float)x * noiseScale + xShift, (float)y * noiseScale + yShift);
                theCellMap.SetValue(x, y, GetMapValue(rd));

                if (rd < 0.5f + saveMargin && rd > 0.5f - saveMargin 
                    && x > theCellMap.GetXMin() + 2 && x < theCellMap.GetXMax() - 2
                    && y > theCellMap.GetYMin() + 2 && y < theCellMap.GetYMax() - 2)
                {
                    saveList.Add(new Vector2Int(x, y));
                }
            }
        }
        randomShiftX = xShift;
        randomShiftY = yShift;

        ModifyMapEdge();

        //營地區域尋找
        float minDis = Mathf.Infinity;
        int saveAreaSizeH = 2;
        Vector2Int bestFound = Vector2Int.zero;
        bool isBestPositionFound = false;
        foreach( Vector2Int savePos in saveList)
        {
            float dis = (savePos.x * savePos.x)  + (savePos.y * savePos.y);
            if (dis < minDis)
            {
                bool isCheck = true;
                for (int ix = savePos.x - saveAreaSizeH; ix <= savePos.x + saveAreaSizeH; ix++)
                {
                    for (int iy = savePos.y - saveAreaSizeH; iy <= savePos.y + saveAreaSizeH; iy++)
                    {
                        if (theCellMap.GetValue(ix, iy) != (int)MY_VALUE.NORMAL)
                        {
                            isCheck = false;
                            continue;
                        }
                    }
                }
                if (isCheck)
                {
                    bestFound = savePos;
                    minDis = dis;
                    isBestPositionFound = true;
                    //print("找到更好的點 " + dis);
                }
            }
        }


        //暴力法挖營地
        if (!isBestPositionFound)
        {
            print("找不到點，硬挖營地 !!");
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    theCellMap.SetValue(x, y, (int)MY_VALUE.NORMAL);
                }
            }
        }
        else
        {
            if (initGameplay)
            {
                initGameplay.transform.position = theCellMap.GetCellCenterPosition(bestFound.x, bestFound.y);
                print("移動營地到 :" + bestFound);
            }
        }
    }

    protected override void FillTiles()
    {
        base.FillTiles();

        if (decadeGenerator)
        {
            DecadeGenerateParameter p = new DecadeGenerateParameter();
            p.mapValue = (int)MY_VALUE.HIGH;
            decadeGenerator.BuildAll(theCellMap.GetOneMap(), p);
        }

    }

    protected override void PreBuild()
    {
        LoadMap();

        base.PreBuild();
    }

    protected override void PostBuild()
    {
        base.PostBuild();

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            theMiniMap.CreateMiniMap(theCellMap.GetOneMap());
        }

        if (enemyManager)
        {
            for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
            {
                for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
                {
                    if (theCellMap.GetValue(x, y) == (int)MY_VALUE.LOW)
                    {
                        Vector2Int iPos = theCellMap.GetCellCenterCoord(x, y);
                        Vector3 vPos = new Vector3(iPos.x + 0.5f, 0, iPos.y + 0.5f);
                        enemyManager.AddNormalPosition(vPos);
                    }
                }
            }
            enemyManager.BuildAllGameplay();
        }

        //地圖存檔
        SaveMap();
    }

    protected void ModifyMapEdge()
    {
        //邊界
        if (edgeWidth >= mapCellWidthH || edgeWidth > mapCellHeightH)
            return;
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                theCellMap.SetValue(x, theCellMap.GetYMin() + i, (int)MY_VALUE.HIGH);
                theCellMap.SetValue(x, theCellMap.GetYMax() - i, (int)MY_VALUE.HIGH);
            }
            if (theCellMap.GetValue(x, theCellMap.GetYMin() + edgeWidth) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(x, theCellMap.GetYMin() + edgeWidth, (int)MY_VALUE.NORMAL);
            if (theCellMap.GetValue(x, theCellMap.GetYMax() - edgeWidth) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(x, theCellMap.GetYMax() - edgeWidth, (int)MY_VALUE.NORMAL);
        }

        for (int y = theCellMap.GetYMin() + edgeWidth; y <= theCellMap.GetYMax() - edgeWidth; y++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                theCellMap.SetValue(theCellMap.GetXMin() +i, y, (int)MY_VALUE.HIGH);
                theCellMap.SetValue(theCellMap.GetXMax() -i, y, (int)MY_VALUE.HIGH);
            }
            if (theCellMap.GetValue(theCellMap.GetXMin() + edgeWidth, y) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(theCellMap.GetXMin() + edgeWidth, y, (int)MY_VALUE.NORMAL);
            if (theCellMap.GetValue(theCellMap.GetXMax() - edgeWidth, y) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(theCellMap.GetXMax() - edgeWidth, y, (int)MY_VALUE.NORMAL);
        }
    }

    protected void SaveMap()
    {
        MapSavePerlinField mapData = new MapSavePerlinField();
        mapData.className = "MG_PerlinField";
        mapData.mapName = mapName;
        mapData.mapCellWidthH = mapCellWidthH;
        mapData.mapCellHeightH = mapCellHeightH;
        mapData.CellSize = CellSize;
        mapData.NoiseScaleOn256 = NoiseScaleOn256;
        mapData.highRatio = highRatio;
        mapData.lowRatio = lowRatio;
        mapData.randomShiftX = randomShiftX;
        mapData.randomShiftY = randomShiftY;

        GameSystem.GetPlayerData().SaveMap(mapName, mapData);

    }

    protected void LoadMap()
    {
        MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().LoadMap(mapName);
        if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
        {
            print("LoadMap: 沒有存檔資料");
            return;
        }

        print("LoadMap: 找到存檔資料 !!!!");
        MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
        print("LoaMap Size: " + new Vector2Int(mapData.mapCellWidthH, mapData.mapCellHeightH));

        mapCellWidthH = mapData.mapCellWidthH;
        mapCellHeightH = mapData.mapCellHeightH;
        CellSize = mapData.CellSize;
        NoiseScaleOn256 = mapData.NoiseScaleOn256;
        highRatio = mapData.highRatio;
        lowRatio = mapData.lowRatio;
        randomShiftX = mapData.randomShiftX;
        randomShiftY = mapData.randomShiftY;

    }

}
