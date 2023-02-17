using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_RandomWalker : MapGeneratorBase
{
    public Tilemap groundTM;
    public Tilemap blockTM;
    public TileGroup grassTG;
    public Tile blockTile;

    public int cellMapSize = 10;

    public int cellNumMax = 160;
    public int initWalkerNum = 1;
    public int maxWalkers = 6;
    public float changeDirRatio = 0.2f;
    public float deadRatio = 0.15f;
    public float cloneRatio = 0.2f;

    public float stepTime = 0.025f;

    protected OneMap theMap= new OneMap();
    protected int cellSize = 4;
    protected int mapXMin, mapXMax, mapYMin, mapYMax;

    protected int cellNum = 0;

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

        while ( cellNum < cellNumMax)
        {
            if (UpdateWalkers())
            {
                yield return new WaitForSeconds(stepTime);
            }
        }

        for (int x = mapXMin; x <= mapXMax; x++)
        {
            for (int y = mapYMin; y<= mapYMax; y++)
            {
                if (theMap.GetValue(x, y) == OneMap.DEFAULT_VALUE)
                {
                    blockTM.SetTile(new Vector3Int(x, y, 0), blockTile);
                }
            }
        }

        theSurface2D.BuildNavMesh();
    }

    public override void BuildAll(int buildLevel = 1)
    {
        int size = (cellMapSize + cellMapSize + 1) * cellSize;
        mapYMin = mapXMin = -size / 2;
        mapYMax = mapXMax = size / 2;
        theMap.InitMap(Vector2Int.zero, size, size);

        IEnumerator theC = BuildMapIterator();
        StartCoroutine(theC);
    }

    //===========================================================
    // Random Walker
    //===========================================================

    protected bool UpdateWalkers()
    {
        bool isCellGen = false;
        foreach( RandomWalker walker in walkerList)
        {
            if (theMap.GetValue(walker.pos) == OneMap.DEFAULT_VALUE)
            {
                theMap.SetValue(walker.pos, 2);
                groundTM.SetTile((Vector3Int)walker.pos, grassTG.GetOneTile());
                cellNum++;
                isCellGen = true;
                if (cellNum == cellNumMax)
                {
                    //print("功德圓滿 !!");
                    return true;
                }
            }
            //移動一步
            walker.pos += walker.dir;
            walker.pos.x = Mathf.Max(Mathf.Min(walker.pos.x, mapXMax), mapXMin);
            walker.pos.y = Mathf.Max(Mathf.Min(walker.pos.y, mapYMax), mapYMin);

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


}
