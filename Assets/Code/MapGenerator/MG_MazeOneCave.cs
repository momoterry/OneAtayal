using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MG_MazeOneCave : MG_MazeOne
{
    public bool extendTerminal = false;

    //public enum MAZE_DIR
    //{
    //    DONW_TO_TOP,
    //    TOP_TO_DOWN,
    //    LEFT_TO_RIGHT,
    //    RIGHT_TO_LEFT,

    //    INSIDE_OUT,     //從中間往外走
    //}
    //public MAZE_DIR mazeDir = MAZE_DIR.INSIDE_OUT;

    protected RandamWalkerMap theRWMap = null;
    protected Vector2Int rwMapShift = Vector2Int.zero;

    protected override void PresetMapInfo()
    {
        //嘗試先計算 RWalkerMap
        theRWMap = new RandamWalkerMap();
        theRWMap.blockFillRatio = blockFillRatio;
        theRWMap.CreateRandomWalkerMap(puzzleWidth, puzzleHeight);

        //根據計算結果重新校正地圖大小
        rwMapShift = new Vector2Int(-theRWMap.xMin, -theRWMap.yMin);
        puzzleWidth = theRWMap.xMax - theRWMap.xMin + 1;
        puzzleHeight = theRWMap.yMax - theRWMap.yMin + 1;


        if (extendTerminal)
        {
            if (mazeDir == MAZE_DIR.DONW_TO_TOP || mazeDir == MAZE_DIR.TOP_TO_DOWN)
            {
                puzzleHeight += 2;
                rwMapShift.y++;
            }
            else if (mazeDir == MAZE_DIR.LEFT_TO_RIGHT || mazeDir == MAZE_DIR.RIGHT_TO_LEFT)
            {
                puzzleWidth += 2;
                rwMapShift.x++;
            }
        }
        base.PresetMapInfo();
    }

    protected override void InitPuzzleMap()
    {
        base.InitPuzzleMap();

        //先清空預設
        for (int i=0; i<puzzleWidth; i++)
        {
            for (int j=0; j<puzzleHeight; j++)
            {
                puzzleMap[i][j].value = cellInfo.INVALID;
            }
        }

        //print("rwMapShift: " + rwMapShift);
        for (int i = 0; i < theRWMap.rwWidth; i++)
        {
            for (int j = 0; j < theRWMap.rwHeight; j++)
            {
                if (theRWMap.rwMap[i, j] != 0)
                {
                    //print("設定 " + (i+rwMapShift.x) + ", " + (j + rwMapShift.y));
                    puzzleMap[i + rwMapShift.x][j + rwMapShift.y].value = cellInfo.NORMAL;
                }
            }
        }

        switch (mazeDir)
        {
            case MAZE_DIR.DONW_TO_TOP:
                puzzleStart = theRWMap.downMost[Random.Range(0, theRWMap.downMost.Count)] + rwMapShift;
                puzzleEnd = theRWMap.topMost[Random.Range(0, theRWMap.topMost.Count)] + rwMapShift;
                BattleSystem.GetInstance().initPlayerDirAngle = 0;
                break;
            case MAZE_DIR.TOP_TO_DOWN:
                puzzleStart = theRWMap.topMost[Random.Range(0, theRWMap.topMost.Count)] + rwMapShift;
                puzzleEnd = theRWMap.downMost[Random.Range(0, theRWMap.downMost.Count)] + rwMapShift;
                BattleSystem.GetInstance().initPlayerDirAngle = 180;
                break;
            case MAZE_DIR.LEFT_TO_RIGHT:
                puzzleStart = theRWMap.leftMost[Random.Range(0, theRWMap.leftMost.Count)] + rwMapShift;
                puzzleEnd = theRWMap.rightMost[Random.Range(0, theRWMap.rightMost.Count)] + rwMapShift;
                BattleSystem.GetInstance().initPlayerDirAngle = 90;
                break;
            case MAZE_DIR.RIGHT_TO_LEFT:
                puzzleStart = theRWMap.rightMost[Random.Range(0, theRWMap.rightMost.Count)] + rwMapShift;
                puzzleEnd = theRWMap.leftMost[Random.Range(0, theRWMap.leftMost.Count)] + rwMapShift;
                BattleSystem.GetInstance().initPlayerDirAngle = -90;
                break;
            default:
                puzzleStart = theRWMap.vStart + rwMapShift;
                puzzleEnd = theRWMap.vEnd + rwMapShift;
                break;
        }

        if (extendTerminal)
        {
            switch (mazeDir)
            {
                case MAZE_DIR.DONW_TO_TOP:
                    puzzleStart.y--;
                    puzzleEnd.y++;
                    break;
                case MAZE_DIR.TOP_TO_DOWN:
                    puzzleStart.y++;
                    puzzleEnd.y--;
                    break;
                case MAZE_DIR.LEFT_TO_RIGHT:
                    puzzleStart.x--;
                    puzzleEnd.x++;
                    break;
                case MAZE_DIR.RIGHT_TO_LEFT:
                    puzzleStart.x++;
                    puzzleEnd.x--;
                    break;
            }
            puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.NORMAL;
            puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.NORMAL;

        }
    }

    ////========================== RandomWalker 參數 =============================
    public float blockFillRatio = 0.35f;

}

