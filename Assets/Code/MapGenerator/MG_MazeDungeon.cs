using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MapSaveMazeDungeon : MapSaveDataBase
{
    [System.Serializable]
    public class RoomSave
    {
        public RectInt rect;
    }
    public int cellWidth = 4;
    public int cellHeight = 4;
    public int wallWidth = 2;
    public int wallHeight = 2;
    public int puzzleHeight = 6;
    public int puzzleWidth = 6;

    public bool extendTerminal = true;
    public bool portalAfterFirstRoomGamplay = false;

    public Vector3 startPos;
    public Vector3 endPos;
    public string puzzleMapData = null;
    public RoomSave[] rooms;

    public string mapMask64 = null;
    public CavPoint[] cavPoints;
}

public class MG_MazeDungeon : MapGeneratorBase
{
    public string mapName;              //用來識別存檔
    //地圖存檔資料
    protected MapSaveMazeDungeon loadedMapData = null;  //如果有的話，是載入存檔的形式
    // 迷宮資料相關
    public int cellWidth = 4;
    public int cellHeight = 4;
    public int wallWidth = 2;
    public int wallHeight = 2;
    public int puzzleHeight = 6;
    public int puzzleWidth = 6;
    public bool allConnect = true;
    public bool extendTerminal = true;
    public GameObject finishPortalRef;
    public bool portalAfterFirstRoomGamplay = false;
    public bool createWallCollider = true;
    protected const float wallBuffer = 0.25f;

    // Tile 資料相關
    public TileGroupDataBase groundTileGroup;
    public TileEdgeGroupDataBase groundEdgeTileGroup;
    public TileEdgeGroupDataBase groundOutEdgeTileGroup;
    public TileGroupDataBase blockTileGroup;                //用在非邊界的 Block 區域
    public TileGroupDataBase defautTileGroup;               //用在地圖的外邊界
    public TileGroupDataBase roomGroundTileGroup;
    public TileEdgeGroupDataBase roomGroundTileEdgeGroup;
    public Tilemap groundTM;
    public Tilemap blockTM;

    // Big Room 用
    [System.Serializable]
    public class BigRoomInfo
    {
        public Vector2Int size;
        public int numDoor;
        public GameObject gameplayRef;
        public float difficultyAdd;     //特別用於校正 Boss 強度用
    }
    public BigRoomInfo[] bigRooms;
    public int noRoomBuffer = 1;    //避免入口就遇到 Room 的緩衝
    protected int bigRoomNum;

    //Gameplay 用
    [Header("Gameplay 設定")]
    public DungeonEnemyManagerBase dungeonEnemyManager; //如果有指定的話，就不管下面的 normalGroup、normalEnemyRate 和 normalEnemyNum
    public EnemyGroup normalGroup;
    public float normalEnemyRate = 0.2f;
    protected int normalEnemyNum;
    protected float dungeonEnemyDifficulty = 1.0f;
    public float noEnemyDistanceRate = 1.0f;    //以 Cell 對角長度為單位

    public GameObject[] exploreRewards;
    protected int exploreRewardNum;

    public GameObject initGampleyRef;

    //裝飾物件
    public MapDecadeGenerator decadeGenerator;


    //基底地圖相關 TODO: 希望獨立出去
    protected int mapWidth = 0;
    protected int mapHeight = 0;
    protected int borderWidth = 4;
    protected Vector3Int mapCenter;
    protected OneMap theMap = new OneMap();
    protected enum MAP_TYPE
    {
        GROUND = 4,
        ROOM = 5,
        BLOCK = 6,
    }

    protected int bufferX = 0;
    protected int bufferY = 0;

    protected int puzzleX1;
    protected int puzzleY1;

    protected int iStart;
    protected int iEnd;
    protected Vector3 startPos;
    protected Vector3 endPos;

    protected class cellInfo
    {
        public bool U, D, L, R;
        public int value = NORMAL;

        public const int NORMAL = 0;
        public const int ROOM = 1;
        public const int TERNIMAL = 2;  //出、入口
        public const int INVALID = 7;

        public int Encode()
        {
            int iDoor = 0;
            iDoor += U ? 8 : 0;
            iDoor += D ? 4 : 0;
            iDoor += L ? 2 : 0;
            iDoor += R ? 1 : 0;
            int iAll = value * 16 + iDoor;
            if (iAll > 255)
            {
                print("ERROR!!!! cellInfo.Encode > 255!!");
            }
            return iAll;
        }

        public void Decode(int code)
        {
            //print(code);
            R = (code % 2) == 1 ? true : false;
            code = code >> 1;
            L = (code % 2) == 1 ? true : false;
            code = code >> 1;
            D = (code % 2) == 1 ? true : false;
            code = code >> 1;
            U = (code % 2) == 1 ? true : false;
            code = code >> 1;

            value = code;
        }
    }

    protected DisjointSetUnion puzzleDSU = new DisjointSetUnion();
    protected cellInfo[][] puzzleMap;

    protected class wallInfo
    {
        public wallInfo(int _id1, int _id2)
        {
            cell_ID_1 = _id1;
            cell_ID_2 = _id2;
        }
        public int cell_ID_1;
        public int cell_ID_2;
    }
    protected List<wallInfo> wallList = new List<wallInfo>();


    public override void BuildAll(int buildLevel = 1)
    {
        bigRoomNum = bigRooms.Length;
        exploreRewardNum = exploreRewards.Length;
        if (normalGroup)
        {
            normalEnemyNum = normalGroup.randomEnemyTotal;
        }

        PresetByContinuousBattle();

        if (mapName != null && mapName != "")
            LoadMap();  //先嘗試載入存檔，有的話更新地圖參數等

        PreCreateMap();

        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth);


        //主要地圖設計部份
        if (loadedMapData != null)
        {
            LoadMazeMap();
        }
        else
        {
            CreatMazeMap();
        }

