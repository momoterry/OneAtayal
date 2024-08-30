using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_HugeRoomMaze : MG_MazeOneBase
{
    [System.Serializable]
    public class BlockInfo
    {
        public enum BLOCK_TYPE
        {
            BIG_ROOM,
            MAZE,
        }
        public BLOCK_TYPE type;
        //public int width;  //如果是大 Room 時候，-1 表示用全大小
        public int blockHeight;
    }
    public BlockInfo[] blocks;

    protected bool extendTerminal = true;

    protected override void PresetMapInfo()
    {
        int totalHeight = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            totalHeight += blocks[i].blockHeight;
        }

        puzzleHeight = totalHeight;
        mazeDir = MAZE_DIR.DONW_TO_TOP;
        if (extendTerminal && loadedMapData == null)
        {
            puzzleHeight += 2;
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

        if (extendTerminal)
        {
            for (int i = 0; i < puzzleWidth; i++)
            {
                puzzleMap[i][0].value = CELL.INVALID;
                puzzleMap[i][puzzleHeight - 1].value = CELL.INVALID;
            }
            puzzleMap[puzzleStart.x][puzzleStart.y].value = CELL.NORMAL;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = CELL.NORMAL;
        }
    }

    protected override void CreatMazeMap()
    {
        //base.CreatMazeMap();
        int currStart = 0;
        Vector2Int blockStart = puzzleStart;
        if (extendTerminal)
        {
            currStart++;
            //ConnectCells(puzzleMap[puzzleStart.x][0], puzzleMap[puzzleStart.x][1], DIRECTION.U);
        }

        for (int i=0; i<blocks.Length; i++)
        {
            blockStart.y = currStart;

            if (blocks[i].type == BlockInfo.BLOCK_TYPE.BIG_ROOM)
            {
                CreateBigRoom(blocks[i], blockStart, (i + 1)/blocks.Length);
            }
            else if (blocks[i].type == BlockInfo.BLOCK_TYPE.MAZE)
            {
                CreateMaze(blocks[i], blockStart, (i + 1) / blocks.Length);
            }
            if (currStart > 0)
                ConnectCells(puzzleMap[blockStart.x][blockStart.y - 1], puzzleMap[blockStart.x][blockStart.y], DIRECTION.U);

            currStart += blocks[i].blockHeight;
        }

        //最後段尾相接
        if (extendTerminal)
        {
            //ConnectCells(puzzleMap[puzzleStart.x][0], puzzleMap[puzzleStart.x][1], DIRECTION.U);
            ConnectCells(puzzleMap[puzzleEnd.x][puzzleEnd.y-1], puzzleMap[puzzleEnd.x][puzzleEnd.y], DIRECTION.U);
        }
    }

    protected void CreateBigRoom(BlockInfo block, Vector2Int currStart, float mainRatio = 0)
    {
        for (int i=0; i<puzzleWidth; i++)
        {
            for (int j=currStart.y; j<currStart.y+block.blockHeight; j++)
            {
                puzzleMap[i][j].value = CELL.ROOM;
            }
        }

        if (createWallCollider && colliderRoot == null)
        {
            colliderRoot = new GameObject("Root_Colliders");
            colliderRoot.transform.parent = transform;
        }

        int blockWidth = puzzleWidth;   //TODO

        int width = blockWidth * cellWidth;
        int height = block.blockHeight * cellHeight;
        int x1 = puzzleX1 + (currStart.x - blockWidth/2) * cellWidth;
        int x2 = x1 + width;
        int y1 = puzzleY1 + currStart.y * cellHeight;
        int y2 = y1 + height;

        //門的位置
        int dx1 = puzzleX1 + currStart.x * cellWidth + (cellWidth-pathWidth)/2;
        int dx2 = dx1 + pathWidth;

        //print(x1 + " ---- " + y1);
        //開始填
        theMap.FillValue(x1 , y1, width, height, (int)MAP_TYPE.GROUND);

        theMap.FillValue(x1, y1, dx1 - x1, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(dx2, y1, x2 - dx2, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x1, y2 - wallHeight, dx1 - x1, wallHeight, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(dx2, y2 - wallHeight, x2 - dx2, wallHeight, (int)MAP_TYPE.BLOCK);

        theMap.FillValue(x1, y1, wallWidth, height, (int)MAP_TYPE.BLOCK);
        theMap.FillValue(x2 - wallWidth, y1, wallWidth, height, (int)MAP_TYPE.BLOCK);
        if (createWallCollider)
        {
            FillBlock(x1, y1, dx1 - x1 - wallBuffer, wallHeight);
            FillBlock(dx2 + wallBuffer, y1, x2 - dx2 - wallBuffer, wallHeight);
            FillBlock(x1, y2 - wallHeight, dx1 - x1 - wallBuffer, wallHeight);
            FillBlock(dx2 + wallBuffer, y2 - wallHeight, x2 - dx2 - wallBuffer, wallHeight);

            FillBlock(x1, y1, wallWidth, height);
            FillBlock(x2 - wallWidth, y1, wallWidth, height);
        }

        //設立 Cell
        CELL bigCell = new CELL();
        bigCell.value = CELL.NORMAL;
        bigCell.isMain = true;

        if (gameManager)
        {
            gameManager.AddRoom(GetCellCenterPos(currStart.x, currStart.y + block.blockHeight/2), width, height, bigCell, mainRatio, pathWidth, pathHeight);
        }
    }

    protected void CreateMaze(BlockInfo block, Vector2Int currStart, float mainRatio = 0)
    {
        //print("CreateMaze at: " + currStart);
        MazeCreator_BackTrace mc = new MazeCreator_BackTrace();
        Vector2Int blockStart = new Vector2Int(puzzleStart.x, 0);
        Vector2Int blockEnd = new Vector2Int(puzzleStart.x, block.blockHeight-1);
        MazeCreator_BackTrace.MyCell[][] maze = mc.CreateMaze(puzzleWidth, block.blockHeight, blockStart, blockEnd);

        for (int i = 0; i < puzzleWidth; i++) 
        {
            for (int j=0; j<block.blockHeight; j++)
            {
                CELL c = puzzleMap[i][j+ currStart.y];
                MazeCreator_BackTrace.MyCell cm = maze[i][j];
                c.D = cm.D;
                c.U = cm.U;
                c.R = cm.R;
                c.L = cm.L;
                if (cm.value == MazeCreator_BackTrace.MyCell.CELL_VALUE.INVALID)
                {
                    c.value = CELL.INVALID;
                }
                else
                {
                    c.isPath = true;
                }
                if (gameManager)
                {
                    gameManager.AddRoom(GetCellCenterPos(c.x, c.y), c, mainRatio);
                }
            }
        }
    }

    protected override void PreCalculateGameplayInfo()
    {

    }

    //===========================================================================================
    //通用 BackTrace 演算法
    //===========================================================================================
    public class MazeCreator_BackTrace
    {
        protected int puzzleWidth;
        protected int puzzleHeight;
        protected Vector2Int puzzleStart;
        protected Vector2Int puzzleEnd;

        public class MyCell :CELL_BASE
        {
            //public int x, y;
            //public bool U, D, L, R;
            public enum CELL_VALUE
            {
                NORMAL,
                INVALID,
            }
            public CELL_VALUE value;
        }

        protected MyCell[][] puzzleMap;

        protected List<MyCell> cellList = new List<MyCell>();
        protected int startDSU = 0;
        protected OneUtility.DisjointSetUnion puzzleDSU = new OneUtility.DisjointSetUnion();

        protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
        protected int GetCellX(int id) { return id % puzzleWidth; }
        protected int GetCellY(int id) { return id / puzzleWidth; }
        protected void ConnectCells(MyCell cFrom, MyCell cTo, DIRECTION toDir)
        {
            switch (toDir)
            {
                case DIRECTION.U:
                    cFrom.U = cTo.D = true;
                    break;
                case DIRECTION.D:
                    cFrom.D = cTo.U = true;
                    break;
                case DIRECTION.L:
                    cFrom.L = cTo.R = true;
                    break;
                case DIRECTION.R:
                    cFrom.R = cTo.L = true;
                    break;
            }
        }

        public MyCell[][] CreateMaze(int _width, int _height, Vector2Int _start, Vector2Int _end)
        {
            //print("CreateMaze: " + _start + " - " + _end);
            puzzleWidth = _width;
            puzzleHeight = _height;
            puzzleStart = _start;
            puzzleEnd = _end;

            puzzleMap = new MyCell[puzzleWidth][];
            for (int i = 0; i < puzzleWidth; i++)
            {
                puzzleMap[i] = new MyCell[puzzleHeight];
                for (int j = 0; j < puzzleHeight; j++)
                {
                    puzzleMap[i][j] = new MyCell();
                    puzzleMap[i][j].x = i;
                    puzzleMap[i][j].y = j;
                    puzzleMap[i][j].value = MyCell.CELL_VALUE.NORMAL;
                }
            }

            puzzleDSU.Init(puzzleHeight * puzzleWidth);

            startDSU = puzzleDSU.Find(GetCellID(puzzleStart.x, puzzleStart.y));
            cellList.Add(puzzleMap[puzzleStart.x][puzzleStart.y]);
            int count = 0;
            while (cellList.Count > 0)
            {
                DoOneCycle();
                count++;
            }
            //print("完成 Cycle " + count);

            return puzzleMap;
        }

        protected MyCell TryConnectRandomCell(MyCell cell)
        {
            List<DIRECTION> choices = new List<DIRECTION>();
            if (!cell.L && cell.x > 0 && puzzleMap[cell.x - 1][cell.y].value != MyCell.CELL_VALUE.INVALID)
            {
                if (puzzleDSU.Find(GetCellID(cell.x - 1, cell.y)) != startDSU)
                    choices.Add(DIRECTION.L);

            }
            if (!cell.D && cell.y > 0 && puzzleMap[cell.x][cell.y - 1].value != MyCell.CELL_VALUE.INVALID)
            {
                if (puzzleDSU.Find(GetCellID(cell.x, cell.y - 1)) != startDSU)
                    choices.Add(DIRECTION.D);

            }
            if (!cell.R && cell.x < puzzleWidth - 1 && puzzleMap[cell.x + 1][cell.y].value != MyCell.CELL_VALUE.INVALID)
            {
                if (puzzleDSU.Find(GetCellID(cell.x + 1, cell.y)) != startDSU)
                    choices.Add(DIRECTION.R);

            }
            if (!cell.U && cell.y < puzzleHeight - 1 && puzzleMap[cell.x][cell.y + 1].value != MyCell.CELL_VALUE.INVALID)
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
            MyCell toCell = null;
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
            MyCell cellToGo;
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

            MyCell nextCell = TryConnectRandomCell(cellToGo);
            if (nextCell != null)
            {
                //print("找到路了，++清單 " + cellList.Count);
                if (nextCell.x == puzzleEnd.x && nextCell.y == puzzleEnd.y)
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
}
