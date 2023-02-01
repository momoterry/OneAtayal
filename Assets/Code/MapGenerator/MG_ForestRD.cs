using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;



[System.Serializable]
public class TileGroup
{
    public Tile baseTile;
    public Tile[] decorateTiles;
    public float decorateRate;
    public Tile GetOneTile()
    {
        if (decorateTiles.Length > 0 && decorateRate > 0)
        {
            float rd = Random.Range(0, 1.0f);
            if (rd < decorateRate)
            {
                return decorateTiles[Random.Range(0, decorateTiles.Length)];
            }
        }
        return baseTile;
    }
}

public class MG_ForestRD : MapGeneratorBase
{
    public Tilemap groundTM;
    public TileGroup grassGroup;
    public TileGroup dirtGroup;

    protected int mapWidth = 12;
    protected int mapHeight = 20;

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

    protected void FillSquareInMap(int value, Vector3Int center, int width, int height)
    {
        int hWidth = width / 2;
        int hHeight = height / 2;
        Vector3Int cd = Vector3Int.zero;
        for (int x = -hWidth; x < hWidth; x++)
        {
            for (int y = -hHeight; y < hHeight; y++)
            {
                cd.x = center.x + x;
                cd.y = center.y + y;
                theMap.SetValue((Vector2Int)cd, value);
            }
        }
    }

    public override void BuildAll(int buildLevel = 1)
    {
        //int mapWidth = 12;
        //int mapHeight = 20;
        Vector3Int mapCenter = Vector3Int.zero;
        theMap.InitMap((Vector2Int)mapCenter, mapWidth, mapHeight);
        
        //==== 以下開始畫地圖

        FillSquareInMap(2, mapCenter, mapWidth, mapHeight);
        FillSquareInMap(3, mapCenter + new Vector3Int(0, 1, 0), mapWidth - 4, 2);
        FillSquareInMap(3, mapCenter + new Vector3Int(1, 0, 0), 2, mapHeight - 4);

        //==== 畫地圖結束

        //theMap.PrintMap();

        GenerateTiles();


        theSurface2D.BuildNavMesh();
    }

    protected void GenerateTiles()
    {
        Vector2Int cd = Vector2Int.zero;
        for ( int x = theMap.xMin; x < theMap.xMax; x++ )
        {
            for ( int y = theMap.yMin; y < theMap.yMax; y++ )
            {
                cd.x = x;
                cd.y = y;
                int value = theMap.GetValue(cd);
                if (value == 2)
                {
                    groundTM.SetTile((Vector3Int)cd, grassGroup.GetOneTile());
                }
                else if (value == 3)
                {
                    groundTM.SetTile((Vector3Int)cd, dirtGroup.GetOneTile());
                }
            }
        }
    }

}
