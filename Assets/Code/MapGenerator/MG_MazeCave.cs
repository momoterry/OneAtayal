using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_MazeCave : MG_MazeDungeon
{
    //RandomWalker
    public float blockFillRatio = 0.35f;
    public int maxWalkers = 6;
    protected int initWalkerNum = 1;
    protected float changeDirRatio = 0.2f;
    protected float deadRatio = 0.15f;
    protected float cloneRatio = 0.2f;


    protected override void PreCreateMap()
    {
        base.PreCreateMap();

        //洞穴地型，中心點移到全圖中間
        mapCenter.y -= cellHeight * (puzzleHeight / 2);
        puzzleX1 = mapCenter.x - (puzzleWidth * cellWidth / 2);
        puzzleY1 = mapCenter.y - (puzzleHeight * cellHeight / 2);
    }

    protected override void InitPuzzleMap()
    {
        //if (bigRooms.Length > 0)
        //{
        //    print("ERROR!!!! 目前 MazeCave 無法支援 Big Rooms !!!!強制清掉 !!");
        //    foreach (BigRoomInfo r in bigRooms)
        //    {
        //        r.size = Vector2Int.zero;
        //    }
        //}

        //重設起點到全圖中心
        //print("原起點 " + puzzleStart);
        puzzleStart.y += puzzleHeight / 2;
        //print("新起點 " + puzzleStart);

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
        //print("RandomWalker Total Block:" + blockNum);

    }

    //protected override List<RectInt> CreateNonOverlappingRects(List<Vector2Int> sizes, RectInt bound)
    //{
    //    List < RectInt > rList =  base.CreateNonOverlappingRects(sizes, bound);

    //    print("Room Info");
    //    foreach (RectInt r in rList)
    //    {
    //        print(r);
    //        for (int i = 0; i < r.width; i++)
    //        {
    //            for (int j = 0; j < r.height; j++)
    //            {
    //                print("Cell: " + puzzleMap[r.x + i][r.y + j].value);
    //            }
    //        }
    //    }

    //    return rList;
    //}

    //方案一: 只要一塊地板是有效地板就通過
    //protected override bool IsInvalidRect(List<RectInt> rects, RectInt newRect)
    //{
    //    bool inValid = base.IsInvalidRect(rects, newRect);
    //    if (!inValid)
    //    {
    //        for (int x=newRect.x; x< newRect.xMax; x++)
    //        {
    //            for (int y=newRect.y; y< newRect.yMax; y++)
    //            {
    //                if (puzzleMap[x][y].value != cellInfo.INVALID)
    //                {
    //                    //只要其中一塊地板不是無效即可
    //                    return false; ;
    //                }
    //            }
    //        }
    //    }
    //    return true; ;
    //}

    //方案二: 找全無效地板但有連接
    protected override bool IsInvalidRect(List<RectInt> rects, RectInt newRect)
    {
        bool inValid = base.IsInvalidRect(rects, newRect);
        if (!inValid)
        {
            for (int x = newRect.x; x < newRect.xMax; x++)
            {
                for (int y = newRect.y; y < newRect.yMax; y++)
                {
                    if (puzzleMap[x][y].value != cellInfo.INVALID)
                    {
                        //只要其中一塊地板不是無效，就算失敗
                        return true;
                    }
                }
            }

            //print("找到全空地版，尋找連結，這是第幾個 Room? " + rects.Count);

            int requireConnect = bigRooms[rects.Count].numDoor;
            int validConnect = 0;
            //print("至少要的連結數: " + requireConnect);
            for (int x = newRect.x; x < newRect.xMax; x++)
            {
                if (newRect.y > 0 && puzzleMap[x][newRect.y - 1].value == cellInfo.NORMAL)
                {
                    validConnect++;
                }
                if (newRect.yMax < puzzleHeight && puzzleMap[x][newRect.yMax].value == cellInfo.NORMAL)
                {
                    validConnect++;
                }

                if (validConnect >= requireConnect)  //TODO: 運算上限
                {
                    //找要找到足夠有效連結就可以
                    //print("找到足夠的連接處");
                    return false;
                }
            }
            for (int y = newRect.y; y < newRect.yMax; y++)
            {
                if (newRect.x > 0 && puzzleMap[newRect.x - 1][y].value == cellInfo.NORMAL)
                {
                    validConnect++;
                }
                if (newRect.xMax < puzzleWidth && puzzleMap[newRect.xMax][y].value == cellInfo.NORMAL)
                {
                    validConnect++;
                }

                if (validConnect >= 2)  //TODO: 運算上限
                {
                    //找要找到足夠有效連結就可以
                    //print("找到足夠的連接處");
                    return false;
                }
            }

            print("找不到足夠連結，失敗，連結數 = " + validConnect);
        }
        return true; ;
    }

    //========================== Random Walker 演算法 =============================
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

