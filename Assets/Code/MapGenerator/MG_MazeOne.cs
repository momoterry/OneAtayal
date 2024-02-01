using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MG_MazeOne : MapGeneratorBase
{
    //public string mapName;              //用來識別存檔
    //地圖存檔資料
    //protected MapSaveMazeDungeon loadedMapData = null;  //如果有的話，是載入存檔的形式
    // 迷宮資料相關


    public int puzzleWidth = 6;
    public int puzzleHeight = 6;

    public int roomWidth = 12;
    public int roomHeight = 16;
    public int wallWidth = 4;
    public int wallHeight = 4;

    public int pathWidth = 4;
    public int pathHeight = 4;
    //public int fixDoorWidth = -1; // 如果值 > 0 就表非門口會縮小，通道較窄

    protected int cellWidth = 4;
    protected int cellHeight = 4;


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


    //Gameplay 用
    //[Header("Gameplay 設定")]
    public DungeonEnemyManagerBase dungeonEnemyManager; 
    //public EnemyGroup normalGroup;
    //public float normalEnemyRate = 0.2f;
    //protected int normalEnemyNum;
    protected float dungeonEnemyDifficulty = 1.0f;
    //public float noEnemyDistanceRate = 1.0f;    //以 Cell 對角長度為單位

    //public GameObject[] exploreRewards;
    //protected int exploreRewardNum;

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

        PresetByContinuousBattle();

        //if (mapName != null && mapName != "")
        //    LoadMap();  //先嘗試載入存檔，有的話更新地圖參數等

        //PreCreateMap();

        PresetMapInfo();    //各種內部參數的初始化，必須在讀取進度或連續戰鬥資訊等確認參數後進行

        InitPuzzleMap();


        //============================= 以下設置每個 Cell 的內容 ===========================================

        //主要迷宮結構的架設
        //if (loadedMapData != null)
        //{
        //    LoadMazeMap();
        //}
        //else
        {
            CreatMazeMap();
        }

        ProcessNormalCells();
        ProcessInitFinish();

        //============================= 以下開始舖設 Tiles ===========================================
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

        // ====================== 裝飾物件建立 =====================================
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
            theMiniMap.CreateMiniMap(theMap);
        }

        //載入已探索的資訊
        //LoadExploreMap();

        ////地圖存檔
        //SaveMap();
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

                print("根據資料修正了迷宮大小: " + puzzleWidth + " - " + puzzleHeight);

                if (cData.dungeonDifficulty > 0)
                {
                    dungeonEnemyDifficulty = cData.dungeonDifficulty;
                }
                ////print("DungeonEnemyDifficulty : " + dungeonEnemyDifficulty);
                if (cData.dungeonEnemyManager != null)
                {
                    GameObject o = Instantiate(cData.dungeonEnemyManager.gameObject);
                    o.transform.parent = gameObject.transform;
                    dungeonEnemyManager = o.GetComponent<DungeonEnemyManager>();
                }

                if (cData.initGameplayRef)
                {
                    initGampleyRef = cData.initGameplayRef;
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

    virtual protected void PresetMapInfo()
    {
        // =============== 各種內部參數設定
        cellWidth = roomWidth + wallWidth + wallWidth;
        cellHeight = roomHeight + wallHeight + wallHeight;
        pathWidth = pathWidth > roomWidth ? roomWidth : pathWidth;
        pathHeight = pathHeight > roomHeight ? roomHeight : pathHeight;
        if ((pathWidth) % 2 != (roomWidth % 2))
            pathWidth++;
        if ((pathHeight) % 2 != (roomHeight % 2))
            pathHeight++;

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

    protected void InitPuzzleMap() 
    {
        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth);

        // =============== 初始化 PuzzleMap
        puzzleMap = new cellInfo[puzzleWidth][];
        for (int i = 0; i < puzzleWidth; i++)
        {
            puzzleMap[i] = new cellInfo[puzzleHeight];
            for (int j = 0; j < puzzleHeight; j++)
            {
                puzzleMap[i][j] = new cellInfo();
            }
        }

    }

    protected void CreatMazeMap()
    {
        //==== Init
        puzzleDSU.Init(puzzleHeight * puzzleWidth);

        puzzleStart = new Vector2Int(puzzleWidth / 2, 0);
        puzzleEnd = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);

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
                FillCell(cStart, puzzleX1 + puzzleStart.x * cellWidth, puzzleY1 + (puzzleStart.y - 1) * cellHeight, cellWidth, cellHeight);
            }
            else
            {
                puzzleMap[puzzleStart.x][puzzleStart.y - 1].U = true;
                puzzleMap[puzzleStart.x][puzzleStart.y - 1].value = cellInfo.TERNIMAL;
            }

            if (puzzleEnd.y == (puzzleHeight - 1))
            {
                cellInfo cEnd = new cellInfo();
                cEnd.D = true;
                FillCell(cEnd, puzzleX1 + puzzleEnd.x * cellWidth, puzzleY1 + (puzzleEnd.y + 1) * cellHeight, cellWidth, cellHeight);
            }
            else
            {
                puzzleMap[puzzleEnd.x][puzzleEnd.y + 1].D = true;
                puzzleMap[puzzleEnd.x][puzzleEnd.y + 1].value = cellInfo.TERNIMAL;
            }

            puzzleMap[puzzleStart.x][puzzleStart.y].D = true;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].U = true;

            startPos.z -= cellHeight;
            endPos.z += cellHeight;
        }
        else
        {
            puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.TERNIMAL;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.TERNIMAL;
        }
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

        int wallGapWidth = 0;
        int wallGapHeight = 0;
        //if (fixDoorWidth > 0)

        wallGapWidth = (roomWidth - pathWidth) / 2;
        wallGapHeight = (roomHeight - pathHeight) / 2;


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
            if (wallGapWidth > 0)
            {
                theMap.FillValue(x1 + wallWidth, y1, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathWidth, y1, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x1, y1, wallWidth + wallGapWidth - wallBuffer, wallHeight - wallBuffer);   // 左下
                if (wallGapWidth > 0)
                {
                    FillBlock(x1 + wallWidth + wallGapWidth + pathWidth + wallBuffer, y1, wallGapWidth, wallHeight - wallBuffer);   // 左下
                }
            }
        }
        if (!cell.U)
        {
            theMap.FillValue(x1 + wallWidth, y2, width - wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1 + wallWidth - wallBuffer, y2 + wallBuffer, width - wallWidth + wallBuffer, wallHeight - wallBuffer);   //右上和上

        }
        else
        {
            if (wallGapWidth > 0)
            {
                theMap.FillValue(x1 + wallWidth, y2, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x1 + wallWidth + wallGapWidth + pathWidth, y2, wallGapWidth, wallHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x2 - wallGapWidth + wallBuffer, y2 + wallBuffer, wallWidth + wallGapWidth - wallBuffer, wallHeight - wallBuffer);   // 右上
                if (wallGapWidth > 0)
                {
                    FillBlock(x1 + wallWidth - wallBuffer, y2 + wallBuffer, wallGapWidth, wallHeight - wallBuffer);   // 右上
                }
            }
        }

        if (!cell.L)
        {
            theMap.FillValue(x1, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x1, y1 + wallHeight - wallBuffer, wallWidth - wallBuffer, height - wallHeight + wallBuffer); //左上和左
        }
        else
        {
            if (wallGapHeight > 0)
            {
                theMap.FillValue(x1, y1 + wallHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x1, y1 + wallHeight + wallGapHeight + pathHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x1, y2 + wallBuffer - wallGapHeight, wallWidth - wallBuffer, wallHeight + wallGapHeight - wallBuffer);    //左上
                if (wallGapHeight > 0)
                {
                    FillBlock(x1, y1 + wallHeight - wallBuffer, wallWidth - wallBuffer, wallGapHeight);    //左上
                }
            }
        }
        if (!cell.R)
        {
            theMap.FillValue(x2, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
            if (isFillBlock)
                FillBlock(x2 + wallBuffer, y1, wallWidth - wallBuffer, height - wallHeight + wallBuffer);  //右下和右
        }
        else
        {
            if (wallGapHeight > 0)
            {
                theMap.FillValue(x2, y1 + wallHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
                theMap.FillValue(x2, y1 + wallHeight + wallGapHeight + pathHeight, wallWidth, wallGapHeight, (int)MAP_TYPE.BLOCK);
            }
            if (isFillBlock)
            {
                FillBlock(x2 + wallBuffer, y1, wallWidth - wallBuffer, wallHeight + wallGapHeight - wallBuffer);   //右下
                if (wallGapHeight > 0)
                {
                    FillBlock(x2 + wallBuffer, y1 + wallHeight + wallGapHeight + pathHeight + wallBuffer, wallWidth - wallBuffer, wallGapHeight);   //右下
                }
            }
        }
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


    protected List<RectInt> rectList;

    protected Vector2Int puzzleStart, puzzleEnd;


    protected void ProcessInitFinish()
    {

        //初始 Gameplay
        if (initGampleyRef)
        {
            BattleSystem.SpawnGameObj(initGampleyRef, startPos);
        }

        //破關門
        if (finishPortalRef )
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);
    }


    protected void ProcessNormalCells()
    {
        //上下延伸區如果超過範圍的處理
        //==== 起終點上下延申處理
        if (extendTerminal)
        {
            //起始區處理
            if (puzzleStart.y == 0)
            {
                cellInfo cStart = new cellInfo();
                cStart.U = true;
                FillCell(cStart, puzzleX1 + puzzleStart.x * cellWidth, puzzleY1 + (puzzleStart.y - 1) * cellHeight, cellWidth, cellHeight);
                //FillCell(cStart, puzzleX1 + GetCellX(iStart) * cellWidth, puzzleY1 + (GetCellY(iStart) - 1) * cellHeight, cellWidth, cellHeight);
            }

            if (puzzleEnd.y == (puzzleHeight - 1))
            {
                cellInfo cEnd = new cellInfo();
                cEnd.D = true;
                FillCell(cEnd, puzzleX1 + puzzleEnd.x * cellWidth, puzzleY1 + (puzzleEnd.y + 1) * cellHeight, cellWidth, cellHeight);
            }
        }

        //==== 一般通道處理

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
            }
        }
    }

}


