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

[System.Serializable]           //�a���J�f����m�s��
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

    //�H���O��������
    protected bool needRandomShift = true;
    protected float randomShiftX = -1;
    protected float randomShiftY = -1;
    //�a�Ϧs�ɸ��
    protected MapSavePerlinField loadedMapData = null;

    //�@�ɦa�s��������T
    protected bool isWorldMap = false;
    protected Vector2Int myWolrdIndex;
    protected bool N;          //�W��O�_�s��
    protected bool S;          //�U��O�_�s��
    protected bool W;          //����O�_�s��
    protected bool E;          //�k��O�_�s��

    //�a������
    [System.Serializable]
    public class DungeonInfo
    {
        public string dungeonID;
        public GameObject entranceRef;
    }
    public DungeonInfo[] dungeons;
    protected Dictionary<string, GameObject> allCavs = new Dictionary<string, GameObject>();    //cav = dungeon entrance
    //����r
    protected const string CAVE_PREFIX = "EXTRA_ENTRACE_";
    //rotected Dictionary<string, GameObject> allCavs = new Dictionary<string, GameObject>(); 


    public void SetZone(ZonePF zone)        //�� WorldMap �]�w�����e
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

        //��a�ϰ�M��
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
                    //print("����n���I " + dis);
                }
            }
        }


        //�ɤO�k����a
        if (!isBestPositionFound)
        {
            print("�䤣���I�A�w����a !!");
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
                print("������a�� :" + initCell);
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
        print("�L�k���J Cav ��m�A�p�⤤ ....");
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
        //TODO: �ھڤJ�f���W��
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

        print("���ո��J�a���J�f�� ......");
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
                    //print("���J�f!!");
                    cavPoints.Add(dungeons[i].dungeonID, new Vector3(loadedMapData.cavPoints[i].x, 0, loadedMapData.cavPoints[i].y));
                    continue;
                }
                //else
                //{
                //    print("���O�J�f!!");
                //}
            }
            if (!isFound)
            {
                //print("�ܤ֤@�ӤJ�f�s�ɧ䤣��A���s�p��");
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
                print("ERROR!! DungeonID �藍�W !!!!");
            GameObject oCavP = BattleSystem.SpawnGameObj(dungeons[i].entranceRef, p.Value);
            oCavP.name = p.Key;
            SceneEntrance se = oCavP.GetComponentInChildren<SceneEntrance>();
            ScenePortal sp = oCavP.GetComponent<ScenePortal>();

            if (se && sp)
            {
                //print("�s�W�J�f: ");
                newList[listOriginal + i] = new MapEntraceData();
                newList[listOriginal + i].name = p.Key;
                newList[listOriginal + i].pos = se.transform;
                newList[listOriginal + i].faceAngle = se.playerInitAngle;

                if (isWorldMap)
                    sp.backScene = WorldMap.WORLDMAP_SCENE;
                else
                    sp.backScene = BattleSystem.GetCurrScene();
                //print("�]�w�a���J�f���^�{: " + sp.backScene);
                sp.backEntrance = p.Key;

                CMazeJsonData data = GameSystem.GetInstance().theDungeonData.GetMazeJsonData(dungeons[i].dungeonID);
                CMazeJsonPortal spJson = oCavP.GetComponent<CMazeJsonPortal>();
                if (data != null && spJson)
                {
                    print("���a�����: " + data.name);
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

        //���J�έ��s�p��a���J�f
        //if (!TryLoadCavEntraces())
        //    CreateCavEntrances();
        Dictionary<string, Vector3> cavPoints = TryLoadCavPoints();
        if (cavPoints == null)
            cavPoints = CalculateCavPoints();
        CreateCavEntrances(cavPoints);

        //�]�w�a���J�f
        if (entranceID != "")
        {
            SetEntrance(entranceID);
        }

        //���J�w��������T
        LoadExploreMap();

        //�a�Ϧs��
        SaveMap();

    }

    protected void ModifyMapEdge()
    {
        //���
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

        //�[�J��ɶǰe��
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
            print("LoadMap: �S���s�ɸ��");
            return;
        }

        print("LoadMap: ���s�ɸ�� !!!!");
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

        // ²�檺�ҵo����k�A�H����ܤ@�ǤT�I�զX�i��p��
        for (int attempt = 0; attempt < maxAttempt; attempt++)
        {
            int i = Random.Range(0, inputPoints.Count);
            int j = Random.Range(0, inputPoints.Count);
            int k = Random.Range(0, inputPoints.Count);

            // �T�O�T���I���۵�
            if (i != j && i != k && j != k)
            {
                // �p��T���I�������Z��
                float distance = Vector2.Distance(inputPoints[i], inputPoints[j]) +
                                 Vector2.Distance(inputPoints[j], inputPoints[k]) +
                                 Vector2.Distance(inputPoints[k], inputPoints[i]);

                // �p�G�Z����j�A��s�̤j�Z���M�������T�I
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

    //// ======= ���������a�Ϫ��O���M�^�_ =======================

    //protected void SaveExploreMap()
    //{
    //    MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
    //    if (mapDataBase == null || mapDataBase.className != "MG_PerlinField")
    //    {
    //        print("SaveExploreMap: �S���s�ɸ�� MapSaveData�A���B�z");
    //        return;
    //    }

    //    MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
    //    if (theMiniMap)
    //    {
    //        ////print("�}�l�����x�s MiniMap");

    //        //Texture2D maskT = theMiniMap.GetMaskTexture();
    //        //// ���Texture2D���Ҧ�����
    //        //Color[] pixels = maskT.GetPixels();

    //        //// ��l��alphaData�Ʋ�
    //        //byte[] alphaData = new byte[pixels.Length];

    //        //// �N�C�ӹ�����alpha���ഫ���r�`�ƾ�
    //        //for (int i = 0; i < pixels.Length; i++)
    //        //{
    //        //    alphaData[i] = (byte)(pixels[i].a * 255);
    //        //}
    //        ////print("Byte �`�q: " + alphaData.Length);

    //        //byte[] compressedAlphaData = OneUtility.CompressData(alphaData);
    //        ////print("���Y�� Byte �`�q" + compressedAlphaData.Length);
    //        ////print("���Y�᤺�e: " + compressedAlphaData);

    //        //string compressedAlpha64Text = System.Convert.ToBase64String(compressedAlphaData);
    //        //print("SaveExploreMap ���Y�᪺��r�q: " + compressedAlpha64Text.Length);
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
    //        print("LoadExploreMap : �S���s�ɸ�ơA���B�z");
    //        return;
    //    }

    //    MapSavePerlinField mapData = (MapSavePerlinField)mapDataBase;
    //    if (mapData.mapMask64 == null || mapData.mapMask64 == "")
    //    {
    //        print("�Ū��a�ϱ�����T: " + mapData.mapMask64);
    //        return;
    //    }

    //    print("LoadExploreMap: ��쪺��r���Y��ơAByte �`�q: " + mapData.mapMask64.Length);
    //    //print("��쪺��r���Y��Ƥ��e: " + mapData.mapMask64);

    //    theMiniMap.DecodeMaskTexture(mapData.mapMask64);

    //}

}
