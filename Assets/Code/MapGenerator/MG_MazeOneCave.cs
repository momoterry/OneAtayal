using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MG_MazeOneCave : MG_MazeOne
{
    public enum MAZE_DIR
    {
        INSIDE_OUT,     //�q�������~��
        DONW_TO_TOP,
        TOP_TO_DOWN,
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT,
    }
    public MAZE_DIR mazeDir = MAZE_DIR.INSIDE_OUT;

    protected override void InitPuzzleMap()
    {
        base.InitPuzzleMap();
        //����w�]�_���I�٭�
        //puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.NORMAL;
        //puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.NORMAL;

        puzzleStart.x = puzzleWidth / 2;
        puzzleStart.y = puzzleHeight / 2;

        RandamWalkerMap theRWMap = new RandamWalkerMap();
        theRWMap.blockFillRatio = blockFillRatio;
        //print("maxWalkers: " + theRWMap.maxWalkers);

        theRWMap.CreateRandomWalkerMap(puzzleWidth, puzzleHeight, puzzleStart, ref puzzleEnd);

        List<Vector2Int> leftMost = new List<Vector2Int>();
        List<Vector2Int> rightMost = new List<Vector2Int>();
        List<Vector2Int> topMost = new List<Vector2Int>();
        List<Vector2Int> downMost = new List<Vector2Int>();
        int xMin = int.MaxValue, yMin = int.MaxValue;
        int xMax = int.MinValue, yMax = int.MinValue;
        for (int i = 0; i < theRWMap.rwWidth; i++)
        {
            for (int j = 0; j < theRWMap.rwHeight; j++)
            {
                if (theRWMap.rwMap[i, j] == 0)
                {
                    puzzleMap[i][j].value = cellInfo.INVALID;
                }
                else
                {
                    if (i <= xMin)  //���̥�����I 
                    {
                        if (i < xMin)
                            leftMost.Clear();
                        leftMost.Add(new Vector2Int(i, j));
                        xMin = i;
                    }
                    if (i >= xMax)  //���̥k����I 
                    {
                        if (i > xMax)
                            rightMost.Clear();
                        rightMost.Add(new Vector2Int(i, j));
                        xMax = i;
                    }
                    if (j <= yMin)  //���̤U�����I
                    {
                        if (j < yMin)
                            downMost.Clear();
                        downMost.Add(new Vector2Int(i, j));
                        yMin = j;
                    }
                    if (j >= yMax)  //���̤W�����I
                    {
                        if (j > yMax)
                            topMost.Clear();
                        topMost.Add(new Vector2Int(i, j));
                        yMax = j;
                    }
                }
            }
        }

        //print("�̥�����I: " + leftMost.Count);
        //print("�̥k����I: " + rightMost.Count);
        //print("�̤U����I: " + downMost.Count);
        //print("�̤W����I: " + topMost.Count);

        switch (mazeDir)
        {
            case MAZE_DIR.DONW_TO_TOP:
                puzzleStart = downMost[Random.Range(0, downMost.Count)];
                puzzleEnd = topMost[Random.Range(0, topMost.Count)];
                BattleSystem.GetInstance().initPlayerDirAngle = 0;
                break;
            case MAZE_DIR.TOP_TO_DOWN:
                puzzleStart = topMost[Random.Range(0, topMost.Count)];
                puzzleEnd = downMost[Random.Range(0, downMost.Count)];
                BattleSystem.GetInstance().initPlayerDirAngle = 180;
                break;
            case MAZE_DIR.LEFT_TO_RIGHT:
                puzzleStart = leftMost[Random.Range(0, leftMost.Count)];
                puzzleEnd = rightMost[Random.Range(0, rightMost.Count)];
                BattleSystem.GetInstance().initPlayerDirAngle = 90;
                break;
            case MAZE_DIR.RIGHT_TO_LEFT:
                puzzleStart = rightMost[Random.Range(0, rightMost.Count)];
                puzzleEnd = leftMost[Random.Range(0, leftMost.Count)];
                BattleSystem.GetInstance().initPlayerDirAngle = -90;
                break;
        }
    }

    ////========================== RandomWalker �Ѽ� =============================
    public float blockFillRatio = 0.35f;
    //protected int maxWalkers = 6;
    //protected int initWalkerNum = 1;
    //protected float changeDirRatio = 0.2f;
    //protected float deadRatio = 0.15f;
    //protected float cloneRatio = 0.2f;
    ////========================== Random Walker �t��k =============================
    //protected List<RandomWalker> walkerList = new List<RandomWalker>();
    //protected int blockNum = 0;
    //protected int blockNumMax;
    //protected int[,] rwMap;
    //protected int rwWidth, rwHeight;
    //protected int rwXMin, rwXMax, rwYMin, rwYMax;

    //protected int endDist;

    //protected void CreateRandomWalkerMap()
    //{
    //    rwWidth = puzzleWidth;
    //    rwHeight = puzzleHeight;
    //    rwMap = new int[rwWidth, rwHeight];
    //    rwXMin = rwYMin = 0;
    //    rwXMax = rwWidth - 1;
    //    rwYMax = rwHeight - 1;

    //    blockNumMax = (int)(rwWidth * rwHeight * blockFillRatio);

    //    for (int i = 0; i < initWalkerNum; i++)
    //    {
    //        RandomWalker w = new RandomWalker(puzzleStart);
    //        walkerList.Add(w);
    //    }

    //    puzzleEnd = puzzleStart;
    //    endDist = 0;
    //    int step = 0;
    //    while (blockNum < blockNumMax && step < 10000)
    //    {
    //        UpdateWalkers();
    //        step++;
    //    }
    //    //print("RandomWalker Total Block:" + blockNum);

    //}

    //protected bool UpdateWalkers()
    //{
    //    bool isCellGen = false;
    //    foreach (RandomWalker walker in walkerList)
    //    {
    //        if (rwMap[walker.pos.x, walker.pos.y] == 0)
    //        {
    //            rwMap[walker.pos.x, walker.pos.y] = 2;
    //            blockNum++;

    //            //����̻�������I

    //            int currDist = Mathf.Abs(walker.pos.x - puzzleStart.x) + Mathf.Abs(walker.pos.y - puzzleStart.y);
    //            if (currDist > endDist)
    //            {
    //                puzzleEnd = walker.pos;
    //                endDist = currDist;
    //                //print("curr endDis: " + endDist + "start " + puzzleStart + " end " + puzzleEnd);
    //            }

    //            isCellGen = true;
    //            if (blockNum == blockNumMax)
    //            {
    //                return true;
    //            }
    //        }
    //        //���ʤ@�B
    //        walker.pos += walker.dir;
    //        walker.pos.x = Mathf.Max(Mathf.Min(walker.pos.x, rwXMax), rwXMin);
    //        walker.pos.y = Mathf.Max(Mathf.Min(walker.pos.y, rwYMax), rwYMin);

    //        if (Random.Range(0, 1) < changeDirRatio)
    //        {
    //            walker.SetRandomDir();
    //        }

    //        //float rd = Random.Range(0, 1.0f);
    //        if (walkerList.Count > 1 && Random.Range(0, 1.0f) < deadRatio)
    //        {
    //            //print("Death !!");
    //            walkerList.Remove(walker);
    //            break;
    //        }
    //        if (walkerList.Count < maxWalkers && Random.Range(0, 1.0f) < cloneRatio)
    //        {
    //            //print("Clone !!");
    //            RandomWalker newWalker = new RandomWalker(walker.pos);
    //            walkerList.Add(newWalker);
    //            break;
    //        }
    //    }
    //    //print("Walker GO: ( " + walkerList.Count + " )");

    //    return isCellGen;
    //}

    //public class RandomWalker
    //{
    //    public Vector2Int pos;
    //    public Vector2Int dir;
    //    public RandomWalker(Vector2Int _pos)
    //    {
    //        pos = _pos;
    //        SetRandomDir();
    //    }
    //    public RandomWalker(Vector2Int _pos, Vector2Int _initDir)
    //    {
    //        pos = _pos;
    //        if (_initDir == Vector2Int.zero)
    //            SetRandomDir();
    //        else
    //            dir = _initDir;
    //    }
    //    public void SetRandomDir()
    //    {
    //        int dirSeed = Random.Range(0, 4);
    //        switch (dirSeed)
    //        {
    //            case 0:
    //                dir.x = 0;
    //                dir.y = 1;
    //                break;
    //            case 1:
    //                dir.x = -1;
    //                dir.y = 0;
    //                break;
    //            case 2:
    //                dir.x = 1;
    //                dir.y = 0;
    //                break;
    //            case 3:
    //                dir.x = 0;
    //                dir.y = -1;
    //                break;
    //        }
    //    }
    //}

}

