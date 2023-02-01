using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;


public class OneMap
{
    public int mapWidth;
    public int mapHeight;

    public const int EDGE_VALUE = 4;
    public const int INVALID_VALUE = -9999;

    Vector2Int center;
    int xMin, xMax;
    int yMin, yMax;
    const int edgeWidth = 2;
    int arrayWidth, arrayHeight;

    int[][] mapArray;

    public void InitMap(int width, int height)
    {
        xMax = width / 2;
        yMax = height / 2;
        xMin = xMax - width;
        yMin = yMax - height;
        mapWidth = width;
        mapHeight = height;

        arrayWidth = mapWidth + edgeWidth + edgeWidth;
        arrayHeight = mapHeight + edgeWidth + edgeWidth;

        mapArray = new int[arrayWidth][];
        for (int i=0; i<arrayWidth; i++)
        {
            mapArray[i] = new int[arrayHeight];
            for (int j=0; j< arrayHeight; j++)
            {
                mapArray[i][j] = 0;
            }
        }

        //Edge
        for (int x = 0; x < arrayWidth; x++)
        {
            for (int e = 0; e < edgeWidth; e++)
            {
                mapArray[x][e] = EDGE_VALUE;
                mapArray[x][arrayHeight - 1 - e] = EDGE_VALUE;
            }
        }
        for (int y = 0; y < arrayHeight; y++)
        {
            for (int e = 0; e < edgeWidth; e++)
            {
                mapArray[e][y] = EDGE_VALUE;
                mapArray[arrayWidth-1-e][y] = EDGE_VALUE;
            }
        }
    }

    public void PrintMap()
    {
        string str = "";
        for (int y = 0; y < arrayHeight; y++)
        {
            //string str = "";
            for (int x = 0; x < arrayWidth; x++)
            {
                str += mapArray[x][y].ToString();
            }
            str += "\n";
        }
        Debug.Log(str);
    }
}


[System.Serializable]
public class TileGroup
{
    public Tile baseTile;
    public Tile[] decorateTiles;
    public float decorateRate;
}

public class MG_ForestRD : MapGeneratorBase
{
    public Tilemap groundTM;
    public TileGroup grassGroup;
    public TileGroup dirtGroup;

    protected OneMap theMap = new OneMap();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void BuildSquareArea( TileGroup tg, Vector3Int center, int width, int height)
    {
        int hWidth = width / 2;
        int hHeight = height / 2;
        Vector3Int cd = Vector3Int.zero;
        //theSurface2D.BuildNavMesh();
        for (int x = -hWidth; x < hWidth; x++)
        {
            for (int y = -hHeight; y < hHeight; y++)
            {
                cd.x = center.x + x;
                cd.y = center.y + y;
                Tile t = tg.baseTile;
                if (tg.decorateTiles.Length > 0 && tg.decorateRate > 0)
                {
                    float rd = Random.Range(0, 1.0f);
                    if (rd < tg.decorateRate)
                    {
                        t = tg.decorateTiles[Random.Range(0, tg.decorateTiles.Length)];
                    }
                }
                groundTM.SetTile(cd, t);
            }
        }
    }


    public override void BuildAll(int buildLevel = 1)
    {
        int mapWidth = 12;
        int mapHeight = 20;
        Vector3Int mapCenter = Vector3Int.zero;

        theMap.InitMap(mapWidth, mapHeight);
        theMap.PrintMap();

        BuildSquareArea(grassGroup, mapCenter, mapWidth, mapHeight);

        BuildSquareArea(dirtGroup, mapCenter + new Vector3Int(0, 1, 0), mapWidth-4, 2);
        BuildSquareArea(dirtGroup, mapCenter + new Vector3Int(1, 0, 0), 2, mapHeight-4);


        theSurface2D.BuildNavMesh();
    }
}