public class RandamWalkerMap
{
    //========================== RandomWalker 參數 =============================
    public float blockFillRatio = 0.35f;
    public int maxWalkers = 6;
    public int initWalkerNum = 1;
    public float changeDirRatio = 0.2f;
    public float deadRatio = 0.15f;
    public float cloneRatio = 0.2f;
    //========================== Random Walker 演算法 =============================
    protected List<RandomWalker> walkerList = new List<RandomWalker>();
    protected int blockNum = 0;
    protected int blockNumMax;
    public int[,] rwMap;
    public int rwWidth, rwHeight;
    protected int rwXMin, rwXMax, rwYMin, rwYMax;
    public Vector2Int vStart;
    public Vector2Int vEnd;
    protected int endDist;
    //========================== 邊界值計算用 ====================================
    public List<Vector2Int> leftMost = new List<Vector2Int>();
    public List<Vector2Int> rightMost = new List<Vector2Int>();
    public List<Vector2Int> topMost = new List<Vector2Int>();
    public List<Vector2Int> downMost = new List<Vector2Int>();
    public int xMin = int.MaxValue, yMin = int.MaxValue;
    public int xMax = int.MinValue, yMax = int.MinValue;

    public void CreateRandomWalkerMap(int mapWidth, int mapHeight)//, Vector2Int _vStart)//, ref Vector2Int _vEnd)
    {
        rwWidth = mapWidth;
        rwHeight = mapHeight;
        rwMap = new int[rwWidth, rwHeight];
        rwXMin = rwYMin = 0;
        rwXMax = rwWidth - 1;
        rwYMax = rwHeight - 1;
        vStart = new Vector2Int(rwWidth/2, rwHeight/2);
        vEnd = vStart;

        blockNumMax = (int)(rwWidth * rwHeight * blockFillRatio);

        for (int i = 0; i < initWalkerNum; i++)
        {
            RandomWalker w = new RandomWalker(vStart);
            walkerList.Add(w);
        }

        endDist = 0;
        int step = 0;
        while (blockNum < blockNumMax && step < 10000)
        {
            UpdateWalkers();
            step++;
        }
        //print("RandomWalker Total Block:" + blockNum);
        //_vEnd = vEnd;

    }

    protected void CheckTernima(Vector2Int pos)
    {
        if (pos.x <= xMin)  //為最左邊端點 
        {
            if (pos.x < xMin)
                leftMost.Clear();
            leftMost.Add(pos);
            xMin = pos.x;
        }
        if (pos.x >= xMax)  //為最右邊端點 
        {
            if (pos.x > xMax)
                rightMost.Clear();
            rightMost.Add(pos);
            xMax = pos.x;
        }
        if (pos.y <= yMin)  //為最下面端點
        {
            if (pos.y < yMin)
                downMost.Clear();
            downMost.Add(pos);
            yMin = pos.y;
        }
        if (pos.y >= yMax)  //為最上面端點
        {
            if (pos.y > yMax)
                topMost.Clear();
            topMost.Add(pos);
            yMax = pos.y;
        }
    }

    protected bool UpdateWalkers()
    {
        bool isCellGen = false;
        foreach (RandomWalker walker in walkerList)
        {
            if (rwMap[walker.pos.x, walker.pos.y] == 0)
            {
                rwMap[walker.pos.x, walker.pos.y] = 2;
                //Debug.Log("第 " + blockNum + " 個: " + walker.pos);
                blockNum++;

                //先找最遠的當終點
                int currDist = Mathf.Abs(walker.pos.x - vStart.x) + Mathf.Abs(walker.pos.y - vStart.y);
                if (currDist > endDist)
                {
                    vEnd = walker.pos;
                    endDist = currDist;
                    //print("curr endDis: " + endDist + "start " + puzzleStart + " end " + puzzleEnd);
                }

                CheckTernima(walker.pos);

                isCellGen = true;
                if (blockNum == blockNumMax)
                {
                    return true;
                }
            }
            //移動一步
            walker.pos += walker.dir;
            walker.pos.x = Mathf.Max(Mathf.Min(walker.pos.x, rwXMax), rwXMin);
            walker.pos.y = Mathf.Max(Mathf.Min(walker.pos.y, rwYMax), rwYMin);

            if (Random.Range(0, 1) < changeDirRatio)
            {
                walker.SetRandomDir();
            }

            //float rd = Random.Range(0, 1.0f);
            if (walkerList.Count > 1 && Random.Range(0, 1.0f) < deadRatio)
            {
                //print("Death !!");
                walkerList.Remove(walker);
                break;
            }
            if (walkerList.Count < maxWalkers && Random.Range(0, 1.0f) < cloneRatio)
            {
                //print("Clone !!");
                RandomWalker newWalker = new RandomWalker(walker.pos);
                walkerList.Add(newWalker);
                break;
            }
        }
        //print("Walker GO: ( " + walkerList.Count + " )");

        return isCellGen;
    }

    public class RandomWalker
    {
        public Vector2Int pos;
        public Vector2Int dir;
        public RandomWalker(Vector2Int _pos)
        {
            pos = _pos;
            SetRandomDir();
        }
        public RandomWalker(Vector2Int _pos, Vector2Int _initDir)
        {
            pos = _pos;
            if (_initDir == Vector2Int.zero)
                SetRandomDir();
            else
                dir = _initDir;
        }
        public void SetRandomDir()
        {
            int dirSeed = Random.Range(0, 4);
            switch (dirSeed)
            {
                case 0:
                    dir.x = 0;
                    dir.y = 1;
                    break;
                case 1:
                    dir.x = -1;
                    dir.y = 0;
                    break;
                case 2:
                    dir.x = 1;
                    dir.y = 0;
                    break;
                case 3:
                    dir.x = 0;
                    dir.y = -1;
                    break;
            }
        }
    }
}
