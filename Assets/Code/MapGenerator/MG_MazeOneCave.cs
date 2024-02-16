using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_MazeOneCave : MG_MazeOne
{
    void Awake()
    {
        //強制修正!!
        if (extendTerminal)
        {
            print("ERROR 強制修正!!!! MG_MazeOneCave 不能使用 extendTerminal");
            extendTerminal = false;
        }
        if (!allConnect)
        {
            print("ERROR 強制修正!!!! MG_MazeOneCave 必定為 allConnect");
            allConnect = true;
        }
    }

    protected override void InitPuzzleMap()
    {
        base.InitPuzzleMap();
        //先把預設起終點還原
        puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.NORMAL;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.NORMAL;

        puzzleStart.x = puzzleWidth / 2;
        puzzleStart.y = puzzleHeight / 2;

        CreateRandomWalkerMap();

        for (int i = 0; i < rwWidth; i++)
        {
            for (int j = 0; j < rwHeight; j++)
            {
                if (rwMap[i, j] == 0)
                {
                    puzzleMap[i][j].value = cellInfo.INVALID;
                }
            }
        }
        puzzleMap[puzzleStart.x][puzzleStart.y].value = cellInfo.TERNIMAL;
        puzzleMap[puzzleEnd.x][puzzleEnd.y].value = cellInfo.TERNIMAL;
    }
    //========================== RandomWalker 參數 =============================
    public float blockFillRatio = 0.35f;
    protected int maxWalkers = 6;
    protected int initWalkerNum = 1;
    protected float changeDirRatio = 0.2f;
    protected float deadRatio = 0.15f;
    protected float cloneRatio = 0.2f;
    //========================== Random Walker 演算法 =============================
    protected List<RandomWalker> walkerList = new List<RandomWalker>();
    protected int blockNum = 0;
    protected int blockNumMax;
    protected int[,] rwMap;
    protected int rwWidth, rwHeight;
    protected int rwXMin, rwXMax, rwYMin, rwYMax;

    protected int endDist;

    protected void CreateRandomWalkerMap()
    {
        rwWidth = puzzleWidth;
        rwHeight = puzzleHeight;
        rwMap = new int[rwWidth, rwHeight];
        rwXMin = rwYMin = 0;
        rwXMax = rwWidth - 1;
        rwYMax = rwHeight - 1;

        blockNumMax = (int)(rwWidth * rwHeight * blockFillRatio);

        for (int i = 0; i < initWalkerNum; i++)
        {
            RandomWalker w = new RandomWalker(puzzleStart);
            walkerList.Add(w);
        }

        puzzleEnd = puzzleStart;
        endDist = 0;
        int step = 0;
        while (blockNum < blockNumMax && step < 10000)
        {
            UpdateWalkers();
            step++;
        }
        //print("RandomWalker Total Block:" + blockNum);

    }

    protected bool UpdateWalkers()
    {
        bool isCellGen = false;
        foreach (RandomWalker walker in walkerList)
        {
            if (rwMap[walker.pos.x, walker.pos.y] == 0)
            {
                rwMap[walker.pos.x, walker.pos.y] = 2;
                blockNum++;

                //先找最遠的當終點

                int currDist = Mathf.Abs(walker.pos.x - puzzleStart.x) + Mathf.Abs(walker.pos.y - puzzleStart.y);
                if (currDist > endDist)
                {
                    puzzleEnd = walker.pos;
                    endDist = currDist;
                    //print("curr endDis: " + endDist + "start " + puzzleStart + " end " + puzzleEnd);
                }

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
