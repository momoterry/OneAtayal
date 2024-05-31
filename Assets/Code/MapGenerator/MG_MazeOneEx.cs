using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//========================================================
//MazeOneEx: 繼承了 MaxeOne，可以改變入口方向並延申
//========================================================

public class MG_MazeOneEx : MG_MazeOne
{
    public bool extendTerminal = true;

    //public enum MAZE_DIR
    //{
    //    DONW_TO_TOP,
    //    TOP_TO_DOWN,
    //    LEFT_TO_RIGHT,
    //    RIGHT_TO_LEFT,
    //}
    //public MAZE_DIR mazeDir = MAZE_DIR.DONW_TO_TOP;
    protected override void PresetMapInfo()
    {

        if (mazeDir == MAZE_DIR.INSIDE_OUT)
        {
            print("ERROR!!! MG_MazeOneEx 不支援的方向類型 INSIDE_OUT");
            mazeDir = MAZE_DIR.DONW_TO_TOP;
        }
        if (extendTerminal &&　loadedMapData == null)
        {
            if (mazeDir == MAZE_DIR.DONW_TO_TOP || mazeDir == MAZE_DIR.TOP_TO_DOWN)
                puzzleHeight += 2;
            else if (mazeDir == MAZE_DIR.LEFT_TO_RIGHT || mazeDir == MAZE_DIR.RIGHT_TO_LEFT)
                puzzleWidth += 2;
        }
        base.PresetMapInfo();
    }

    protected override void InitPuzzleMap()
    {
        base.InitPuzzleMap();
        if (mazeDir == MAZE_DIR.DONW_TO_TOP)
        {
            puzzleStart = new Vector2Int(puzzleWidth / 2, 0);
            puzzleEnd = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);
            BattleSystem.GetInstance().initPlayerDirAngle = 0;
        }
        else if (mazeDir == MAZE_DIR.TOP_TO_DOWN)
        {
            puzzleStart = new Vector2Int(puzzleWidth / 2, puzzleHeight - 1);
            puzzleEnd = new Vector2Int(puzzleWidth / 2, 0);
            BattleSystem.GetInstance().initPlayerDirAngle = 180;
        }
        else if (mazeDir == MAZE_DIR.LEFT_TO_RIGHT)
        {
            puzzleStart = new Vector2Int(0, puzzleHeight / 2);
            puzzleEnd = new Vector2Int(puzzleWidth - 1, puzzleHeight / 2);
            BattleSystem.GetInstance().initPlayerDirAngle = 90;
        }
        else if (mazeDir == MAZE_DIR.RIGHT_TO_LEFT)
        {
            puzzleStart = new Vector2Int(puzzleWidth - 1, puzzleHeight / 2);
            puzzleEnd = new Vector2Int(0, puzzleHeight / 2);
            BattleSystem.GetInstance().initPlayerDirAngle = -90;
        }

        if (extendTerminal)
        {
            if (mazeDir == MAZE_DIR.DONW_TO_TOP || mazeDir == MAZE_DIR.TOP_TO_DOWN)
            {
                for (int i = 0; i < puzzleWidth; i++)
                {
                    puzzleMap[i][0].value = CELL.INVALID;
                    puzzleMap[i][puzzleHeight - 1].value = CELL.INVALID;
                }
            }
            else if (mazeDir == MAZE_DIR.LEFT_TO_RIGHT || mazeDir == MAZE_DIR.RIGHT_TO_LEFT)
            {
                for (int i = 0; i < puzzleHeight; i++)
                {
                    puzzleMap[0][i].value = CELL.INVALID;
                    puzzleMap[puzzleWidth - 1][i].value = CELL.INVALID;
                }
            }
            puzzleMap[puzzleStart.x][puzzleStart.y].value = CELL.NORMAL;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.NORMAL;
        }
    }
}

//========================================================
//MazeOne: 使用 DisjointSet 和 Wall 的資訊來連結的迷宮
//========================================================

public class MG_MazeOne : MG_MazeOneBase
{
    //public enum MAZE_DIR
    //{
    //    DONW_TO_TOP,
    //    TOP_TO_DOWN,
    //    LEFT_TO_RIGHT,
    //    RIGHT_TO_LEFT,

    //    INSIDE_OUT,     //從中間往外走，保留給 Cave 型迷宮用
    //}
    //public MAZE_DIR mazeDir = MAZE_DIR.DONW_TO_TOP;

    public enum MAZE_ALGORITHM
    {
        RANDOM_WALL,
        BACK_TRACE,
    }
    public MAZE_ALGORITHM ChooseAlgorithm = MAZE_ALGORITHM.RANDOM_WALL;

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


    protected OneUtility.DisjointSetUnion puzzleDSU = new OneUtility.DisjointSetUnion();

    override protected void CreatMazeMap()
    {
        //==== Init
        //DisjointSetUnion puzzleDSU = new DisjointSetUnion();
        puzzleDSU.Init(puzzleHeight * puzzleWidth);

        switch (ChooseAlgorithm)
        {
            case MAZE_ALGORITHM.RANDOM_WALL:
                CreateMazeByRandomWall();
                break;
            case MAZE_ALGORITHM.BACK_TRACE:
                CreateMazeByBackTrace();
                break;
        }

    }

