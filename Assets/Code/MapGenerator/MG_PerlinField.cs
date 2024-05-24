using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MapPoint
{
    public string name;
    public float x;
    public float y;
}

[System.Serializable]           //地城入口的位置存檔
public class CavPoint: MapPoint
{

}

[System.Serializable]
public class MapSavePerlinField : MapSaveDataBase
{
    public int CellSize = 4;
    public int mapCellWidthH = 15;
    public int mapCellHeightH = 10;
    public float NoiseScaleOn256 = 5.0f;
    public float highRatio = 0.35f;
    public float lowRatio = 0.35f;
    public float randomShiftX = -9999;
    public float randomShiftY = -9999;
    //public string mapMask64 = null;
    //public MapPoint[] mapPoints;
    public CavPoint[] cavPoints;
}


public class MG_PerlinField : MG_PerlinNoise
{
    //public string mapName;
    public DungeonEnemyManagerBase enemyManager;
    public MapDecadeGenerator decadeGenerator;
    public GameObject initGameplay;
    public int edgeWidth = 4;

    //public GameObject cavRef;

    protected Vector2Int initCell = Vector2Int.zero;

    //隨機記錄的部份
    protected bool needRandomShift = true;
    protected float randomShiftX = -1;
    protected float randomShiftY = -1;
    //地圖存檔資料
    protected MapSavePerlinField loadedMapData = null;

    //世界地連接相關資訊
    protected bool isWorldMap = false;
    protected Vector2Int myWolrdIndex;
    protected bool N;          //上方是否連接
    protected bool S;          //下方是否連接
    protected bool W;          //左方是否連接
    protected bool E;          //右方是否連接

    //地城相關
    [System.Serializable]
    public class DungeonInfo
    {
        public string dungeonID;
        public GameObject entranceRef;
    }
    public DungeonInfo[] dungeons;
    protected Dictionary<string, GameObject> allCavs = new Dictionary<string, GameObject>();    //cav = dungeon entrance
    //關鍵字
    protected const string CAVE_PREFIX = "EXTRA_ENTRACE_";
    //rotected Dictionary<string, GameObject> allCavs = new Dictionary<string, GameObject>(); 


    public void SetZone(ZonePF zone)        //由 WorldMap 設定的內容
    {
        //print("MG_PerlinField.SetZone !!");
        isWorldMap = true;

        NoiseScaleOn256 = zone.NoiseScaleOn256;
        randomShiftX = zone.perlinShiftX;
        randomShiftY = zone.perlinShiftY;
        needRandomShift = false;
        CellSize = zone.cellSize;
        mapName = zone.ID;
        mapCellWidthH = (int)zone.width / CellSize / 2;
        mapCellHeightH = (int)zone.height / CellSize / 2;
        edgeWidth = zone.edgeWidth;

        N = zone.N;
        S = zone.S;
        W = zone.W;
        E = zone.E;
        myWolrdIndex = zone.worldIndex;
}

    //public override void OnEixtMap()
    //{
    //    SaveExploreMap();
    //}

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

