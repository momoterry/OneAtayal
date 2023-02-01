using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;


public class OneMap
{
    public int mapWidth;
    public int mapHeight;

    public const int EDGE_VALUE = -1;
    public const int INVALID_VALUE = -9999;

    Vector2Int center;
    int xMin, xMax;
    int yMin, yMax;
    const int edgeWidth = 2;
    int arrayWidth, arrayHeight;

    int[][] mapArray;

    public void CreateMap(int width, int height)
    {
        xMax = width / 2;
        yMax = height / 2;
        xMin = xMax - width;
        yMin = yMax - height;
        mapWidth = width;
        mapHeight = height;

        arrayWidth = mapWidth + edgeWidth + edgeWidth;
        arrayHeight = mapHeight + edgeWidth + edgeWidth;

        //mapArray = new int[arrayWidth][];
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

        BuildSquareArea(grassGroup, mapCenter, mapWidth, mapHeight);

        BuildSquareArea(dirtGroup, mapCenter + new Vector3Int(0, 1, 0), mapWidth-4, 2);
        BuildSquareArea(dirtGroup, mapCenter + new Vector3Int(1, 0, 0), 2, mapHeight-4);

        //TEST
        int hWidth = mapWidth / 2;
        int hHeight = mapHeight / 2;
        Vector3Int coord = Vector3Int.zero;
        for (int i=-hWidth; i<hWidth; i++)
        {
            coord.x = i;
            //Tile t = groundTM.GetTile(coord).name;
            print(groundTM.GetTile(coord).name);

        }
        //
        theSurface2D.BuildNavMesh();
    }
}
