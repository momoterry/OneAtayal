using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MG_MazeDungeon : MapGeneratorBase
{
    // 迷宮資料相關
    public int cellSize = 4;
    public int puzzleHeight = 6;
    public int puzzleWidth = 6;
    public bool allConnect = true;
    public bool extendTerminal = true;
    public GameObject finishPortalRef;

    // Tile 資料相關
    public TileGroupData groundTileGroup;
    public TileEdgeGroupDataBase groundEdgeTileGroup;
    public Tilemap groundTM;
    public Tilemap blockTM;

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

    protected NavMeshAgent pcAgent;
    protected int pcNaveAgentToSleep = -1;

    List<Vector2Int> correctPathList = new List<Vector2Int>();

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
        if (pcNaveAgentToSleep > 0)
        {
            pcNaveAgentToSleep--;
            if (pcNaveAgentToSleep <= 0)
            {
                if (pcAgent)
                {
                    pcAgent.enabled = true;
                }
                pcNaveAgentToSleep = -1;
            }
        }
    }

    public override void BuildAll(int buildLevel = 1)
    {
        //groundTG = groundTileGroupData.GetTileGroup();

        PreCreateMap();

        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth);


        //主要地圖設計部份
        CreatMazeMap();

        //theMap.PrintMap();

        //theMap.FillTileAll((int)TILE_TYPE.GRASS, groundTM, groundTG.baseTile);
        theMap.FillTileAll((int)TILE_TYPE.GRASS, groundTM, blockTM, groundTileGroup.GetTileGroup(), groundEdgeTileGroup.GetTileEdgeGroup(), false);

        theSurface2D.BuildNavMesh();
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
        mapHeight = (puzzleHeight + bufferY + bufferY) * cellSize;  //加入上下緩衝
        mapWidth = (puzzleWidth + bufferX + bufferX) * cellSize;
        mapCenter.y = puzzleHeight * cellSize / 2 - (cellSize / 2);
        if (extendTerminal)
            mapCenter.y += cellSize;

        if (puzzleWidth % 2 == 0)
        {
            mapCenter.x = -cellSize / 2;
        }
    }

    protected void FillCell(cellInfo cell, int x1, int y1, int width, int height)
    {
        //print("FillCell: " + x1 + ", " + y1 + "(" + width + ", " + height + ")");
        int x2 = x1 + width - 1;
        int y2 = y1 + height - 1;
        theMap.FillValue(x1, y1, width, height, (int)TILE_TYPE.GRASS);

        theMap.SetValue(x1, y1, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x1, y2, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x2, y1, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x2, y2, (int)TILE_TYPE.BLOCK);
        if (!cell.D)
        {
            for (int x = x1 + 1; x < x2; x++)
                theMap.SetValue(x, y1, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.U)
        {
            for (int x = x1 + 1; x < x2; x++)
                theMap.SetValue(x, y2, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.L)
        {
            for (int y = y1 + 1; y < y2; y++)
                theMap.SetValue(x1, y, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.R)
        {
            for (int y = y1 + 1; y < y2; y++)
                theMap.SetValue(x2, y, (int)TILE_TYPE.BLOCK);
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

    //TODO: 這個要換掉
    //protected void FillSquareInMap(int value, int x1, int y1, int width, int height)
    //{
    //    for (int x = x1; x < x1 + width; x++)
    //    {
    //        for (int y = y1; y < y1 + height; y++)
    //        {
    //            theMap.SetValue(x, y, value);
    //        }
    //    }
    //}

    //protected void MarkCellbyID(int _id)
    //{
    //    //int puzzleX1 = mapCenter.x - (puzzleWidth * cellSize / 2);
    //    //int puzzleY1 = mapCenter.y - (puzzleHeight * cellSize / 2);
    //    int x1 = GetCellX(_id) * cellSize + puzzleX1;
    //    int y1 = GetCellY(_id) * cellSize + puzzleY1;
    //    //FillSquareInMap((int)TILE_TYPE.DIRT, new Vector3Int(x1, y1, 0), cellSize, cellSize);
    //    FillSquareInMap((int)TILE_TYPE.DIRT, x1, y1, cellSize, cellSize);
    //}

    protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
    protected int GetCellX(int id) { return id % puzzleWidth; }
    protected int GetCellY(int id) { return id / puzzleWidth; }

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
        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                if (x < puzzleWidth - 1)
                    wallList.Add(new wallInfo(GetCellID(x, y), GetCellID(x + 1, y)));
                if (y < puzzleHeight - 1)
                    wallList.Add(new wallInfo(GetCellID(x, y), GetCellID(x, y + 1)));
            }
        }

        //==== 開始隨機連結 !!
        iStart = GetCellID(puzzleWidth / 2, 0);
        iEnd = GetCellID(puzzleWidth / 2, puzzleHeight - 1);

        int loop = 0;
        int wallTotal = wallList.Count;
        while (loop < wallTotal)
        {
            loop++;
            int rd = Random.Range(0, wallList.Count);
            wallInfo w = wallList[rd];
            if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //不要自體相連
            {
                ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
                puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
            }
            wallList.Remove(w);

            if (!allConnect && (puzzleDSU.Find(iStart) == puzzleDSU.Find(iEnd)))
            {
                print("發現大祕寶啦 !! Loop = " + (loop + 1));
                break;
            }
        }

        //==== Set up all cells
        puzzleX1 = mapCenter.x - (puzzleWidth * cellSize / 2);
        puzzleY1 = mapCenter.y - (puzzleHeight * cellSize / 2);

        //MarkCellbyID(iStart);
        //MarkCellbyID(iEnd);
        startPos = new Vector3(puzzleX1 + GetCellX(iStart) * cellSize + cellSize / 2, 1, puzzleY1 + GetCellY(iStart) * cellSize + cellSize / 2);
        endPos = new Vector3(puzzleX1 + GetCellX(iEnd) * cellSize + cellSize / 2, 1, puzzleY1 + GetCellY(iEnd) * cellSize + cellSize / 2);

        //== 緩衝區處理
        if (extendTerminal)
        {
            //int bufferSizeY = bufferY * cellSize;
            //int bufferSizeX = bufferX * cellSize;
            //FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x - (mapWidth / 2), mapCenter.y - (mapHeight / 2), mapWidth, bufferSizeY);
            //FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x - (mapWidth / 2), mapCenter.y + (mapHeight / 2) - bufferSizeY, mapWidth, bufferSizeY);
            //FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x - (mapWidth / 2), mapCenter.y - (mapHeight / 2), bufferSizeX, mapHeight);
            //FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter.x + (mapWidth / 2) - bufferSizeX, mapCenter.y - (mapHeight / 2), bufferSizeX, mapHeight);

            //起始區處理
            cellInfo cStart = new cellInfo();
            cellInfo cEnd = new cellInfo();
            cStart.U = true;
            cEnd.D = true;
            //FillSquareInMap((int)TILE_TYPE.DIRT, puzzleX1 + GetCellX(iStart) * cellSize, puzzleY1 + (GetCellY(iStart) - 1) * cellSize, cellSize, cellSize);
            FillCell(cStart, puzzleX1 + GetCellX(iStart) * cellSize, puzzleY1 + (GetCellY(iStart) - 1) * cellSize, cellSize, cellSize);
            //FillSquareInMap((int)TILE_TYPE.DIRT, puzzleX1 + GetCellX(iEnd) * cellSize, puzzleY1 + (GetCellY(iEnd) + 1) * cellSize, cellSize, cellSize);
            FillCell(cEnd, puzzleX1 + GetCellX(iEnd) * cellSize, puzzleY1 + (GetCellY(iEnd) + 1) * cellSize, cellSize, cellSize);
            puzzleMap[GetCellX(iStart)][GetCellY(iStart)].D = true;
            puzzleMap[GetCellX(iEnd)][GetCellY(iEnd)].U = true;

            startPos.z -= cellSize;
            endPos.z += cellSize;
        }

        for (int i = 0; i < puzzleWidth; i++)
        {
            for (int j = 0; j < puzzleHeight; j++)
            {
                int x1 = puzzleX1 + i * cellSize;
                int y1 = puzzleY1 + j * cellSize;
                FillCell(puzzleMap[i][j], x1, y1, cellSize, cellSize);
            }
        }

        //破關門
        if (finishPortalRef)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);

    }


}

