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
    NOT = 0,
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
    public int mapWidth = 12;
    public int mapHeight = 16;
    public int borderWidth = 4;
    public Vector3Int mapCenter;

    public Tilemap groundTM;
    public Tilemap blockTM;
    public TileGroup grassGroup;
    public TileGroup dirtGroup;
    public TileGroup blockGroup;
    public TileEdgeGroup grassEdgeGroup;
    public TileEdgeGroup dirtEdgeGroup;
    public TileEdgeGroup blockEdgeGroup;

    protected enum TILE_TYPE
    {
        GRASS = 4,
        DIRT = 5,
        BLOCK = 6,
        EDGE_VALUE = 100, //大於等於這個值就是 Edge
        GRASS_EDGE =400,
        DIRT_EDGE = 500,
        BLOCK_EDGE = 600,
    }

    protected OneMap theMap = new OneMap();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    virtual protected void CreateForestMap()
    {
        FillSquareInMap((int)TILE_TYPE.DIRT, mapCenter + new Vector3Int(0, -2, 0), mapWidth - 4, 4);
        FillSquareInMap((int)TILE_TYPE.DIRT, mapCenter + new Vector3Int(2, 0, 0), 4, mapHeight - 4);

        FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(-4, 4, 0), 4, 4);
    }

    virtual protected void PreCreateMap()
    {

    }

    public override void BuildAll(int buildLevel = 1)
    {
        PreCreateMap();

        //Vector3Int mapCenter = Vector3Int.zero;
        theMap.InitMap((Vector2Int)mapCenter, mapWidth + borderWidth + borderWidth, mapHeight + borderWidth + borderWidth);

        //==== 開始畫地圖
        //基本地板
        FillSquareInMap((int)TILE_TYPE.GRASS, mapCenter, mapWidth, mapHeight);
        //主要地圖設計部份
        CreateForestMap(); 


        //四方邊界
        FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(-mapWidth / 2 - borderWidth / 2, 0, 0), borderWidth, mapHeight + borderWidth + borderWidth);
        FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(mapWidth / 2 + borderWidth / 2, 0, 0), borderWidth, mapHeight + borderWidth + borderWidth);
        FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(0, mapHeight / 2 + borderWidth / 2, 0), mapWidth, borderWidth);
        FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(0, -mapHeight / 2 - borderWidth / 2, 0), mapWidth, borderWidth);

        //!! 交界處處理 !!
        //EdgeDetectInMap((int)TILE_TYPE.DIRT, (int)TILE_TYPE.GRASS, (int)TILE_TYPE.DIRT_EDGE, mapCenter, mapWidth, mapHeight);
        //EdgeDetectInMap((int)TILE_TYPE.BLOCK, (int)TILE_TYPE.GRASS, (int)TILE_TYPE.BLOCK_EDGE, mapCenter, mapWidth + borderWidth, mapHeight + borderWidth);
        EdgeDetectInMap((int)TILE_TYPE.DIRT, -1, (int)TILE_TYPE.DIRT_EDGE, mapCenter, mapWidth, mapHeight);
        EdgeDetectInMap((int)TILE_TYPE.BLOCK, -1, (int)TILE_TYPE.BLOCK_EDGE, mapCenter, mapWidth + borderWidth, mapHeight + borderWidth);


        //==== 畫地圖結束，實裝 Tile

        //theMap.PrintMap();

        GenerateTiles();


        theSurface2D.BuildNavMesh();
    }

    protected void FillSquareInMap(int value, int x1, int y1, int width, int height)
    {
        for (int x = x1; x < x1+width; x++)
        {
            for (int y = y1; y < y1+height; y++)
            {
                theMap.SetValue(x, y, value);
            }
        }
    }

    protected void FillSquareInMap(int value, Vector3Int center, int width, int height)
    {
        //int xMax = width / 2 + center.x;
        //int yMax = height / 2 + center.y;
        int xMin = center.x - (width/2);
        int yMin = center.y - (height/2);
        //for (int x = xMin; x < xMax; x++)
        //{
        //    for (int y = yMin; y < yMax; y++)
        //    {
        //        theMap.SetValue(x, y, value);
        //    }
        //}
        FillSquareInMap(value, xMin, yMin, width, height);
    }

    protected int CheckEdgeType( int x, int y, int outValue)
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

        return (int)MAP_EDGE_TYPE.NOT;
    }

    bool CheckIsOut(int toCompare, int inValue, int edgeValue)
    {
        if (toCompare == inValue)
            return false;
        if (toCompare/100*100 == edgeValue)
            return false;
        if (toCompare == OneMap.EDGE_VALUE)
            return false;
        return true;
    }

    protected int CheckEdgeTypeIgnoreOut(int x, int y, int value, int edgeValue)
    {
        bool bL = CheckIsOut(theMap.GetValue(x - 1, y), value, edgeValue);
        bool bR = CheckIsOut(theMap.GetValue(x + 1, y), value, edgeValue);
        if (CheckIsOut(theMap.GetValue(x, y + 1), value, edgeValue))        //上方有邊界
        {
            if (bL)  //左方也有邊界
                return (int)MAP_EDGE_TYPE.LU;
            else if (bR)  //右方也有邊界
                return (int)MAP_EDGE_TYPE.RU;
            return (int)MAP_EDGE_TYPE.UU;
        }
        if (CheckIsOut(theMap.GetValue(x, y - 1), value, edgeValue))        //下方有邊界
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

        if (CheckIsOut(theMap.GetValue(x - 1, y + 1), value, edgeValue))
            return (int)MAP_EDGE_TYPE.LU_S;
        if (CheckIsOut(theMap.GetValue(x + 1, y + 1), value, edgeValue))
            return (int)MAP_EDGE_TYPE.RU_S;
        if (CheckIsOut(theMap.GetValue(x - 1, y - 1), value, edgeValue))
            return (int)MAP_EDGE_TYPE.LD_S;
        if (CheckIsOut(theMap.GetValue(x + 1, y - 1), value, edgeValue))
            return (int)MAP_EDGE_TYPE.RD_S;

        return (int)MAP_EDGE_TYPE.NOT;
    }

    protected void EdgeDetectInMap( int value, int outValue, int edgeTypeValue, Vector3Int center, int width, int height)
    {
        int xMax = width / 2 + center.x;
        int yMax = height / 2 + center.y;
        int xMin = xMax - width;
        int yMin = yMax - height;
        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++) 
            { 
                if (theMap.GetValue(x, y) == value )
                {
                    int newValue;
                    if (outValue < 0)
                    {
                        newValue = CheckEdgeTypeIgnoreOut(x, y, value, edgeTypeValue);
                    }
                    else
                    {
                        newValue = CheckEdgeType(x, y, outValue);
                    }
                    if (newValue != (int)MAP_EDGE_TYPE.NOT)
                        theMap.SetValue(x, y, edgeTypeValue + newValue);
                }
            }
        }
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
                else if (value == (int)TILE_TYPE.BLOCK)
                {
                    blockTM.SetTile((Vector3Int)cd, blockGroup.GetOneTile());
                }
                else if (value >= (int)TILE_TYPE.EDGE_VALUE)
                {
                    int edgeType = value % 100;
                    int type = value - edgeType;
                    TileEdgeGroup eTG = null;
                    Tilemap tM = groundTM;
                    if (type == (int)TILE_TYPE.GRASS_EDGE)
                    {
                        eTG = grassEdgeGroup;
                    }
                    else if (type == (int)TILE_TYPE.DIRT_EDGE)
                    {
                        eTG = dirtEdgeGroup;
                    }
                    else if (type == (int)TILE_TYPE.BLOCK_EDGE)
                    {
                        //TODO: 要改用 Block 的 TM
                        eTG = blockEdgeGroup;
                        tM = blockTM;
                    }
                    
                    tM.SetTile((Vector3Int)cd, eTG.GetTile((MAP_EDGE_TYPE)edgeType));
                }
            }
        }
    }

}