        float noiseScale = NoiseScaleOn256 / 256.0f;
        float randomSscale = 10.0f;
        float xShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        float yShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        if (!needRandomShift)
        {
            xShift = randomShiftX;
            yShift = randomShiftY;
        }
        //print("TEST XMin, Xmax: " + theCellMap.GetXMin() + "," + theCellMap.GetXMax());
        //print((float)theCellMap.GetXMin() * noiseScale + xShift);
        //print(xShift);
        //print((float)theCellMap.GetXMax() * noiseScale + xShift);
        //print((float)theCellMap.GetYMin() * noiseScale + yShift);
        //print(yShift);
        //print((float)theCellMap.GetYMax() * noiseScale + yShift);
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
            initCell = bestFound;
            if (initGameplay)
            {
                initGameplay.transform.position = theCellMap.GetCellCenterPosition(initCell.x, initCell.y);
                print("移動營地到 :" + initCell);
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

    protected Dictionary<string, Vector3> CalculateCavPoints()
    {
        print("無法載入 Cav 位置，計算中 ....");
        //float timeStart = Time.realtimeSinceStartup;
        float minCavDis = Mathf.Sqrt(mapCellWidthH * mapCellWidthH + mapCellHeightH * mapCellHeightH) * 0.3f;
        int noCavBuff = edgeWidth + 1;
        List<Vector2Int> cavCandidates = new List<Vector2Int>();
        for (int x = theCellMap.GetXMin() + noCavBuff; x <= theCellMap.GetXMax() - noCavBuff; x++)
        {
            for (int y = theCellMap.GetYMin() + noCavBuff; y <= theCellMap.GetYMax() - noCavBuff; y++)
            {
                if (theCellMap.GetValue(x, y) != (int)MY_VALUE.NORMAL)
                    continue;
                Vector2Int pos = new Vector2Int(x, y);
                float dis = Vector2Int.Distance(initCell, pos);
                if (dis > minCavDis)
                {
                    cavCandidates.Add(pos);
                }
            }
        }
        //TODO: 根據入口的名稱
        List<Vector2Int> cav2DPoints = GetMaxDistancePoints(cavCandidates, 5);
        Dictionary<string, Vector3> cavPoints = new Dictionary<string, Vector3>();
        for (int i = 0; i < dungeons.Length; i++)
        {
            //string cavName = CAVE_PREFIX + i;
            cavPoints.Add(dungeons[i].dungeonID, theCellMap.GetCellCenterPosition(cav2DPoints[i].x, cav2DPoints[i].y));
        }
        return cavPoints;
    }

    protected Dictionary<string, Vector3> TryLoadCavPoints()
    {
        if (loadedMapData == null || loadedMapData.cavPoints == null )
            return null;

        print("嘗試載入地城入口中 ......");
        Dictionary<string, Vector3> cavPoints = new Dictionary<string, Vector3>();
        for (int i = 0; i < dungeons.Length; i++)
        {
            bool isFound = false;
            for (int j=0; j<loadedMapData.cavPoints.Length; j++)
            {
                //print(dungeons[i].dungeonID + " -- " + loadedMapData.cavPoints[j].name);
                if (dungeons[i].dungeonID == loadedMapData.cavPoints[j].name)
                {
                    isFound = true;
                    //print("找到入口!!");
                    cavPoints.Add(dungeons[i].dungeonID, new Vector3(loadedMapData.cavPoints[i].x, 0, loadedMapData.cavPoints[i].y));
                    continue;
                }
                //else
                //{
                //    print("不是入口!!");
                //}
            }
            if (!isFound)
            {
                //print("至少一個入口存檔找不到，重新計算");
                return null;
            }
        }

        return cavPoints;
    }

    protected  void CreateCavEntrances( Dictionary<string, Vector3> cavPoints)
    {
        int listOriginal = entraceList.Length;
        MapEntraceData[] newList = new MapEntraceData[listOriginal + cavPoints.Count];
        System.Array.Copy(entraceList, newList, listOriginal);
        int i = 0;
        foreach (KeyValuePair<string, Vector3> p in cavPoints)
        {
            if (p.Key != dungeons[i].dungeonID)
                print("ERROR!! DungeonID 對不上 !!!!");
            GameObject oCavP = BattleSystem.SpawnGameObj(dungeons[i].entranceRef, p.Value);
            oCavP.name = p.Key;
            SceneEntrance se = oCavP.GetComponentInChildren<SceneEntrance>();
            ScenePortal sp = oCavP.GetComponent<ScenePortal>();

            if (se && sp)
            {
                //print("新增入口: ");
                newList[listOriginal + i] = new MapEntraceData();
                newList[listOriginal + i].name = p.Key;
                newList[listOriginal + i].pos = se.transform;
                newList[listOriginal + i].faceAngle = se.playerInitAngle;

                if (isWorldMap)
                    sp.backScene = WorldMap.WORLDMAP_SCENE;
                else
                    sp.backScene = BattleSystem.GetCurrScene();
                //print("設定地城入口的回程: " + sp.backScene);
                sp.backEntrance = p.Key;

                CMazeJsonData data = GameSystem.GetInstance().theDungeonData.GetMazeJsonData(dungeons[i].dungeonID);
                CMazeJsonPortal spJson = oCavP.GetComponent<CMazeJsonPortal>();
                if (data != null && spJson)
                {
                    print("有地城資料: " + data.name);
                    spJson.SetupCMazeJsonData(data);
                }

            }
            allCavs.Add(p.Key, oCavP);
            i++;
        }
        entraceList = newList;
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
            float dis2Max = Mathf.Sqrt((mapCellHeightH * mapCellHeightH) + (mapCellWidthH * mapCellWidthH));
            for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
            {
                for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
                {
                    if (theCellMap.GetValue(x, y) == (int)MY_VALUE.LOW)
                    {
                        Vector2Int iPos = theCellMap.GetCellCenterCoord(x, y);
                        Vector3 vPos = new Vector3(iPos.x + 0.5f, 0, iPos.y + 0.5f);

                        float dis2Ratio = Mathf.Sqrt((x - initCell.x)* (x - initCell.x) + (y - initCell.y)* (y - initCell.y)) / dis2Max;
                        //print("dis2Ratio:" + dis2Ratio);
                        enemyManager.AddNormalPosition(vPos, dis2Ratio);
                    }
                }
            }
            enemyManager.BuildAllGameplay();
        }

        //載入或重新計算地城入口
        //if (!TryLoadCavEntraces())
        //    CreateCavEntrances();
        Dictionary<string, Vector3> cavPoints = TryLoadCavPoints();
        if (cavPoints == null)
            cavPoints = CalculateCavPoints();
        CreateCavEntrances(cavPoints);

        //設定地城入口
        if (entranceID != "")
        {
            SetEntrance(entranceID);
        }

        //載入已探索的資訊
        LoadExploreMap();

        //地圖存檔
        SaveMap();

    }

