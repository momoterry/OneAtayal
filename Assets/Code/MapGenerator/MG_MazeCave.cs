using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_MazeCave : MG_MazeDungeon
{
    //RandomWalker
    public float blockFillRatio = 0.35f;
    public int maxWalkers = 6;
    //public Vector2Int walkerInitDir = Vector2Int.up;
    protected int initWalkerNum = 1;
    protected float changeDirRatio = 0.2f;
    protected float deadRatio = 0.15f;
    protected float cloneRatio = 0.2f;


    protected override void PreCreateMap()
    {
        base.PreCreateMap();
        print("��mapCenter " + mapCenter);
        mapCenter.y -= cellHeight * (puzzleHeight / 2);
        print("��mapCenter " + mapCenter);
    }

    protected override void InitPuzzleMap()
    {
        if (bigRooms.Length > 0)
        {
            print("ERROR!!!! �ثe MazeCave �L�k�䴩 Big Rooms !!!!�j��M�� !!");
            foreach (BigRoomInfo r in bigRooms)
            {
                r.size = Vector2Int.zero;
            }
        }

        //���]�_�I����Ϥ���
        print("��_�I " + puzzleStart);
        puzzleStart.y += puzzleHeight / 2;
        print("�s�_�I " + puzzleStart);

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
        if (extendTerminal)
        {
            if (puzzleStart.y > 0)
            {
                puzzleMap[puzzleStart.x][puzzleStart.y - 1].value = cellInfo.NORMAL;
            }
            if (puzzleEnd.y < puzzleHeight - 1)
            {
                puzzleMap[puzzleEnd.x][puzzleEnd.y + 1].value = cellInfo.NORMAL;
            }
        }
    }

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
        print("RandomWalker Total Block:" + blockNum);

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

                //����̻�������I

                int currDist = Mathf.Abs(walker.pos.x - puzzleStart.x) + Mathf.Abs(walker.pos.y - puzzleStart.y);
                if (currDist > endDist)
                {
                    puzzleEnd = walker.pos;
                    endDist = currDist;
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