        ProcessNormalCells();
        ProcessBigRoomAndInitFinish();

        //theMap.PrintMap();
        if (defautTileGroup)
            theMap.FillTileAll(OneMap.DEFAULT_VALUE, blockTM, defautTileGroup.GetTileGroup());

        if (groundEdgeTileGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTM, groundTileGroup.GetTileGroup(), groundEdgeTileGroup.GetTileEdgeGroup(), false, (int)MAP_TYPE.BLOCK);
        else
            theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTileGroup.GetTileGroup());

        if (roomGroundTileGroup != null)
        {
            if (roomGroundTileEdgeGroup)
                theMap.FillTileAll((int)MAP_TYPE.ROOM, groundTM, groundTM, roomGroundTileGroup.GetTileGroup(), roomGroundTileEdgeGroup.GetTileEdgeGroup(), false);
            else
                theMap.FillTileAll((int)MAP_TYPE.ROOM, groundTM, roomGroundTileGroup.GetTileGroup());
        }

        if (blockTileGroup)
            theMap.FillTileAll((int)MAP_TYPE.BLOCK, blockTM, blockTileGroup.GetTileGroup());


        if (groundOutEdgeTileGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, null, blockTM, null, groundOutEdgeTileGroup.GetTileEdgeGroup(), true, (int)MAP_TYPE.BLOCK);

        GenerateNavMesh(theSurface2D);

        //裝飾物件建立
        //MapDecadeGeneratorBase dGen = GetComponent<MapDecadeGeneratorBase>();
        if (decadeGenerator)
        {
            DecadeGenerateParameter p = new DecadeGenerateParameter();
            p.mapValue = (int)MAP_TYPE.BLOCK;
            decadeGenerator.BuildAll(theMap, p);
        }

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            theMiniMap.CreateMiniMap(theMap, MyGetColorCB);
        }

        //地圖存檔
        SaveMap();
    }

    public Color MyGetColorCB(int value)
    {
        switch (value)
        {
            case (int)MAP_TYPE.GROUND:
                return new Color(0.5f, 0.5f, 0.5f);
                //case (int)MAP_TYPE.BLOCK:
                //    return new Color(1.0f, 1.0f, 1.0f);
        }
        return Color.black;
    }

    virtual protected void PresetByContinuousBattle()
    {
        ContinuousBattleDataBase cBase = ContinuousBattleManager.GetCurrBattleData();
        if (cBase != null)
        {
            if (cBase is ContinuousMazeData)
            {
                ContinuousMazeData cData = (ContinuousMazeData)cBase;
                puzzleWidth = cData.puzzleWidth;
                puzzleHeight = cData.puzzleHeight;

                if (cData.bigRooms != null && cData.bigRooms.Length > 0)
                {
                    bigRoomNum = cData.bigRooms.Length;
                    bigRooms = cData.bigRooms;
                    portalAfterFirstRoomGamplay = cData.portalAfterFirstRoomGamplay;
                }
                else
                    bigRoomNum = Mathf.Min(bigRoomNum, cData.bigRoomNum);
                print("根據資料修正了迷宮大小: " + puzzleWidth + " - " + puzzleHeight + " Room 數: " + bigRoomNum);

                if (cData.dungeonDifficulty > 0)
                {
                    dungeonEnemyDifficulty = cData.dungeonDifficulty;
                }
                //print("DungeonEnemyDifficulty : " + dungeonEnemyDifficulty);
                if (cData.dungeonEnemyManager != null)
                {
                    //dungeonEnemyManager = cData.dungeonEnemyManager;      //不能直接使用，要產生一個實體

                    GameObject o = Instantiate(cData.dungeonEnemyManager.gameObject);
                    o.transform.parent = gameObject.transform;
                    dungeonEnemyManager = o.GetComponent<DungeonEnemyManager>();
                }
                if (cData.normalEnemyNum > 0)
                {
                    if (normalGroup)
                    {
                        normalEnemyNum = cData.normalEnemyNum;
                        normalGroup.randomEnemyTotal = normalEnemyNum;
                    }
                }
                if (cData.normalEnemyRate > 0)
                {
                    normalEnemyRate = cData.normalEnemyRate;
                }
                if (cData.exploreRewards != null && cData.exploreRewards.Length > 0)
                {
                    exploreRewardNum = cData.exploreRewards.Length;
                    exploreRewards = cData.exploreRewards;
                }
                if (cData.initGameplayRef)
                {
                    initGampleyRef = cData.initGameplayRef;
                }
                else if (cData.maxExploreReward > 0)
                {
                    exploreRewardNum = Mathf.Min(exploreRewardNum, cData.maxExploreReward);
                }

                if (cData.levelID != null)
                {
                    BattleSystem.GetInstance().levelID = cData.levelID;
                }
            }
            else
            {
                print("ERROR!! ContinuousBattle 錯誤，下個關卡資料不是 ContinuousMazeData !!");
            }
        }
    }

    virtual protected void PreCreateMap()
    {
        if (extendTerminal)
        {
            bufferX = 0;
            bufferY = 1;
        }
        else
        {
            bufferY = 0;
            bufferX = 0;
        }
        mapHeight = (puzzleHeight + bufferY + bufferY) * cellHeight;  //加入上下緩衝
        mapWidth = (puzzleWidth + bufferX + bufferX) * cellWidth;
        mapCenter.y = puzzleHeight * cellHeight / 2 - (cellHeight / 2);
        if (extendTerminal)
            mapCenter.y += cellHeight;

        if (puzzleWidth % 2 == 0)
        {
            mapCenter.x = -cellWidth / 2;
        }

        puzzleX1 = mapCenter.x - (puzzleWidth * cellWidth / 2);
        puzzleY1 = mapCenter.y - (puzzleHeight * cellHeight / 2);
    }

    protected void FillBlock(float x1, float y1, float width, float height)
    {
        if (!createWallCollider)
            return;
        GameObject newObject = new GameObject("MyBoxObj");
        newObject.transform.position = new Vector3(x1 + width * 0.5f, 0, y1 + height * 0.5f);
        BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, 2.0f, height);
        //boxCollider.l
        newObject.transform.parent = gameObject.transform;
        newObject.isStatic = true;
        newObject.layer = LayerMask.NameToLayer("Wall");
    }

    protected void FillCell(cellInfo cell, int x1, int y1, int width, int height)
    {
        int x2 = x1 + width - wallWidth;
        int y2 = y1 + height - wallHeight;
        if (cell.value == cellInfo.INVALID)
        {
            theMap.FillValue(x1, y1, width, height, (int)MAP_TYPE.BLOCK);
            return;
        }
        theMap.FillValue(x1, y1, width, height, (int)MAP_TYPE.GROUND);

        theMap.FillValue(x1, y1, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x1, y2, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2, y1, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2, y2, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);

        bool isFillBlock = createWallCollider && (cell.value != cellInfo.ROOM);

        if (!cell.D)
        {
            theMap.FillValue(x1 + wallWidth, y1, width - wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1, y1, width - wallWidth + wallBuffer, wallHeight - wallBuffer);   //左下和下
        }
        else
        {
            if (isFillBlock)
                FillBlock(x1, y1, wallWidth - wallBuffer, wallHeight - wallBuffer);   // 左下
        }
        if (!cell.U)
        {
            theMap.FillValue(x1 + wallWidth, y2, width - wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1 + wallWidth - wallBuffer, y2 + wallBuffer, width - wallWidth + wallBuffer, wallHeight - wallBuffer);   //右上和上
        }
        else
        {
            if (isFillBlock)
                FillBlock(x2 + wallBuffer, y2 + wallBuffer, wallWidth - wallBuffer, wallHeight - wallBuffer);   // 右上
        }

        if (!cell.L)
        {
            theMap.FillValue(x1, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1, y1 + wallHeight - wallBuffer, wallWidth - wallBuffer, height - wallHeight + wallBuffer); //左上和左
        }
        else
        {
            if (isFillBlock)
                FillBlock(x1, y2 + wallBuffer, wallWidth - wallBuffer, wallHeight - wallBuffer);    //左上
        }
        if (!cell.R)
        {
            theMap.FillValue(x2, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x2 + wallBuffer, y1, wallWidth - wallBuffer, height - wallHeight + wallBuffer);  //右下和右
        }
        else
        {
            if (isFillBlock)
                FillBlock(x2 + wallBuffer, y1, wallWidth - wallBuffer, wallHeight - wallBuffer);   //右下
        }
    }

    protected void FillRoomWallColliders(RectInt rc)
    {
        int x1 = puzzleX1 + rc.x * cellWidth;
        int y1 = puzzleY1 + rc.y * cellHeight;
        //FillBlock(x1, y1, rc.width * cellWidth, rc.height * cellHeight);
        //下方
        float startx = x1;
        float endx = x1;
        for (int ix = rc.x; ix < rc.x + rc.width; ix++)
        {
            if (puzzleMap[ix][rc.y].D)
            {
                //print("下方| |");
                FillBlock(startx, y1, endx - startx + wallWidth - wallBuffer, wallHeight - wallBuffer);
                startx = puzzleX1 + (ix + 1) * cellWidth - wallWidth + wallBuffer;
                endx = puzzleX1 + (ix + 1) * cellWidth;
            }
            else
            {
                //print("下方==");
                endx += cellWidth;
            }
        }
        FillBlock(startx, y1, endx - startx, wallHeight - wallBuffer);
        //上方
        startx = x1;
        endx = x1;
        float yU = y1 + rc.height * cellHeight - wallHeight + wallBuffer;
        for (int ix = rc.x; ix < rc.x + rc.width; ix++)
        {
            if (puzzleMap[ix][rc.y + rc.height - 1].U)
            {
                //print("上方| |");
                FillBlock(startx, yU, endx - startx + wallWidth - wallBuffer, wallHeight - wallBuffer);
                startx = puzzleX1 + (ix + 1) * cellWidth - wallWidth + wallBuffer;
                endx = puzzleX1 + (ix + 1) * cellWidth;
            }
            else
            {
                //print("上方==");
                endx += cellWidth;
            }
        }
        FillBlock(startx, yU, endx - startx, wallHeight - wallBuffer);
        //左方
        float starty = y1;
        float endy = y1;
        for (int iy = rc.y; iy < rc.y + rc.height; iy++)
        {
            if (puzzleMap[rc.x][iy].L)
            {
                //print("左方| |");
                FillBlock(x1, starty, wallWidth - wallBuffer, endy - starty + wallHeight - wallBuffer);
                starty = puzzleY1 + (iy + 1) * cellHeight - wallHeight + wallBuffer;
                endy = puzzleY1 + (iy + 1) * cellHeight;
            }
            else
            {
                //print("左方==");
                endy += cellHeight;
            }
        }
        FillBlock(x1, starty, wallWidth - wallBuffer, endy - starty);
        //右方
        starty = y1;
        endy = y1;
        float xR = x1 + rc.width * cellWidth - wallWidth + wallBuffer;
        for (int iy = rc.y; iy < rc.y + rc.height; iy++)
        {
            if (puzzleMap[rc.x + rc.width - 1][iy].R)
            {
                //print("右方| |");
                FillBlock(xR, starty, wallWidth - wallBuffer, endy - starty + wallHeight - wallBuffer);
                starty = puzzleY1 + (iy + 1) * cellHeight - wallHeight + wallBuffer;
                endy = puzzleY1 + (iy + 1) * cellHeight;
            }
            else
            {
                //print("右方==");
                endy += cellHeight;
            }
        }
        FillBlock(xR, starty, wallWidth - wallBuffer, endy - starty);
    }

    protected void ConnectCellsByID(int id_1, int id_2)
    {
        cellInfo cell_1 = puzzleMap[GetCellX(id_1)][GetCellY(id_1)];
        cellInfo cell_2 = puzzleMap[GetCellX(id_2)][GetCellY(id_2)];
        if (id_1 + 1 == id_2) //左連到右
        {
            cell_1.R = true;
            cell_2.L = true;
        }
        else if (id_1 + puzzleWidth == id_2) //下連到上
        {
            cell_1.U = true;
            cell_2.D = true;
        }
    }

    protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
    protected int GetCellX(int id) { return id % puzzleWidth; }
    protected int GetCellY(int id) { return id / puzzleWidth; }

    protected Vector3 GetCellCenterPos(int x, int y)
    {
        return new Vector3(puzzleX1 + cellWidth * (x + 0.5f), 0, puzzleY1 + cellHeight * (y + 0.5f));
    }

    virtual protected void InitPuzzleMap() { }

    protected List<RectInt> rectList;

    protected Vector2Int puzzleStart, puzzleEnd;

    protected void CreatMazeMap()
    {
        //==== Init Puzzle Map
        puzzleDSU.Init(puzzleHeight * puzzleWidth);
        puzzleMap = new cellInfo[puzzleWidth][];
        for (int i = 0; i < puzzleWidth; i++)
        {
            puzzleMap[i] = new cellInfo[puzzleHeight];
            for (int j = 0; j < puzzleHeight; j++)
            {
                puzzleMap[i][j] = new cellInfo();
            }
        }
        puzzleStart = new Vector2Int(puzzleWidth / 2, 0);
        puzzleEnd = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);

        InitPuzzleMap();        //處理非全方型 PuzzleMap 的情況

        //==== Init Connection Info
        wallInfo[,] lrWalls = new wallInfo[puzzleWidth - 1, puzzleHeight];
        wallInfo[,] udWalls = new wallInfo[puzzleWidth, puzzleHeight + 1];

        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                bool addToWallList = true;
                if (puzzleMap[x][y].value == cellInfo.INVALID)
                    addToWallList = false;

                if (x < puzzleWidth - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x + 1, y));
                    if (addToWallList && puzzleMap[x + 1][y].value != cellInfo.INVALID)
                        wallList.Add(w);
                    lrWalls[x, y] = w;
                }
                if (y < puzzleHeight - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x, y + 1));
                    if (addToWallList && puzzleMap[x][y + 1].value != cellInfo.INVALID)
                        wallList.Add(w);
                    udWalls[x, y] = w;
                }
            }
        }

        // ==== 產生大 Room
        List<Vector2Int> sizeList = new List<Vector2Int>();
        for (int i = 0; i < bigRoomNum; i++)
        {
            if (bigRooms[i].size.x > 0 && bigRooms[i].size.y > 0)
                sizeList.Add(bigRooms[i].size);
        }
        rectList = CreateNonOverlappingRects(sizeList, new RectInt(0, noRoomBuffer, puzzleWidth, puzzleHeight - noRoomBuffer));

        foreach (RectInt rc in rectList)
        {
            int rX = rc.x; int rY = rc.y;
            int rW = rc.width; int rH = rc.height;
            //List<wallInfo> roomWalls = new List<wallInfo>();
            for (int x = rX; x < rX + rW; x++)
            {
                if (rY > 0)
                {
                    wallList.Remove(udWalls[x, rY - 1]);
                }
                if (rY + rH < puzzleHeight)
                {
                    wallList.Remove(udWalls[x, rY + rH - 1]);
                }
            }
            for (int y = rY; y < rY + rH; y++)
            {
                if (rX > 0)
                {
                    wallList.Remove(lrWalls[rX - 1, y]);
                }
                if (rX + rW < puzzleWidth)
                {
                    wallList.Remove(lrWalls[rX + rW - 1, y]);
                }
            }

            for (int x = rX; x < rX + rW; x++)
            {
                for (int y = rY; y < rY + rH; y++)
                {
                    if (x + 1 < rX + rW)
                    {
                        ConnectCellsByID(GetCellID(x, y), GetCellID(x + 1, y));
                        puzzleDSU.Union(GetCellID(x, y), GetCellID(x + 1, y));
                    }
                    if (y + 1 < rY + rH)
                    {
                        ConnectCellsByID(GetCellID(x, y), GetCellID(x, y + 1));
                        puzzleDSU.Union(GetCellID(x, y), GetCellID(x, y + 1));
                    }
                    puzzleMap[x][y].value = cellInfo.ROOM;
                }
            }
        }

        //==== 開始隨機連結 !!
        //iStart = GetCellID(puzzleWidth / 2, 0);
        //iEnd = GetCellID(puzzleWidth / 2, puzzleHeight - 1);
        iStart = GetCellID(puzzleStart.x, puzzleStart.y);
        iEnd = GetCellID(puzzleEnd.x, puzzleEnd.y);
        if (allConnect)
        {
            //使用隨機排序
            OneUtility.Shuffle(wallList);
            foreach (wallInfo w in wallList)
            {
                if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //不要自體相連
                {
                    ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                    puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
                }
            }
        }
        else
        {
            //使用隨機排序
            OneUtility.Shuffle(wallList);
            foreach (wallInfo w in wallList)
            {
                if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //不要自體相連
                {
                    ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                    puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
                    if (puzzleDSU.Find(iStart) == puzzleDSU.Find(iEnd))
                    {
                        //print("發現大祕寶啦 !! Loop = " + (loop + 1));
                        break;
                    }
                }
            }
        }

        //==== 連結完再處理 Big Room 的開口
        for (int i = 0; i < rectList.Count; i++)
        {
            RectInt rc = rectList[i];
            List<wallInfo> roomWalls = new List<wallInfo>();
            for (int x = rc.x; x < rc.xMax; x++)
            {
                if (rc.y > 0)
                {
                    if (puzzleMap[x][rc.y - 1].value != cellInfo.INVALID)
                        roomWalls.Add(udWalls[x, rc.y - 1]);
                }
                if (rc.yMax < puzzleHeight)
                {
                    if (puzzleMap[x][rc.yMax].value != cellInfo.INVALID)
                        roomWalls.Add(udWalls[x, rc.yMax - 1]);
                }
            }
            for (int y = rc.y; y < rc.yMax; y++)
            {
                if (rc.x > 0)
                {
                    if (puzzleMap[rc.x - 1][y].value != cellInfo.INVALID)
                        roomWalls.Add(lrWalls[rc.x - 1, y]);
                }
                if (rc.xMax < puzzleWidth)
                {
                    if (puzzleMap[rc.xMax][y].value != cellInfo.INVALID)
                        roomWalls.Add(lrWalls[rc.xMax - 1, y]);
                }
            }

            //print("roomWalls.Count: " + roomWalls.Count);
            for (int c = 0; c < bigRooms[i].numDoor; c++)
            {
                if (roomWalls.Count == 0)
                {
                    print("大房間沒有那麼多牆可以開洞了 !!");
                    break;
                }
                wallInfo w = roomWalls[Random.Range(0, roomWalls.Count)];
                //print("w: "+w);
                ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
                roomWalls.Remove(w);
            }
        }

        //==== Set up all cells
        //puzzleX1 = mapCenter.x - (puzzleWidth * cellWidth / 2);
        //puzzleY1 = mapCenter.y - (puzzleHeight * cellHeight / 2);

        startPos = new Vector3(puzzleX1 + GetCellX(iStart) * cellWidth + cellWidth / 2, 1, puzzleY1 + GetCellY(iStart) * cellHeight + cellHeight / 2);
        endPos = new Vector3(puzzleX1 + GetCellX(iEnd) * cellWidth + cellWidth / 2, 1, puzzleY1 + GetCellY(iEnd) * cellHeight + cellHeight / 2);

        //==== 起終點上下延申處理
        if (extendTerminal)
        {
            //起始區處理
            if (puzzleStart.y == 0)
            {
                cellInfo cStart = new cellInfo();
                cStart.U = true;
                FillCell(cStart, puzzleX1 + GetCellX(iStart) * cellWidth, puzzleY1 + (GetCellY(iStart) - 1) * cellHeight, cellWidth, cellHeight);
            }
            else
            {
                puzzleMap[GetCellX(iStart)][GetCellY(iStart) - 1].U = true;
                puzzleMap[GetCellX(iStart)][GetCellY(iStart) - 1].value = cellInfo.TERNIMAL;
            }

            if (puzzleEnd.y == (puzzleHeight - 1))
            {
                cellInfo cEnd = new cellInfo();
                cEnd.D = true;
                FillCell(cEnd, puzzleX1 + GetCellX(iEnd) * cellWidth, puzzleY1 + (GetCellY(iEnd) + 1) * cellHeight, cellWidth, cellHeight);
            }
            else
            {
                puzzleMap[GetCellX(iEnd)][GetCellY(iEnd) + 1].D = true;
                puzzleMap[GetCellX(iEnd)][GetCellY(iEnd) + 1].value = cellInfo.TERNIMAL;
            }

            puzzleMap[GetCellX(iStart)][GetCellY(iStart)].D = true;
            puzzleMap[GetCellX(iEnd)][GetCellY(iEnd)].U = true;

            startPos.z -= cellHeight;
            endPos.z += cellHeight;
        }
        else
        {
            puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.TERNIMAL;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.TERNIMAL;
        }

        //==== 一般通道處理
        //ProcessNormalCells();

        //List<Vector2Int> deadEnds = new List<Vector2Int>();
        //int startValue = puzzleDSU.Find(iStart);
        //float enemyMinDistanceToStart = Mathf.Sqrt((cellHeight * cellHeight) + (cellWidth * cellWidth)) * noEnemyDistanceRate + 0.1f;
        ////用來標記判斷敵人強度變化的值 TODO: 想辦法改用路徑
        //float maxDistance = Mathf.Sqrt((puzzleWidth * cellWidth * 0.5f) * (puzzleWidth * cellWidth * 0.5f) + (puzzleHeight * cellHeight * 0.5f) * (puzzleHeight * cellHeight * 0.5f));
        //for (int i = 0; i < puzzleWidth; i++)
        //{
        //    for (int j = 0; j < puzzleHeight; j++)
        //    {
        //        int x1 = puzzleX1 + i * cellWidth;
        //        int y1 = puzzleY1 + j * cellHeight;
        //        if (allConnect)
        //            FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);
        //        else if (puzzleDSU.Find(GetCellID(i, j)) == startValue)
        //            FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);

        //        if (puzzleMap[i][j].value == cellInfo.NORMAL)
        //        {
        //            Vector3 pos = GetCellCenterPos(i, j);
        //            float startDis = Vector3.Distance(pos, startPos);

        //            int wallCount = (puzzleMap[i][j].U ? 0 : 1) + (puzzleMap[i][j].D ? 0 : 1) + (puzzleMap[i][j].L ? 0 : 1) + +(puzzleMap[i][j].R ? 0 : 1);
        //            if (wallCount == 3)
        //            {
        //                deadEnds.Add(new Vector2Int(i, j));
        //            }
        //            else if (dungeonEnemyManager && startDis > enemyMinDistanceToStart)
        //            {
        //                //print("diffAdd: " + startDis / maxDistance);
        //                dungeonEnemyManager.AddNormalPosition(pos, startDis/maxDistance);
        //            }

        //            if (normalGroup && !dungeonEnemyManager && startDis > enemyMinDistanceToStart)
        //            {
        //                if (Random.Range(0, 1.0f) < normalEnemyRate)
        //                {
        //                    GameObject egObj = BattleSystem.SpawnGameObj(normalGroup.gameObject, pos);
        //                    EnemyGroup eg = egObj.GetComponent<EnemyGroup>();
        //                    eg.isRandomEnemyTotal = true;
        //                    //TODO: 敵人的動態強度需要根據迷宮深淺來調整 ?
        //                    //eg.randomEnemyTotal = 4 + (j * (4 + 1) / puzzleHeight);
        //                }
        //            }
        //        }
        //    }
        //}

        //print("總共找到的終端:" + deadEnds.Count);
        //int expRewardCount = Mathf.Min(exploreRewardNum, deadEnds.Count);
        //OneUtility.Shuffle(deadEnds);
        //OneUtility.Shuffle(exploreRewards);     //因應獎勵可能更少的時候
        //for ( int i=0; i< deadEnds.Count; i++)
        //{
        //    Vector3 pos = GetCellCenterPos(deadEnds[i].x, deadEnds[i].y );
        //    if (i < expRewardCount)
        //    {
        //        BattleSystem.SpawnGameObj(exploreRewards[i], pos);
        //    }
        //    else if (dungeonEnemyManager)
        //    {
        //        if (Vector3.Distance(pos, startPos) > enemyMinDistanceToStart)
        //        {
        //            dungeonEnemyManager.AddNormalPosition(pos); //如果沒放寶，就可以放怪
        //        }
        //    }
        //}

        //if (dungeonEnemyManager)
        //{
        //    print("開始 Build dungeonEnemyManager !!");
        //    dungeonEnemyManager.BuildAllGameplay(dungeonEnemyDifficulty);
        //}

        //==== Big Room 的部份處理
        //ProcessBigRoomAndInitFinish();
        //bool isFinishPortalDone = false;
        //for (int i=0; i<rectList.Count; i++)
        //{
        //    RectInt rc = rectList[i];
        //    int x1 = puzzleX1 + rc.x * cellWidth;
        //    int y1 = puzzleY1 + rc.y * cellHeight;
        //    if (roomGroundTileGroup != null)
        //        theMap.FillValue(x1 + borderWidth, y1 + borderWidth,
        //            rc.width * cellWidth - borderWidth - borderWidth, rc.height * cellHeight - borderWidth - borderWidth, (int)MAP_TYPE.ROOM);
        //    else
        //        theMap.FillValue(x1 + borderWidth, y1 + borderWidth,
        //            rc.width * cellWidth - borderWidth - borderWidth, rc.height * cellHeight - borderWidth - borderWidth, (int)MAP_TYPE.GROUND);

        //    Vector3 pos = new Vector3(x1 + rc.width * cellWidth / 2, 0, y1 + rc.height * cellHeight / 2);
        //    if (bigRooms[i].gameplayRef)
        //    {
        //        GameObject o = BattleSystem.SpawnGameObj(bigRooms[i].gameplayRef, pos);
        //        if (portalAfterFirstRoomGamplay && i == 0)
        //        {
        //            MultiSpawner s = o.AddComponent<MultiSpawner>();
        //            s.AreaHeight = s.AreaWidth = 0;
        //            s.MaxNum = s.MinNum = 1;
        //            s.objRef = finishPortalRef;
        //            isFinishPortalDone = true;
        //        }

        //        //難度校正
        //        if (bigRooms[i].difficultyAdd != 0)
        //        {
        //            //print("=================難度校正: " + (1.0f + bigRooms[i].difficultyAdd));
        //            EnemyGroup[] epArray = o.GetComponentsInChildren<EnemyGroup>();
        //            foreach (EnemyGroup ep in epArray)
        //            {
        //                ep.difficulty = 1.0f + bigRooms[i].difficultyAdd;
        //            }
        //        }
        //    }
        //    else if (normalGroup)
        //    {
        //        GameObject egObj = BattleSystem.SpawnGameObj(normalGroup.gameObject, pos);
        //        EnemyGroup eg = egObj.GetComponent<EnemyGroup>();
        //        eg.isRandomEnemyTotal = true;
        //        //eg.randomEnemyTotal = rc.width * rc.height * 2;
        //        eg.randomEnemyTotal = normalEnemyNum * 2;
        //        //eg.height = rc.height * 4;
        //        //eg.width = rc.width * 4;
        //        eg.height *= 2;
        //        eg.width *= 2;
        //    }

        //    //Big Room 的牆面處理
        //    FillRoomWallColliders(rc);
        //}

        ////初始 Gameplay
        //if (initGampleyRef)
        //{
        //    BattleSystem.SpawnGameObj(initGampleyRef, startPos);
        //}

        ////破關門
        //if (finishPortalRef && !isFinishPortalDone)
        //    BattleSystem.SpawnGameObj(finishPortalRef, endPos);
    }



    protected void ProcessBigRoomAndInitFinish()
    {
        bool isFinishPortalDone = false;
        for (int i = 0; i < rectList.Count; i++)
        {
            RectInt rc = rectList[i];
            int x1 = puzzleX1 + rc.x * cellWidth;
            int y1 = puzzleY1 + rc.y * cellHeight;
            if (roomGroundTileGroup != null)
                theMap.FillValue(x1 + borderWidth, y1 + borderWidth,
                    rc.width * cellWidth - borderWidth - borderWidth, rc.height * cellHeight - borderWidth - borderWidth, (int)MAP_TYPE.ROOM);
            else
                theMap.FillValue(x1 + borderWidth, y1 + borderWidth,
                    rc.width * cellWidth - borderWidth - borderWidth, rc.height * cellHeight - borderWidth - borderWidth, (int)MAP_TYPE.GROUND);

            Vector3 pos = new Vector3(x1 + rc.width * cellWidth / 2, 0, y1 + rc.height * cellHeight / 2);
            if (i < bigRooms.Length && bigRooms[i].gameplayRef)
            {
                GameObject o = BattleSystem.SpawnGameObj(bigRooms[i].gameplayRef, pos);
                if (portalAfterFirstRoomGamplay && i == 0)
                {
                    MultiSpawner s = o.AddComponent<MultiSpawner>();
                    s.AreaHeight = s.AreaWidth = 0;
                    s.MaxNum = s.MinNum = 1;
                    s.objRef = finishPortalRef;
                    isFinishPortalDone = true;
                }

                //難度校正
                if (bigRooms[i].difficultyAdd != 0)
                {
                    //print("=================難度校正: " + (1.0f + bigRooms[i].difficultyAdd));
                    EnemyGroup[] epArray = o.GetComponentsInChildren<EnemyGroup>();
                    foreach (EnemyGroup ep in epArray)
                    {
                        ep.difficulty = 1.0f + bigRooms[i].difficultyAdd;
                    }
                }
            }
            else if (normalGroup)
            {
                GameObject egObj = BattleSystem.SpawnGameObj(normalGroup.gameObject, pos);
                EnemyGroup eg = egObj.GetComponent<EnemyGroup>();
                eg.isRandomEnemyTotal = true;
                //eg.randomEnemyTotal = rc.width * rc.height * 2;
                eg.randomEnemyTotal = normalEnemyNum * 2;
                //eg.height = rc.height * 4;
                //eg.width = rc.width * 4;
                eg.height *= 2;
                eg.width *= 2;
            }

            //Big Room 的牆面處理
            FillRoomWallColliders(rc);
        }

        //初始 Gameplay
        if (initGampleyRef)
        {
            BattleSystem.SpawnGameObj(initGampleyRef, startPos);
        }

        //破關門
        if (finishPortalRef && !isFinishPortalDone)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);
    }


    protected void ProcessNormalCells()
    {

        //==== 一般通道處理
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        //int startValue = puzzleDSU.Find(iStart);
        float enemyMinDistanceToStart = Mathf.Sqrt((cellHeight * cellHeight) + (cellWidth * cellWidth)) * noEnemyDistanceRate + 0.1f;
        //用來標記判斷敵人強度變化的值 TODO: 想辦法改用路徑
        float maxDistance = Mathf.Sqrt((puzzleWidth * cellWidth * 0.5f) * (puzzleWidth * cellWidth * 0.5f) + (puzzleHeight * cellHeight * 0.5f) * (puzzleHeight * cellHeight * 0.5f));
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                int x1 = puzzleX1 + i * cellWidth;
                int y1 = puzzleY1 + j * cellHeight;
                //if (allConnect)
                //    FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);
                //else if (puzzleDSU.Find(GetCellID(i, j)) == startValue)
                //    FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);
                FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);

                if (puzzleMap[i][j].value == cellInfo.NORMAL)
                {
                    Vector3 pos = GetCellCenterPos(i, j);
                    float startDis = Vector3.Distance(pos, startPos);

                    int wallCount = (puzzleMap[i][j].U ? 0 : 1) + (puzzleMap[i][j].D ? 0 : 1) + (puzzleMap[i][j].L ? 0 : 1) + +(puzzleMap[i][j].R ? 0 : 1);
                    if (wallCount == 3)
                    {
                        deadEnds.Add(new Vector2Int(i, j));
                    }
                    else if (dungeonEnemyManager && startDis > enemyMinDistanceToStart)
                    {
                        //print("diffAdd: " + startDis / maxDistance);
                        dungeonEnemyManager.AddNormalPosition(pos, startDis / maxDistance);
                    }

                    if (normalGroup && !dungeonEnemyManager && startDis > enemyMinDistanceToStart)
                    {
                        if (Random.Range(0, 1.0f) < normalEnemyRate)
                        {
                            GameObject egObj = BattleSystem.SpawnGameObj(normalGroup.gameObject, pos);
                            EnemyGroup eg = egObj.GetComponent<EnemyGroup>();
                            eg.isRandomEnemyTotal = true;
                            //TODO: 敵人的動態強度需要根據迷宮深淺來調整 ?
                            //eg.randomEnemyTotal = 4 + (j * (4 + 1) / puzzleHeight);
                        }
                    }
                }
            }
        }

        print("總共找到的終端:" + deadEnds.Count);
        int expRewardCount = Mathf.Min(exploreRewardNum, deadEnds.Count);
        OneUtility.Shuffle(deadEnds);
        OneUtility.Shuffle(exploreRewards);     //因應獎勵可能更少的時候
        for (int i = 0; i < deadEnds.Count; i++)
        {
            Vector3 pos = GetCellCenterPos(deadEnds[i].x, deadEnds[i].y);
            if (i < expRewardCount)
            {
                BattleSystem.SpawnGameObj(exploreRewards[i], pos);
            }
            else if (dungeonEnemyManager)
            {
                if (Vector3.Distance(pos, startPos) > enemyMinDistanceToStart)
                {
                    dungeonEnemyManager.AddNormalPosition(pos); //如果沒放寶，就可以放怪
                }
            }
        }

        if (dungeonEnemyManager)
        {
            print("開始 Build dungeonEnemyManager !!");
            dungeonEnemyManager.BuildAllGameplay(dungeonEnemyDifficulty);
        }
    }


    // ============================= 產生複數 Big Room 的判斷用 =============================================

    protected List<RectInt> CreateNonOverlappingRects(List<Vector2Int> sizes)
    {
        return CreateNonOverlappingRects(sizes, new RectInt(0, 0, puzzleWidth, puzzleHeight));
    }

    virtual protected List<RectInt> CreateNonOverlappingRects(List<Vector2Int> sizes, RectInt bound)
    {
        int maxAttempts = 1000;
        int retryCount = 0;
        int maxRetryCount = 100;
        List<RectInt> rects = new List<RectInt>();

        //TEST
        //rects.Add(new RectInt(2, 3, 3, 3));
        //rects.Add(new RectInt(3, 7, 3, 3));

        while (rects.Count < sizes.Count)
        {
            Vector2Int size = sizes[rects.Count];
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                int startX = Random.Range(bound.xMin, bound.xMax - size.x + 1);
                int startY = Random.Range(bound.yMin, bound.yMax - size.y + 1);

                RectInt newRect = new RectInt(startX, startY, size.x, size.y);

                if (!IsInvalidRect(rects, newRect))
                {
                    rects.Add(newRect);
                    break;
                }

                attempts++;
            }
            if (attempts >= maxAttempts)
            {
                retryCount++;
                if (retryCount > maxRetryCount)
                {
                    print("ERROR!!!! CreateNonOverlappingRects 一直退回也試不出來，放棄!!!!!!!!!!!!!!");
                    break;
                }
                if (rects.Count > 0)
                {
                    print("目前試不出來，回到上一層，目前 rects Count = " + rects.Count);
                    rects.RemoveAt(rects.Count - 1);
                }
            }
        }
        return rects;
    }

    //virtual protected bool IsOverlappingOrAdjacent(List<RectInt> rects, RectInt newRect)
    virtual protected bool IsInvalidRect(List<RectInt> rects, RectInt newRect)
    {
        foreach (RectInt existingRect in rects)
        {
            //if (existingRect.xMax + 1 >= newRect.x - 1 && existingRect.xMin - 1 <= newRect.xMax + 1 &&
            //    existingRect.yMax + 1 >= newRect.y - 1 && existingRect.yMin - 1 <= newRect.yMax + 1)
            if (existingRect.xMax >= newRect.xMin && existingRect.xMin <= newRect.xMax &&
                existingRect.yMax >= newRect.yMin && existingRect.yMin <= newRect.yMax)
            {
                //print("相交: " + newRect + "--" + existingRect);
                return true;
            }
        }

        return false;
    }


    protected void LoadMazeMap()
    {
        print("載入 PuzzleMap 資料!!" + loadedMapData.puzzleMapData);

        byte[] bData = System.Convert.FromBase64String(loadedMapData.puzzleMapData);
        if (bData.Length != puzzleWidth * puzzleHeight)
        {
            print("ERROR!!!! Size 不符 !!");
        }

        int i = 0;
        puzzleMap = new cellInfo[puzzleWidth][];
        for (int x = 0; x < puzzleWidth; x++)
        {
            puzzleMap[x] = new cellInfo[puzzleHeight];
            for (int y = 0; y < puzzleHeight; y++)
            {
                puzzleMap[x][y] = new cellInfo();
                puzzleMap[x][y].Decode((int)bData[i]);
                i++;
            }
        }

        //載入 Room 的資訊
        print("loadedMapData.rooms.Length: " + loadedMapData.rooms.Length);
        rectList = new List<RectInt>();
        for (i = 0; i < loadedMapData.rooms.Length; i++)
        {
            rectList.Add(loadedMapData.rooms[i].rect);
        }

        //ProcessNormalCells();
    }

    protected void SaveMap()
    {
        print("================= SaveMap");
        if (mapName == null || mapName == "")
            return;
        MapSaveMazeDungeon mapData = new MapSaveMazeDungeon();
        mapData.className = "MG_MazeDungeon";
        mapData.mapName = mapName;
        mapData.cellWidth = cellWidth;
        mapData.cellHeight = cellHeight;
        mapData.puzzleWidth = puzzleWidth;
        mapData.puzzleHeight = puzzleHeight;
        mapData.wallWidth = wallWidth;
        mapData.wallHeight = wallHeight;

        mapData.extendTerminal = extendTerminal;
        mapData.portalAfterFirstRoomGamplay = portalAfterFirstRoomGamplay;

        mapData.startPos = startPos;
        mapData.endPos = endPos;

        int i = 0;
        byte[] bData = new byte[puzzleHeight * puzzleWidth];
        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                //int ec = EncodeCell(puzzleMap[x][y]);
                int ec = puzzleMap[x][y].Encode();
                bData[i] = (byte)ec;
                i++;
            }
        }
        mapData.puzzleMapData = System.Convert.ToBase64String(bData);
        print("編碼結果!!" + mapData.puzzleMapData);

        //記錄 Rooms
        i = 0;
        if (rectList.Count > 0)
        {
            mapData.rooms = new MapSaveMazeDungeon.RoomSave[rectList.Count];
            foreach (RectInt rc in rectList)
            {
                mapData.rooms[i] = new MapSaveMazeDungeon.RoomSave();
                mapData.rooms[i].rect = rc;
                i++;
            }
        }

        GameSystem.GetPlayerData().SaveMap(mapName, mapData);

    }

    protected void LoadMap()
    {
        MapSaveDataBase mapDataBase = GameSystem.GetPlayerData().GetSavedMap(mapName);
        if (mapDataBase == null || mapDataBase.className != "MG_MazeDungeon")
        {
            print("MG_MazeDungeon.LoadMap: 沒有存檔資料");
            return;
        }

        print("MG_MazeDungeon.LoadMap: 找到存檔資料 !!!!");
        loadedMapData = (MapSaveMazeDungeon)mapDataBase;

        mapName = loadedMapData.mapName;
        cellWidth = loadedMapData.cellWidth;
        cellHeight = loadedMapData.cellHeight;
        puzzleWidth = loadedMapData.puzzleWidth;
        puzzleHeight = loadedMapData.puzzleHeight;
        wallWidth = loadedMapData.wallWidth;
        wallHeight = loadedMapData.wallHeight;

        extendTerminal = loadedMapData.extendTerminal;
        portalAfterFirstRoomGamplay = loadedMapData.portalAfterFirstRoomGamplay;

        startPos = loadedMapData.startPos;
        endPos = loadedMapData.endPos;
    }
}

