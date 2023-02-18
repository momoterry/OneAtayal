using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_RandomWalker : MapGeneratorBase
{
    public Tilemap groundTM;
    public Tilemap blockTM;
    public TileGroup grassTG;
    public TileIslandEdgeGroup islandEG;
    public Tile blockTile;

    public int mapHalfSize = 50;
    public int cellHalfSize = 2;

    public int blockNumMax = 800;
    public int initWalkerNum = 1;
    public int maxWalkers = 6;
    public float changeDirRatio = 0.2f;
    public float deadRatio = 0.15f;
    public float cloneRatio = 0.2f;

    public float stepTime = 0.025f;

    //protected OneMap theMap= new OneMap();
    protected CellMap theCellMap = new CellMap();
    //protected int cellSize = 4;
    protected int mapXMin, mapXMax, mapYMin, mapYMax;

    protected int blockNum = 0;

    //===== Random Walker
    protected List<RandomWalker> walkerList = new List<RandomWalker>();

    // Start is called before the first frame update
    void Start()
    {

    }


    protected IEnumerator BuildMapIterator()
    {
        for (int i = 0; i < initWalkerNum; i++)
        {
            RandomWalker w = new RandomWalker(Vector2Int.zero);
            walkerList.Add(w);
        }

        while (blockNum < blockNumMax)
        {
            if (UpdateWalkers())
            {
                yield return new WaitForSeconds(stepTime);
            }
        }

        //for (int x = mapXMin; x <= mapXMax; x++)
        //{
        //    for (int y = mapYMin; y <= mapYMax; y++)
        //    {
        //        if (theCellMap.GetValue(x, y) == 0)
        //        {
        //            FillCell(x, y, blockTM, blockTile);
        //        }
        //    }
        //}

        theSurface2D.BuildNavMesh();
    }

    public override void BuildAll(int buildLevel = 1)
    {
        mapYMin = mapXMin = -mapHalfSize;
        mapYMax = mapXMax = mapHalfSize;
        //theMap.InitMap(Vector2Int.zero, mapHalfSize*2+1, mapHalfSize*2+1);
        theCellMap.InitMap(mapHalfSize, cellHalfSize);

        IEnumerator theC = BuildMapIterator();
        StartCoroutine(theC);
    }

    //===========================================================
    // Random Walker
    //===========================================================

    protected void FillCell(int x, int y, Tilemap tm, Tile tile)
    {
        Vector2Int vMin = theCellMap.GetCellMinCoord(x, y);
        for (int ix = 0; ix < theCellMap.GetCellSize(); ix++)
        {
            for (int iy = 0; iy < theCellMap.GetCellSize(); iy++)
            {
                Vector2Int pos = vMin + new Vector2Int(ix, iy);
                tm.SetTile((Vector3Int)pos, tile);
            }
        }
    }

    protected void FillTile(int xMin, int yMin, int width, int height, Tilemap tm, Tile tile)
    {
        Vector3Int pos = new Vector3Int(xMin, yMin, 0);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tm.SetTile(pos, tile);
                pos.y++;
            }
            pos.x++;
            pos.y = yMin;
        }
    }

    protected void FillGroundCell(int x, int y)
    {
        Vector2Int vMin = theCellMap.GetCellMinCoord(x, y);
        int cellSize = theCellMap.GetCellSize();
        for (int ix = 0; ix < cellSize; ix++)
        {
            for (int iy = 0; iy < cellSize; iy++)
            {
                Vector2Int pos = vMin + new Vector2Int(ix, iy);
                groundTM.SetTile((Vector3Int)pos, grassTG.GetOneTile());
                blockTM.SetTile((Vector3Int)pos, null);
            }
        }
        //邊界處理
        //上
        if (theCellMap.GetValue(x, y + 1) == 0)
        {
            FillTile(vMin.x, vMin.y + cellSize, cellSize, 1, blockTM, islandEG.UU);
            if (theCellMap.GetValue(x - 1, y) == 0)
                FillTile(vMin.x-1, vMin.y + cellSize, 1, 1, blockTM, islandEG.LU);
            if (theCellMap.GetValue(x + 1, y) == 0)
                FillTile(vMin.x + cellSize, vMin.y + cellSize, 1, 1, blockTM, islandEG.RU);
            //凹陷處理
            if (theCellMap.GetValue(x - 1, y + 1) != 0)
            {
                FillTile(vMin.x, vMin.y + cellSize, 1, 1, blockTM, islandEG.RU_S);
            }
            if (theCellMap.GetValue(x + 1, y + 1) != 0)
            {
                FillTile(vMin.x + cellSize - 1, vMin.y + cellSize, 1, 1, blockTM, islandEG.LU_S);
            }
        }
        //下
        if (theCellMap.GetValue(x, y - 1) == 0)
        {
            FillTile(vMin.x, vMin.y - 1, cellSize, 1, blockTM, islandEG.DD);
            FillTile(vMin.x, vMin.y - 2, cellSize, 1, blockTM, islandEG.DD2);
            if (theCellMap.GetValue(x - 1, y) == 0)
            {
                FillTile(vMin.x - 1, vMin.y - 1, 1, 1, blockTM, islandEG.LD);
                FillTile(vMin.x - 1, vMin.y - 2, 1, 1, blockTM, islandEG.LD2);
            }
            if (theCellMap.GetValue(x + 1, y) == 0)
            {
                FillTile(vMin.x + cellSize, vMin.y - 1, 1, 1, blockTM, islandEG.RD);
                FillTile(vMin.x + cellSize, vMin.y - 2, 1, 1, blockTM, islandEG.RD2);
            }
            //凹陷處理
            if (theCellMap.GetValue(x - 1, y - 1) != 0)
            {
                FillTile(vMin.x, vMin.y - 1 , 1, 1, blockTM, islandEG.RD_S);
                FillTile(vMin.x, vMin.y - 2, 1, 1, blockTM, islandEG.RD_S2);
            }
            if (theCellMap.GetValue(x + 1, y - 1) != 0)
            {
                FillTile(vMin.x + cellSize - 1, vMin.y - 1, 1, 1, blockTM, islandEG.LD_S);
                FillTile(vMin.x + cellSize - 1, vMin.y - 2, 1, 1, blockTM, islandEG.LD_S2);
            }
        }

        //左
        if (theCellMap.GetValue(x - 1, y) == 0)
        {
            FillTile(vMin.x-1, vMin.y, 1, cellSize, blockTM, islandEG.LL);
            //凹陷處理
            if (theCellMap.GetValue(x - 1, y + 1) != 0)
            {
                FillTile(vMin.x - 1, vMin.y + cellSize - 1, 1, 1, blockTM, islandEG.LD_S);
                FillTile(vMin.x - 1, vMin.y + cellSize - 2, 1, 1, blockTM, islandEG.LD_S2);
            }
            if (theCellMap.GetValue(x - 1, y - 1) != 0)
            {
                FillTile(vMin.x - 1, vMin.y, 1, 1, blockTM, islandEG.LU_S);
            }
        }

        //右
        if (theCellMap.GetValue(x + 1, y) == 0)
        {
            FillTile(vMin.x + cellSize, vMin.y, 1, cellSize, blockTM, islandEG.RR);
            //凹陷處理
            if (theCellMap.GetValue(x + 1, y + 1) != 0)
            {
                FillTile(vMin.x + cellSize, vMin.y + cellSize - 1, 1, 1, blockTM, islandEG.RD_S);
                FillTile(vMin.x + cellSize, vMin.y + cellSize - 2, 1, 1, blockTM, islandEG.RD_S2);
            }
            if (theCellMap.GetValue(x + 1, y - 1) != 0)
            {
                FillTile(vMin.x + cellSize, vMin.y, 1, 1, blockTM, islandEG.RU_S);
            }
        }

    }

    protected bool UpdateWalkers()
    {
        bool isCellGen = false;
        foreach (RandomWalker walker in walkerList)
        {
            //if (theMap.GetValue(walker.pos) == OneMap.DEFAULT_VALUE)
            if (theCellMap.GetValue(walker.pos.x, walker.pos.y) == 0)
            {
                //theMap.SetValue(walker.pos.x, walker.pos.y, 2);
                //groundTM.SetTile((Vector3Int)walker.pos, grassTG.GetOneTile());
                theCellMap.SetValue(walker.pos.x, walker.pos.y, 2);
                FillGroundCell(walker.pos.x, walker.pos.y);
                blockNum++;
                isCellGen = true;
                if (blockNum == blockNumMax)
                {
                    //print("功德圓滿 !!");
                    return true;
                }
            }
            //移動一步
            walker.pos += walker.dir;
            walker.pos.x = Mathf.Max(Mathf.Min(walker.pos.x, mapXMax-1), mapXMin+1);
            walker.pos.y = Mathf.Max(Mathf.Min(walker.pos.y, mapYMax-1), mapYMin+1);

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

    // ============================ 水邊界處理 =============================
    // TODO : 應該集中到別處
    [System.Serializable]
    public class TileIslandEdgeGroup : TileEdgeGroup
    {
        //下
        public Tile DD2;
        //左下、右下
        public Tile LD2;
        public Tile RD2;
        //左下陷、右下陷
        public Tile LD_S2;
        public Tile RD_S2;
    }
}
