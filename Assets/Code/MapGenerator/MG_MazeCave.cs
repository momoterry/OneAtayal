using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_MazeCave : MapGeneratorBase
{
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

    // Tile 資料相關
    public TileGroupDataBase groundTileGroup;
    public TileEdgeGroupDataBase groundEdgeTileGroup;
    public TileEdgeGroupDataBase wallEdgeTileGroup;     //應是 Ground 的外部 Edge，名字應該改掉
    public TileGroupDataBase blockTileGroup;
    public TileGroupDataBase defautTileGroup;
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
    }
    public BigRoomInfo[] bigRooms;
    public int noRoomBuffer = 1;    //避免入口就遇到 Room 的緩衝

    //Gameplay 用
    public EnemyGroup normalGroup;
    public GameObject[] exploreRewards;


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
        PreCreateMap();

        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth);


        //主要地圖設計部份
        CreatMazeMap();

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
            theMap.FillTileAll((int)MAP_TYPE.BLOCK, groundTM, blockTileGroup.GetTileGroup());


        if (wallEdgeTileGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, null, blockTM, null, wallEdgeTileGroup.GetTileEdgeGroup(), true, (int)MAP_TYPE.BLOCK);

        theSurface2D.BuildNavMesh();

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            theMiniMap.CreateMiniMap(theMap, MyGetColorCB);
        }
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

    protected void PreCreateMap()
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
    }

    protected void FillCell(cellInfo cell, int x1, int y1, int width, int height)
    {
        int x2 = x1 + width - wallWidth;
        int y2 = y1 + height - wallHeight;
        theMap.FillValue(x1, y1, width, height, (int)MAP_TYPE.GROUND);

        theMap.FillValue(x1, y1, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x1, y2, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2, y1, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2, y2, wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);

        if (!cell.D)
        {
            theMap.FillValue(x1+ wallWidth, y1, width- wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        }
        if (!cell.U)
        {
            theMap.FillValue(x1 + wallWidth, y2, width - wallWidth - wallWidth, wallHeight, (int)MAP_TYPE.BLOCK);
        }
        if (!cell.L)
        {
            theMap.FillValue(x1, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
        }
        if (!cell.R)
        {
            theMap.FillValue(x2, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)MAP_TYPE.BLOCK);
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
        //==== Init Connection Info
        wallInfo[,] lrWalls = new wallInfo[puzzleWidth - 1, puzzleHeight];
        wallInfo[,] udWalls = new wallInfo[puzzleWidth, puzzleHeight + 1];

        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                if (x < puzzleWidth - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x + 1, y));
                    wallList.Add(w);
                    lrWalls[x, y] = w;
                }
                if (y < puzzleHeight - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x, y + 1));
                    wallList.Add(w);
                    udWalls[x, y] = w;
                }
            }
        }

        // ==== 產生大 Room
        List<Vector2Int> sizeList = new List<Vector2Int>();
        for (int i = 0; i < bigRooms.Length; i++)
        {
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
        iStart = GetCellID(puzzleWidth / 2, 0);
        iEnd = GetCellID(puzzleWidth / 2, puzzleHeight - 1);

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
        for (int i=0; i<rectList.Count; i++)
        {
            RectInt rc = rectList[i];
            List<wallInfo> roomWalls = new List<wallInfo>();
            for (int x = rc.x; x < rc.xMax; x++)
            {
                if (rc.y > 0)
                {
                    roomWalls.Add(udWalls[x, rc.y - 1]);
                }
                if (rc.yMax < puzzleHeight)
                {
                    roomWalls.Add(udWalls[x, rc.yMax - 1]);
                }
            }
            for (int y = rc.y; y < rc.yMax; y++)
            {
                if (rc.x > 0)
                {
                    roomWalls.Add(lrWalls[rc.x - 1, y]);
                }
                if (rc.xMax < puzzleWidth)
                {
                    roomWalls.Add(lrWalls[rc.xMax - 1, y]);
                }
            }

            for (int c = 0; c < bigRooms[i].numDoor; c++)
            {
                wallInfo w = roomWalls[Random.Range(0, roomWalls.Count)];
                ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
                roomWalls.Remove(w);
            }
        }

        //==== Set up all cells
        puzzleX1 = mapCenter.x - (puzzleWidth * cellWidth / 2);
        puzzleY1 = mapCenter.y - (puzzleHeight * cellHeight / 2);

        startPos = new Vector3(puzzleX1 + GetCellX(iStart) * cellWidth + cellWidth / 2, 1, puzzleY1 + GetCellY(iStart) * cellHeight + cellHeight / 2);
        endPos = new Vector3(puzzleX1 + GetCellX(iEnd) * cellWidth + cellWidth / 2, 1, puzzleY1 + GetCellY(iEnd) * cellHeight + cellHeight / 2);

        //==== 緩衝區處理
        if (extendTerminal)
        {
            //起始區處理
            cellInfo cStart = new cellInfo();
            cellInfo cEnd = new cellInfo();
            cStart.U = true;
            cEnd.D = true;
            FillCell(cStart, puzzleX1 + GetCellX(iStart) * cellWidth, puzzleY1 + (GetCellY(iStart) - 1) * cellHeight, cellWidth, cellHeight);
            FillCell(cEnd, puzzleX1 + GetCellX(iEnd) * cellWidth, puzzleY1 + (GetCellY(iEnd) + 1) * cellHeight, cellWidth, cellHeight);
            puzzleMap[GetCellX(iStart)][GetCellY(iStart)].D = true;
            puzzleMap[GetCellX(iEnd)][GetCellY(iEnd)].U = true;

            startPos.z -= cellHeight;
            endPos.z += cellHeight;
        }

        //==== 一般通道處理
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        int startValue = puzzleDSU.Find(iStart);
        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                int x1 = puzzleX1 + i * cellWidth;
                int y1 = puzzleY1 + j * cellHeight;
                if (allConnect)
                    FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);
                else if (puzzleDSU.Find(GetCellID(i, j)) == startValue)
                    FillCell(puzzleMap[i][j], x1, y1, cellWidth, cellHeight);

                if (puzzleMap[i][j].value == cellInfo.NORMAL)
                {
                    //Vector3 pos = new Vector3(x1 + cellWidth/2, 0, y1 + cellHeight/2);
                    Vector3 pos = GetCellCenterPos(i, j);
                    if (normalGroup && Vector3.Distance(pos, startPos) > 20.0f)
                    {
                        if (Random.Range(0, 1.0f) < 0.2f)
                        {
                            GameObject egObj = BattleSystem.SpawnGameObj(normalGroup.gameObject, pos);
                            EnemyGroup eg = egObj.GetComponent<EnemyGroup>();
                            eg.isRandomEnemyTotal = true;
                            eg.randomEnemyTotal = 4 + (j * (4 + 1) / puzzleHeight);
                        }
                    }

                    int wallCount = (puzzleMap[i][j].U ? 0 : 1) + (puzzleMap[i][j].D ? 0 : 1) 
                        + (puzzleMap[i][j].L ? 0 : 1) + +(puzzleMap[i][j].R ? 0 : 1);
                    if (wallCount == 3)
                    {
                        deadEnds.Add(new Vector2Int(i,j));
                    }
                }
            }
        }
        print("總共找到的終端:" + deadEnds.Count);
        int expRewardCount = Mathf.Min(exploreRewards.Length, deadEnds.Count);
        OneUtility.Shuffle(deadEnds);
        for ( int i=0; i<expRewardCount; i++)
        {
            Vector3 pos = GetCellCenterPos(deadEnds[i].x, deadEnds[i].y );
            BattleSystem.SpawnGameObj(exploreRewards[i], pos);
        }

        //==== Big Room 的部份處理
        for (int i=0; i<rectList.Count; i++)
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
            if (bigRooms[i].gameplayRef)
            {
                BattleSystem.SpawnGameObj(bigRooms[i].gameplayRef, pos);
            }
            else if (normalGroup)
            {
                GameObject egObj = BattleSystem.SpawnGameObj(normalGroup.gameObject, pos);
                EnemyGroup eg = egObj.GetComponent<EnemyGroup>();
                eg.isRandomEnemyTotal = true;
                eg.randomEnemyTotal = rc.width * rc.height * 2;
                eg.height = rc.height * 4;
                eg.width = rc.width * 4;
            }
        }

        //破關門
        if (finishPortalRef)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);
    }


    // ============================= 產生複數 Big Room 的判斷用 =============================================

    protected List<RectInt> CreateNonOverlappingRects(List<Vector2Int> sizes)
    {
        return CreateNonOverlappingRects(sizes, new RectInt(0, 0, puzzleWidth, puzzleHeight));
    }

    protected List<RectInt> CreateNonOverlappingRects(List<Vector2Int> sizes, RectInt bound)
    {
        int maxAttempts = 1000;
        int retryCount = 0;
        int maxRetryCount = 100;
        List<RectInt>  rects = new List<RectInt>();

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

                if (!IsOverlappingOrAdjacent(rects, newRect))
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

    private bool IsOverlappingOrAdjacent(List<RectInt> rects, RectInt newRect)
    {
        foreach (RectInt existingRect in rects)
        {
            //if (existingRect.xMax + 1 >= newRect.x - 1 && existingRect.xMin - 1 <= newRect.xMax + 1 &&
            //    existingRect.yMax + 1 >= newRect.y - 1 && existingRect.yMin - 1 <= newRect.yMax + 1)
            if (existingRect.xMax >= newRect.xMin && existingRect.xMin <= newRect.xMax && 
                existingRect.yMax >= newRect.yMin && existingRect.yMin <= newRect.yMax )
            {
                //print("相交: " + newRect + "--" + existingRect);
                return true;
            }
        }

        return false;
    }

}

