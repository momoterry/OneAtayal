using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_RandomWalker : MapGeneratorBase
{
    public Tilemap groundTM;

    public TileGroup grassTG;

    protected OneMap theMap= new OneMap();
    protected int cellMapSize = 10;
    protected int cellSize = 4;

    protected int mapXMin, mapXMax, mapYMin, mapYMax;

    //===== Random Walker
    protected List<RandomWalker> walkerList = new List<RandomWalker>();

    // Start is called before the first frame update
    void Start()
    {
        
    }


    protected IEnumerator BuildMapIterator()
    {
        float stepTime = 0.025f;
        for (int i = 0; i < 3; i++)
        {
            RandomWalker w = new RandomWalker(Vector2Int.zero);
            walkerList.Add(w);
        }

        //for (int x = -5; x < 5; x++)
        //{
        //    for (int y = -5; y < 5; y++)
        //    {
        //        groundTM.SetTile(new Vector3Int(x, y, 0), grassTG.GetOneTile());
        //        yield return new WaitForSeconds(stepTime);
        //    }
        //}

        for (int i = 0; i < 100; i++)
        {
            UpdateWalkers();
            yield return new WaitForSeconds(stepTime);
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
        foreach( RandomWalker walker in walkerList)
        {
            if (theMap.GetValue(walker.pos) == OneMap.DEFAULT_VALUE)
            {
                theMap.SetValue(walker.pos, 2);
                groundTM.SetTile((Vector3Int)walker.pos, grassTG.GetOneTile());
            }
            //²¾°Ê¤@¨B
            walker.pos += walker.dir;
            walker.pos.x = Mathf.Max(Mathf.Min(walker.pos.x, mapXMax), mapXMin);
            walker.pos.y = Mathf.Max(Mathf.Min(walker.pos.y, mapYMax), mapYMin);
            if (Random.Range(0, 1) < 0.2f)
            {
                walker.SetRandomDir();
            }
            //print("Walker GO: ( " + walker.pos.x + ", " + walker.pos.y + " )");
        }

        return true;
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