public class RandamWalkerMap
{
    //========================== RandomWalker �Ѽ� =============================
    public float blockFillRatio = 0.35f;
    public int maxWalkers = 6;
    public int initWalkerNum = 1;
    public float changeDirRatio = 0.2f;
    public float deadRatio = 0.15f;
    public float cloneRatio = 0.2f;
    //========================== Random Walker �t��k =============================
    protected List<RandomWalker> walkerList = new List<RandomWalker>();
    protected int blockNum = 0;
    protected int blockNumMax;
    public int[,] rwMap;
    public int rwWidth, rwHeight;
    protected int rwXMin, rwXMax, rwYMin, rwYMax;
    protected Vector2Int vStart;
    protected Vector2Int vEnd;
    protected int endDist;

    public void CreateRandomWalkerMap(int mapWidth, int mapHeight, Vector2Int _vStart, ref Vector2Int _vEnd)
    {
        rwWidth = mapWidth;
        rwHeight = mapHeight;
        rwMap = new int[rwWidth, rwHeight];
        rwXMin = rwYMin = 0;
        rwXMax = rwWidth - 1;
        rwYMax = rwHeight - 1;
        vStart = _vStart;
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
        _vEnd = vEnd;
    }

    protected bool UpdateWalkers()
    {
        bool isCellGen = false;
        foreach (RandomWalker walker in walkerList)
        {
            if (rwMap[walker.pos.x, walker.pos.y] == 0)
            {
                rwMap[walker.pos.x, walker.pos.y] = 2;
                //Debug.Log("�� " + blockNum + " ��: " + walker.pos);
                blockNum++;

                //����̻�������I

                int currDist = Mathf.Abs(walker.pos.x - vStart.x) + Mathf.Abs(walker.pos.y - vStart.y);
                if (currDist > endDist)
                {
                    vEnd = walker.pos;
                    endDist = currDist;
                    //print("curr endDis: " + endDist + "start " + puzzleStart + " end " + puzzleEnd);
                }

                isCellGen = true;
                if (blockNum == blockNumMax)
                {
                    return true;
                }
            }
            //���ʤ@�B
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