    protected void ModifyMapEdge()
    {
        //邊界
        int iEdge = edgeTG == null ? (int)MY_VALUE.HIGH : (int)MY_VALUE.EDGE;

        if (edgeWidth >= mapCellWidthH || edgeWidth > mapCellHeightH)
            return;
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                if (!S)
                    theCellMap.SetValue(x, theCellMap.GetYMin() + i, iEdge);
                if (!N)
                    theCellMap.SetValue(x, theCellMap.GetYMax() - i, iEdge);
            }

            if (edgeTG == null)
            {
                if (!S && theCellMap.GetValue(x, theCellMap.GetYMin() + edgeWidth) == (int)MY_VALUE.LOW)
                    theCellMap.SetValue(x, theCellMap.GetYMin() + edgeWidth, (int)MY_VALUE.NORMAL);
                if (!N && theCellMap.GetValue(x, theCellMap.GetYMax() - edgeWidth) == (int)MY_VALUE.LOW)
                    theCellMap.SetValue(x, theCellMap.GetYMax() - edgeWidth, (int)MY_VALUE.NORMAL);
            }
            else
            {
                if (!S)
                    theCellMap.SetValue(x, theCellMap.GetYMin() + edgeWidth, (int)MY_VALUE.NORMAL);
                if (!N)
                    theCellMap.SetValue(x, theCellMap.GetYMax() - edgeWidth, (int)MY_VALUE.NORMAL);
            }
        }

        for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                if (!W)
                    theCellMap.SetValue(theCellMap.GetXMin() +i, y, iEdge);
                if (!E)
                    theCellMap.SetValue(theCellMap.GetXMax() -i, y, iEdge);
            }
            if (edgeTG == null)
            {
                if (!W && theCellMap.GetValue(theCellMap.GetXMin() + edgeWidth, y) == (int)MY_VALUE.LOW)
                    theCellMap.SetValue(theCellMap.GetXMin() + edgeWidth, y, (int)MY_VALUE.NORMAL);
                if (!E && theCellMap.GetValue(theCellMap.GetXMax() - edgeWidth, y) == (int)MY_VALUE.LOW)
                    theCellMap.SetValue(theCellMap.GetXMax() - edgeWidth, y, (int)MY_VALUE.NORMAL);
            }
            else
            {
                if (!W && theCellMap.GetValue(theCellMap.GetXMin() + edgeWidth, y) != (int)MY_VALUE.EDGE)
                    theCellMap.SetValue(theCellMap.GetXMin() + edgeWidth, y, (int)MY_VALUE.NORMAL);
                if (!E && theCellMap.GetValue(theCellMap.GetXMax() - edgeWidth, y) != (int)MY_VALUE.EDGE)
                    theCellMap.SetValue(theCellMap.GetXMax() - edgeWidth, y, (int)MY_VALUE.NORMAL);
            }
        }

        //加入邊界傳送區
        float xTotal = theCellMap.GetBooundXMax() - theCellMap.GetBooundXMin();
        float yTotal = theCellMap.GetBooundYMax() - theCellMap.GetBooundYMin();
        float xCenter = theCellMap.GetBooundXMin() + ( xTotal * 0.5f );
        float yCenter = theCellMap.GetBooundYMin() + (yTotal * 0.5f);
        float hEdge = 0.5f * edgeWidth * CellSize;
        if (W)
        {
            //CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(-1,0), theCellMap.GetCellCenterPosition(theCellMap.GetXMin(), 0), edgeWidth* CellSize, theCellMap.GetHeight()*CellSize, ZONE_EDGE_DIR.W);
            CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(-1, 0), new Vector3(theCellMap.GetBooundXMin()+ hEdge, yCenter), hEdge+ hEdge, yTotal, ZONE_EDGE_DIR.W);

        }
        if (E)
        {
            //CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(1, 0), theCellMap.GetCellCenterPosition(theCellMap.GetXMax(), 0), edgeWidth * CellSize, theCellMap.GetHeight() * CellSize, ZONE_EDGE_DIR.E);
            CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(1, 0), new Vector3(theCellMap.GetBooundXMax() - hEdge, 0, yCenter), hEdge + hEdge, yTotal, ZONE_EDGE_DIR.E);

        }
        if (S)
        {
            //CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(0, -1), theCellMap.GetCellCenterPosition(0, theCellMap.GetYMin()), theCellMap.GetWidth() * CellSize, edgeWidth * CellSize, ZONE_EDGE_DIR.S);
            CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(0, -1), new Vector3(xCenter, 0, theCellMap.GetBooundYMin() + hEdge), xTotal, hEdge + hEdge, ZONE_EDGE_DIR.S);
        }
        if (N)
        {
            //CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(0, 1), theCellMap.GetCellCenterPosition(0, theCellMap.GetYMax()), theCellMap.GetWidth() * CellSize, edgeWidth * CellSize, ZONE_EDGE_DIR.N);
            CreateZoneEdgeTrigger(myWolrdIndex + new Vector2Int(0, 1), new Vector3(xCenter, 0, theCellMap.GetBooundYMax() - hEdge), xTotal, hEdge + hEdge, ZONE_EDGE_DIR.N);
        }
    }

    protected enum ZONE_EDGE_DIR { S,N,E,W,}

    protected GameObject CreateZoneEdgeTrigger(Vector2Int toZoneIndex, Vector3 center, float width, float height, ZONE_EDGE_DIR dir)
    {
        GameObject o = new GameObject("ZoneTrigger_" + toZoneIndex);
        o.transform.position = center;
        BoxCollider bc = o.AddComponent<BoxCollider>();
        bc.size = new Vector3(width, 2.0f, height);
        bc.isTrigger = true;
        AreaTG atg = o.AddComponent<AreaTG>();
        atg.TriggerTargets = new GameObject[1];
        atg.triggerOnce = false;
        atg.TriggerTargets[0] = o;
        WorldPortal wp = o.AddComponent<WorldPortal>();
        wp.toWorldZoneIndex = toZoneIndex;
        wp.messageHint = true;
        switch (dir)
        {
            case ZONE_EDGE_DIR.N:
                wp.enterFaceAngle = 0;
                wp.enterPosition = theCellMap.GetCellCenterPosition(0, theCellMap.GetYMin() + edgeWidth);
                wp.enterWithCurrX = true;
                break;
            case ZONE_EDGE_DIR.S:
                wp.enterFaceAngle = 180;
                wp.enterPosition = theCellMap.GetCellCenterPosition(0, theCellMap.GetYMax() - edgeWidth);
                wp.enterWithCurrX = true;
                break;
            case ZONE_EDGE_DIR.E:
                wp.enterFaceAngle = 90;
                wp.enterPosition = theCellMap.GetCellCenterPosition(theCellMap.GetXMin() + edgeWidth, 0);
                wp.enterWithCurrZ = true;
                break;
            case ZONE_EDGE_DIR.W:
                wp.enterFaceAngle = -90;
                wp.enterPosition = theCellMap.GetCellCenterPosition(theCellMap.GetXMax() - edgeWidth, 0);
                wp.enterWithCurrZ = true;
                break;
        }
        GroundHintManager.GetInstance().ShowSquareHint(center, Vector3.back, new Vector2(width, height), -1, Color.green);
        return o;
    }

    override protected void CreateMapSaveData()
    {
        mapDataBase = new MapSavePerlinField();
    }

    override protected void SaveMap()
    {
        //MapSavePerlinField mapData = new MapSavePerlinField();
        //MapSavePerlinField mapData = mapDataBase == null ? new MapSavePerlinField() : (MapSavePerlinField)mapDataBase;
        //mapDataBase = mapData;
        base.SaveMap();
        if (mapDataBase == null)
            return;
        MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
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

        //if (entraceList.Length > 0)
        if (allCavs.Count > 0)
        {
            mapData.cavPoints = new CavPoint[allCavs.Count];
            //for (int i=0; i<mapData.mapPoints.Length; i++)
            int i = 0;
            foreach (KeyValuePair<string, GameObject> p in allCavs)
            {
                mapData.cavPoints[i] = new CavPoint();
                mapData.cavPoints[i].name = p.Key;
                mapData.cavPoints[i].x = p.Value.transform.position.x;
                mapData.cavPoints[i].y = p.Value.transform.position.z;
                i++;
            }
        }

        GameSystem.GetPlayerData().SaveMap(mapName, mapData);

    }

    override protected void LoadMap()
    {
        //mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        base.LoadMap();
        if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
        {
            print("LoadMap: 沒有存檔資料");
            return;
        }

        print("LoadMap: 找到存檔資料 !!!!");
        MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
        print("LoaMap Size: " + new Vector2Int(mapData.mapCellWidthH, mapData.mapCellHeightH));
        //print("LoaMap RandomShift: " + new Vector2(mapData.randomShiftX, mapData.randomShiftY));
        //print("LoadMap map64: " + mapData.mapMask64);

        mapCellWidthH = mapData.mapCellWidthH;
        mapCellHeightH = mapData.mapCellHeightH;
        CellSize = mapData.CellSize;
        NoiseScaleOn256 = mapData.NoiseScaleOn256;
        highRatio = mapData.highRatio;
        lowRatio = mapData.lowRatio;
        randomShiftX = mapData.randomShiftX;
        randomShiftY = mapData.randomShiftY;
        needRandomShift = false;

        loadedMapData = mapData;
    }

    //From AI
    private List<Vector2Int> GetMaxDistancePoints(List<Vector2Int> inputPoints, int maxAttempt = 100)
    {
        List<Vector2Int> maxDistancePoints = new List<Vector2Int>();
        float maxDistance = float.MinValue;

        // 簡單的啟發式方法，隨機選擇一些三點組合進行計算
        for (int attempt = 0; attempt < maxAttempt; attempt++)
        {
            int i = Random.Range(0, inputPoints.Count);
            int j = Random.Range(0, inputPoints.Count);
            int k = Random.Range(0, inputPoints.Count);

            // 確保三個點不相等
            if (i != j && i != k && j != k)
            {
                // 計算三個點之間的距離
                float distance = Vector2.Distance(inputPoints[i], inputPoints[j]) +
                                 Vector2.Distance(inputPoints[j], inputPoints[k]) +
                                 Vector2.Distance(inputPoints[k], inputPoints[i]);

                // 如果距離更大，更新最大距離和對應的三點
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistancePoints.Clear();
                    maxDistancePoints.Add(inputPoints[i]);
                    maxDistancePoints.Add(inputPoints[j]);
                    maxDistancePoints.Add(inputPoints[k]);
                }
            }
        }

        return maxDistancePoints;
    }

    //// ======= 有關探索地圖的記錄和回復 =======================

    //protected void SaveExploreMap()
    //{
    //    MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
    //    if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
    //    {
    //        print("SaveExploreMap: 沒有存檔資料 MapSaveData，不處理");
    //        return;
    //    }

    //    MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
    //    if (theMiniMap)
    //    {
    //        ////print("開始嘗試儲存 MiniMap");

    //        //Texture2D maskT = theMiniMap.GetMaskTexture();
    //        //// 獲取Texture2D的所有像素
    //        //Color[] pixels = maskT.GetPixels();

    //        //// 初始化alphaData數組
    //        //byte[] alphaData = new byte[pixels.Length];

    //        //// 將每個像素的alpha值轉換為字節數據
    //        //for (int i = 0; i < pixels.Length; i++)
    //        //{
    //        //    alphaData[i] = (byte)(pixels[i].a * 255);
    //        //}
    //        ////print("Byte 總量: " + alphaData.Length);

    //        //byte[] compressedAlphaData = OneUtility.CompressData(alphaData);
    //        ////print("壓縮後 Byte 總量" + compressedAlphaData.Length);
    //        ////print("壓縮後內容: " + compressedAlphaData);

    //        //string compressedAlpha64Text = System.Convert.ToBase64String(compressedAlphaData);
    //        //print("SaveExploreMap 壓縮後的文字量: " + compressedAlpha64Text.Length);
    //        ////print("TEXT: " + compressedAlpha64Text);


    //        //MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
    //        //mapData.mapMask64 = compressedAlpha64Text;
    //        MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
    //        mapData.mapMask64 = theMiniMap.EncodeMaskTexture();
    //    }
    //}

    //protected void LoadExploreMap()
    //{
    //    MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
    //    if (!theMiniMap)
    //    {
    //        return;
    //    }
    //    MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
    //    if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
    //    {
    //        print("LoadExploreMap : 沒有存檔資料，不處理");
    //        return;
    //    }

    //    MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
    //    if (mapData.mapMask64 == null || mapData.mapMask64 == "")
    //    {
    //        print("空的地圖探索資訊: " + mapData.mapMask64);
    //        return;
    //    }

    //    print("LoadExploreMap: 找到的文字壓縮資料，Byte 總量: " + mapData.mapMask64.Length);
    //    //print("找到的文字壓縮資料內容: " + mapData.mapMask64);

    //    theMiniMap.DecodeMaskTexture(mapData.mapMask64);

    //}

}
