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

        theMap.FillTileAll((int)TILE_TYPE.GRASS, groundTM, groundTM, groundTileGroup.GetTileGroup(), groundEdgeTileGroup.GetTileEdgeGroup(), false);
        theMap.FillTileAll((int)TILE_TYPE.GRASS, null, blockTM, null, wallEdgeTileGroup.GetTileEdgeGroup(), true);

        theSurface2D.BuildNavMesh();

        MiniMap theMiniMap = BattleSystem.GetInstance().theBattleHUD.miniMap;
        if (theMiniMap)
        {
            theMiniMap.CreateMinMap(theMap, MyGetColorCB);
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
            //int loop = 0;
            //int wallTotal = wallList.Count;
            //while (loop < wallTotal)
            //{
            //    loop++;
            //    int rd = Random.Range(0, wallList.Count);
            //    wallInfo w = wallList[rd];
            //    if (puzzleDSU.Find(w.cell_ID_1) != puzzleDSU.Find(w.cell_ID_2)) //不要自體相連
            //    {
            //        ConnectCellsByID(w.cell_ID_1, w.cell_ID_2);
            //        puzzleDSU.Union(w.cell_ID_1, w.cell_ID_2);
            //    }
            //    wallList.Remove(w);

            //    if (puzzleDSU.Find(iStart) == puzzleDSU.Find(iEnd))
            //    {
            //        //print("發現大祕寶啦 !! Loop = " + (loop + 1));
            //        break;
            //    }
            //}
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

        //破關門
        if (finishPortalRef)
            BattleSystem.SpawnGameObj(finishPortalRef, endPos);

    }


}