    //===========================================================================================
    // 隨機牆拆除演算法
    //===========================================================================================
    protected void CreateMazeByRandomWall()
    {
        List<wallInfo> wallList = new List<wallInfo>();

        //==== Init Connection Info
        wallInfo[,] lrWalls = new wallInfo[puzzleWidth - 1, puzzleHeight];
        wallInfo[,] udWalls = new wallInfo[puzzleWidth, puzzleHeight + 1];

        for (int x = 0; x < puzzleWidth; x++)
        {
            for (int y = 0; y < puzzleHeight; y++)
            {
                bool addToWallList = true;
                if (puzzleMap[x][y].value == CELL.INVALID)
                    addToWallList = false;

                if (x < puzzleWidth - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x + 1, y));
                    if (addToWallList && puzzleMap[x + 1][y].value != CELL.INVALID)
                        wallList.Add(w);
                    lrWalls[x, y] = w;
                }
                if (y < puzzleHeight - 1)
                {
                    wallInfo w = new wallInfo(GetCellID(x, y), GetCellID(x, y + 1));
                    if (addToWallList && puzzleMap[x][y + 1].value != CELL.INVALID)
                        wallList.Add(w);
                    udWalls[x, y] = w;
                }
            }
        }


        //==== 開始隨機連結 !!
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

    //===========================================================================================
    //BackTrace 演算法
    //===========================================================================================
    protected List<CELL> cellList = new List<CELL>();
    protected int startDSU = 0;

    protected void CreateMazeByBackTrace()
    {
        startDSU = puzzleDSU.Find(GetCellID(puzzleStart.x, puzzleStart.y));
        cellList.Add(puzzleMap[puzzleStart.x][puzzleStart.y]);
        while (cellList.Count > 0)
        {
            DoOneCycle();
        }
    }

    protected CELL TryConnectRandomCell(CELL cell)
    {
        List<DIRECTION> choices = new List<DIRECTION>();
        if (!cell.L && cell.x > 0 && puzzleMap[cell.x - 1][cell.y].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x - 1, cell.y)) != startDSU)
                choices.Add(DIRECTION.L);

        }
        if (!cell.D && cell.y > 0 && puzzleMap[cell.x][cell.y - 1].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x, cell.y - 1)) != startDSU)
                choices.Add(DIRECTION.D);

        }
        if (!cell.R && cell.x < puzzleWidth - 1 && puzzleMap[cell.x + 1][cell.y].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x + 1, cell.y)) != startDSU)
                choices.Add(DIRECTION.R);

        }
        if (!cell.U && cell.y < puzzleHeight - 1 && puzzleMap[cell.x][cell.y + 1].value != CELL.INVALID)
        {
            if (puzzleDSU.Find(GetCellID(cell.x, cell.y + 1)) != startDSU)
                choices.Add(DIRECTION.U);
        }

        if (choices.Count == 0)
        {
            //print("找到底囉!!");
            return null;
        }

        DIRECTION dir = choices[Random.Range(0, choices.Count)];
        CELL toCell = null;
        int toDSU = -1;
        switch (dir)
        {
            case DIRECTION.U:
                toCell = puzzleMap[cell.x][cell.y + 1];
                toDSU = GetCellID(cell.x, cell.y + 1);
                break;
            case DIRECTION.D:
                toCell = puzzleMap[cell.x][cell.y - 1];
                toDSU = GetCellID(cell.x, cell.y - 1);

                break;
            case DIRECTION.L:
                toCell = puzzleMap[cell.x - 1][cell.y];
                toDSU = GetCellID(cell.x - 1, cell.y);

                break;
            case DIRECTION.R:
                toCell = puzzleMap[cell.x + 1][cell.y];
                toDSU = GetCellID(cell.x + 1, cell.y);

                break;
        }
        ConnectCells(cell, toCell, dir);
        puzzleDSU.Union(startDSU, toDSU);

        return toCell;
    }

    protected bool gotFinal = false;
    protected bool DoOneCycle()
    {
        CELL cellToGo;
        if (gotFinal)
        {
            //cellToGo = cellList[0];
            cellToGo = cellList[Random.Range(0, cellList.Count)];
            //cellToGo = cellList[cellList.Count - 1];
        }
        else
        {
            //cellToGo = cellList[0];
            //cellToGo = cellList[Random.Range(0, cellList.Count)];
            cellToGo = cellList[cellList.Count - 1];
        }

        CELL nextCell = TryConnectRandomCell(cellToGo);
        if (nextCell != null)
        {
            //print("找到路了，++清單 " + cellList.Count);
            if (!FinishAtDeepest && nextCell.x == puzzleEnd.x && nextCell.y == puzzleEnd.y)
            {
                gotFinal = true;
                //print("連到終點了，改變 gotFinal => " + gotFinal);
                //cellList = List<CELL>.re cellList
            }
            else
            {
                cellList.Add(nextCell);     //無論如何終點不要被加到 cellList，不要再另外連出去
            }
            return true;
        }
        else
        {
            //print("沒路可走，--清單 " + cellList.Count);
            cellList.Remove(cellToGo);
            return false;
        }
    }
}

