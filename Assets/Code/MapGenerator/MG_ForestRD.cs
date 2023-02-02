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

public enum MAP_EDGE_TYPE
{
    UU = 11,
    DD = 12,
    LL = 13,
    RR = 14,
    LU = 21,
    RU = 22,
    LD = 23,
    RD = 24,
    LU_S = 31,
    RU_S = 32,
    LD_S = 33,
    RD_S = 34,
}

[System.Serializable]
public class TileEdgeGroup
{
    //上、下、左、右
    public Tile UU;
    public Tile DD;
    public Tile LL;
    public Tile RR;
    //左上、右上、左下、右下
    public Tile LU;
    public Tile RU;
    public Tile LD;
    public Tile RD;
    //左上陷、右上陷、左下陷、右下陷
    public Tile LU_S;
    public Tile RU_S;
    public Tile LD_S;
    public Tile RD_S;

    public Tile GetTile(MAP_EDGE_TYPE type)
    {
        switch (type)
        {
            case MAP_EDGE_TYPE.UU:
                return UU;
            case MAP_EDGE_TYPE.DD:
                return DD;
            case MAP_EDGE_TYPE.LL:
                return LL;
            case MAP_EDGE_TYPE.RR:
                return RR;
            //====
            case MAP_EDGE_TYPE.LU:
                return LU;
            case MAP_EDGE_TYPE.RU:
                return RU;
            case MAP_EDGE_TYPE.LD:
                return LD;
            case MAP_EDGE_TYPE.RD:
                return RD;
            //====
            case MAP_EDGE_TYPE.LU_S:
                return LU_S;
            case MAP_EDGE_TYPE.RU_S:
                return RU_S;
            case MAP_EDGE_TYPE.LD_S:
                return LD_S;
            case MAP_EDGE_TYPE.RD_S:
                return RD_S;
        }
        return null;
    }
}

public class MG_ForestRD : MapGeneratorBase
{
    public Tilemap groundTM;
    public TileGroup grassGroup;
    public TileGroup dirtGroup;
    public TileEdgeGroup grassEdgeGroup;

    protected enum TILE_TYPE
    {
        GRASS = 4,
        DIRT = 5,
    }

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
        //Vector3Int cd = Vector3Int.zero;
        for (int x = -hWidth; x < hWidth; x++)
        {
            for (int y = -hHeight; y < hHeight; y++)
            {
                theMap.SetValue(x, y, value);
            }
        }
    }

    protected int CheckEdgeType( int x, int y, int value, int outValue)
    {
        bool bL = (theMap.GetValue(x - 1, y) == outValue);
        bool bR = (theMap.GetValue(x + 1, y) == outValue);
        if (theMap.GetValue(x, y+1) == outValue)        //上方有邊界
        {
            if (bL)  //左方也有邊界
                return (int)MAP_EDGE_TYPE.LU;
            else if (bR)  //右方也有邊界
                return (int)MAP_EDGE_TYPE.RU;
            return (int)MAP_EDGE_TYPE.UU;
        }
        if (theMap.GetValue(x, y - 1) == outValue)        //下方有邊界
        {
            if (bL)  //左方也有邊界
                return (int)MAP_EDGE_TYPE.LD;
            else if (bR)  //右方也有邊界
                return (int)MAP_EDGE_TYPE.RD;
            return (int)MAP_EDGE_TYPE.DD;
        }
        if (bL)
            return (int)MAP_EDGE_TYPE.LL;
        if (bR)
            return (int)MAP_EDGE_TYPE.RR;
        
        if (theMap.GetValue(x - 1, y + 1) == outValue)
            return (int)MAP_EDGE_TYPE.LU_S;
        if (theMap.GetValue(x + 1, y + 1) == outValue)
            return (int)MAP_EDGE_TYPE.RU_S;
        if (theMap.GetValue(x - 1, y - 1) == outValue)
            return (int)MAP_EDGE_TYPE.LD_S;
        if (theMap.GetValue(x + 1, y - 1) == outValue)
            return (int)MAP_EDGE_TYPE.RD_S;

        return value;
    }

    protected void EdgeDetectInMap( int value, int outValue, Vector3Int center, int width, int height)
    {
        int hWidth = width / 2;
        int hHeight = height / 2;
        for (int x = -hWidth; x < hWidth; x++)
        {
            for (int y = -hHeight; y < hHeight; y++)
            {
                if (theMap.GetValue(x, y) == (int)TILE_TYPE.GRASS )
                {
                    int newValue = CheckEdgeType(x, y, (int)TILE_TYPE.GRASS, (int)TILE_TYPE.DIRT);
                    theMap.SetValue(x, y, newValue);
                }
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

        FillSquareInMap((int)TILE_TYPE.GRASS, mapCenter, mapWidth, mapHeight);
        FillSquareInMap((int)TILE_TYPE.DIRT, mapCenter + new Vector3Int(0, 1, 0), mapWidth - 4, 2);
        FillSquareInMap((int)TILE_TYPE.DIRT, mapCenter + new Vector3Int(1, 0, 0), 2, mapHeight - 4);

        EdgeDetectInMap((int)TILE_TYPE.GRASS, (int)TILE_TYPE.DIRT, mapCenter, mapWidth, mapHeight);

        //==== 畫地圖結束，實裝 Tile

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
                int value = theMap.GetValue(x, y);
                if (value == (int)TILE_TYPE.GRASS)
                {
                    groundTM.SetTile((Vector3Int)cd, grassGroup.GetOneTile());
                }
                else if (value == (int)TILE_TYPE.DIRT)
                {
                    groundTM.SetTile((Vector3Int)cd, dirtGroup.GetOneTile());
                }
                else
                {
                    Tile eT = grassEdgeGroup.GetTile((MAP_EDGE_TYPE)theMap.GetValue(x, y));
                    groundTM.SetTile((Vector3Int)cd, eT);
                }
            }
        }
    }

}
