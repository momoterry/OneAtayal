using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Dynamic;

public class MG_MazeDungeon : MapGeneratorBase
{
    // 迷宮資料相關
    //public int cellSize = 4;
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
    public TileEdgeGroupDataBase wallEdgeTileGroup;
    public Tilemap groundTM;
    public Tilemap blockTM;

    // Big Room 用
    public Vector2Int[] roomSize;
    public int noRoomBuffer = 1;    //避免入口就遇到 Room 的緩衝

    //基底地圖相關 TODO: 希望獨立出去
    protected int mapWidth = 0;
    protected int mapHeight = 0;
    protected int borderWidth = 4;
    protected Vector3Int mapCenter;
    protected OneMap theMap = new OneMap();
    protected enum TILE_TYPE    //TODO: 要拿掉
    {
        GRASS = 4,
        DIRT = 5,
        BLOCK = 6,
        EDGE_VALUE = 100, //大於等於這個值就是 Edge
        GRASS_EDGE = 400,
        DIRT_EDGE = 500,
        BLOCK_EDGE = 600,
    }

    //Tile 資料
    //protected TileGroup groundTG;

    protected int bufferX = 0;
    protected int bufferY = 0;

    protected int puzzleX1;
    protected int puzzleY1;

    protected int iStart;
    protected int iEnd;
    protected Vector3 startPos;
    protected Vector3 endPos;

    //protected NavMeshAgent pcAgent;
    //protected int pcNaveAgentToSleep = -1;

    //List<Vector2Int> correctPathList = new List<Vector2Int>();

    protected class cellInfo
    {
        //public int ID;
        public bool U, D, L, R;
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

    private void Update()
    {
        //if (pcNaveAgentToSleep > 0)
        //{
        //    pcNaveAgentToSleep--;
        //    if (pcNaveAgentToSleep <= 0)
        //    {
        //        if (pcAgent)
        //        {
        //            pcAgent.enabled = true;
        //        }
        //        pcNaveAgentToSleep = -1;
        //    }
        //}
    }

    public override void BuildAll(int buildLevel = 1)
    {
        //groundTG = groundTileGroupData.GetTileGroup();

        PreCreateMap();

        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth);


        //主要地圖設計部份
        CreatMazeMap();

        //theMap.PrintMap();

        theMap.FillTileAll((int)TILE_TYPE.GRASS, groundTM, groundTM, groundTileGroup.GetTileGroup(), groundEdgeTileGroup.GetTileEdgeGroup(), false);
        theMap.FillTileAll((int)TILE_TYPE.GRASS, null, blockTM, null, wallEdgeTileGroup.GetTileEdgeGroup(), true);

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
            case (int)TILE_TYPE.GRASS:
                return new Color(0.5f, 0.5f, 0.5f);
            //case (int)TILE_TYPE.BLOCK:
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
        //int wallThick = 2;

        int x2 = x1 + width - wallWidth;
        int y2 = y1 + height - wallHeight;
        theMap.FillValue(x1, y1, width, height, (int)TILE_TYPE.GRASS);

        //theMap.SetValue(x1, y1, (int)TILE_TYPE.BLOCK);
        //theMap.SetValue(x1, y2, (int)TILE_TYPE.BLOCK);
        //theMap.SetValue(x2, y1, (int)TILE_TYPE.BLOCK);
        //theMap.SetValue(x2, y2, (int)TILE_TYPE.BLOCK);
        theMap.FillValue(x1, y1, wallWidth, wallHeight, (int)TILE_TYPE.BLOCK);
        theMap.FillValue(x1, y2, wallWidth, wallHeight, (int)TILE_TYPE.BLOCK);
        theMap.FillValue(x2, y1, wallWidth, wallHeight, (int)TILE_TYPE.BLOCK);
        theMap.FillValue(x2, y2, wallWidth, wallHeight, (int)TILE_TYPE.BLOCK);

        if (!cell.D)
        {
            theMap.FillValue(x1+ wallWidth, y1, width- wallWidth - wallWidth, wallHeight, (int)TILE_TYPE.BLOCK);
            //for (int x = x1 + 1; x < x2; x++)
            //    theMap.SetValue(x, y1, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.U)
        {
            theMap.FillValue(x1 + wallWidth, y2, width - wallWidth - wallWidth, wallHeight, (int)TILE_TYPE.BLOCK);
            //for (int x = x1 + 1; x < x2; x++)
            //    theMap.SetValue(x, y2, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.L)
        {
            theMap.FillValue(x1, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)TILE_TYPE.BLOCK);
            //for (int y = y1 + 1; y < y2; y++)
            //    theMap.SetValue(x1, y, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.R)
        {
            theMap.FillValue(x2, y1 + wallHeight, wallWidth, height - wallHeight - wallHeight, (int)TILE_TYPE.BLOCK);
            //for (int y = y1 + 1; y < y2; y++)
            //    theMap.SetValue(x2, y, (int)TILE_TYPE.BLOCK);
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

    protected List<RectInt> rectList;

    protected  void CreatMazeMap()
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
        rectList = CreateNonOverlappingRects(roomSize, new RectInt(0, noRoomBuffer, puzzleWidth, puzzleHeight - noRoomBuffer));

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
                    //roomWalls.Add(udWalls[x, rY - 1]);
                }
                if (rY + rH < puzzleHeight)
                {
                    wallList.Remove(udWalls[x, rY + rH - 1]);
                    //roomWalls.Add(udWalls[x, rY + rH - 1]);
                }
            }
            for (int y = rY; y < rY + rH; y++)
            {
                if (rX > 0)
                {
                    wallList.Remove(lrWalls[rX - 1, y]);
                    //roomWalls.Add(lrWalls[rX - 1, y]);
                }
                if (rX + rW < puzzleWidth)
                {
                    wallList.Remove(lrWalls[rX + rW - 1, y]);
                    //roomWalls.Add(lrWalls[rX + rW - 1, y]);
                }
            }
            //for (int c = 0; c < 1; c++)
            //{
            //    wallInfo w = roomWalls[Random.Range(0, roomWalls.Count)];
            //    ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
            //    puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
            //    roomWalls.Remove(w);
            //}
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
        foreach (RectInt rc in rectList)
        {
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
            //TODO: 每個 Room 各別設定開口數
            for (int c = 0; c < 2; c++)
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

        //== 緩衝區處理
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
            }
        }

        //==== Big Room 的部份處理
        foreach (RectInt rc in rectList)
        {
            theMap.FillValue(puzzleX1 + rc.x * cellWidth + borderWidth, puzzleY1 + rc.y * cellHeight + borderWidth,
                rc.width * cellWidth - borderWidth - borderWidth, rc.height * cellHeight - borderWidth - borderWidth, (int)TILE_TYPE.GRASS);
        }

        //破關門
        if (finishPortalRef)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);
    }

    protected List<RectInt> CreateNonOverlappingRects(Vector2Int[] sizes)
    {
        return CreateNonOverlappingRects(sizes, new RectInt(0, 0, puzzleWidth, puzzleHeight));
    }

    protected List<RectInt> CreateNonOverlappingRects(Vector2Int[] sizes, RectInt bound)
    {
        int maxAttempts = 1000;
        int retryCount = 0;
        int maxRetryCount = 100;
        List<RectInt>  rects = new List<RectInt>();

        //TEST
        //rects.Add(new RectInt(2, 3, 3, 3));
        //rects.Add(new RectInt(3, 7, 3, 3));

        while (rects.Count < sizes.Length)
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

