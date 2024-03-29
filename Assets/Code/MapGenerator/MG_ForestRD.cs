using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;


public class MG_ForestRD : MapGeneratorBase
{
    public int mapWidth = 12;
    public int mapHeight = 16;
    public int borderWidth = 4;
    public Vector3Int mapCenter;

    public Tilemap groundTM;
    public Tilemap highTM;
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
        EdgeDetectInMap((int)TILE_TYPE.DIRT, (int)TILE_TYPE.DIRT_EDGE, mapCenter, mapWidth, mapHeight);
        EdgeDetectInMap((int)TILE_TYPE.BLOCK, (int)TILE_TYPE.BLOCK_EDGE, mapCenter, mapWidth + borderWidth, mapHeight + borderWidth);


        //==== 畫地圖結束，實裝 Tile

        //theMap.PrintMap();

        GenerateTiles();


        GenerateNavMesh(theSurface2D);
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

    //protected int CheckEdgeType( int x, int y, int outValue)
    //{
    //    bool bL = (theMap.GetValue(x - 1, y) == outValue);
    //    bool bR = (theMap.GetValue(x + 1, y) == outValue);
    //    if (theMap.GetValue(x, y+1) == outValue)        //上方有邊界
    //    {
    //        if (bL)  //左方也有邊界
    //            return (int)MAP_EDGE_TYPE.LU;
    //        else if (bR)  //右方也有邊界
    //            return (int)MAP_EDGE_TYPE.RU;
    //        return (int)MAP_EDGE_TYPE.UU;
    //    }
    //    if (theMap.GetValue(x, y - 1) == outValue)        //下方有邊界
    //    {
    //        if (bL)  //左方也有邊界
    //            return (int)MAP_EDGE_TYPE.LD;
    //        else if (bR)  //右方也有邊界
    //            return (int)MAP_EDGE_TYPE.RD;
    //        return (int)MAP_EDGE_TYPE.DD;
    //    }
    //    if (bL)
    //        return (int)MAP_EDGE_TYPE.LL;
    //    if (bR)
    //        return (int)MAP_EDGE_TYPE.RR;
        
    //    if (theMap.GetValue(x - 1, y + 1) == outValue)
    //        return (int)MAP_EDGE_TYPE.LU_S;
    //    if (theMap.GetValue(x + 1, y + 1) == outValue)
    //        return (int)MAP_EDGE_TYPE.RU_S;
    //    if (theMap.GetValue(x - 1, y - 1) == outValue)
    //        return (int)MAP_EDGE_TYPE.LD_S;
    //    if (theMap.GetValue(x + 1, y - 1) == outValue)
    //        return (int)MAP_EDGE_TYPE.RD_S;

    //    return (int)MAP_EDGE_TYPE.NOT;
    //}

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

    protected int CheckEdgeType(int x, int y, int value, int edgeValue)
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

    protected void EdgeDetectInMap(int value, int edgeTypeValue, int x1, int y1, int width, int height)
    {
        int x2 = x1 + width;
        int y2 = y1 + height;
        for (int x = x1; x < x2; x++)
        {
            for (int y = y1; y < y2; y++)
            {
                if (theMap.GetValue(x, y) == value)
                {
                    int newValue = CheckEdgeType(x, y, value, edgeTypeValue);
                    if (newValue != (int)MAP_EDGE_TYPE.NOT)
                        theMap.SetValue(x, y, edgeTypeValue + newValue);
                }
            }
        }
    }

    protected void EdgeDetectInMap( int value, int edgeTypeValue, Vector3Int center, int width, int height)
    {
        int x1 =  center.x - width / 2;
        int y1 = center.y - height / 2;
        EdgeDetectInMap(value, edgeTypeValue, x1, y1, width, height);
        //int xMax = width / 2 + center.x;
        //int yMax = height / 2 + center.y;
        //int xMin = xMax - width;
        //int yMin = yMax - height;
        //for (int x = xMin; x < xMax; x++)
        //{
        //    for (int y = yMin; y < yMax; y++) 
        //    { 
        //        if (theMap.GetValue(x, y) == value )
        //        {
        //            int newValue = CheckEdgeType(x, y, value, edgeTypeValue);
        //            //if (outValue < 0)
        //            //{
        //            //    newValue = CheckEdgeType(x, y, value, edgeTypeValue);
        //            //}
        //            //else
        //            //{
        //            //    newValue = CheckEdgeType(x, y, outValue);
        //            //}
        //            if (newValue != (int)MAP_EDGE_TYPE.NOT)
        //                theMap.SetValue(x, y, edgeTypeValue + newValue);
        //        }
        //    }
        //}
    }

    protected void GenerateTiles( int x1, int y1, int width, int height)
    {
        int x2 = x1 + width;
        int y2 = y1 + height;
        Vector2Int cd = Vector2Int.zero;
        for (int x = x1; x < x2; x++)
        {
            for (int y = y1; y < y2; y++)
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
                        eTG = blockEdgeGroup;
                        tM = blockTM;
                        //if (highTM && edgeType == (int)MAP_EDGE_TYPE.UU)
                        //    tM = highTM;
                        //else
                        //    tM = blockTM;
                    }

                    tM.SetTile((Vector3Int)cd, eTG.GetTile((MAP_EDGE_TYPE)edgeType));
                }
            }
        }
    }

    protected void GenerateTiles()
    {
        GenerateTiles(theMap.xMin, theMap.yMin, theMap.mapWidth, theMap.mapHeight);
        //Vector2Int cd = Vector2Int.zero;
        //for ( int x = theMap.xMin; x < theMap.xMax; x++ )
        //{
        //    for ( int y = theMap.yMin; y < theMap.yMax; y++ )
        //    {
        //        cd.x = x;
        //        cd.y = y;
        //        int value = theMap.GetValue(x, y);
        //        if (value == (int)TILE_TYPE.GRASS)
        //        {
        //            groundTM.SetTile((Vector3Int)cd, grassGroup.GetOneTile());
        //        }
        //        else if (value == (int)TILE_TYPE.DIRT)
        //        {
        //            groundTM.SetTile((Vector3Int)cd, dirtGroup.GetOneTile());
        //        }
        //        else if (value == (int)TILE_TYPE.BLOCK)
        //        {
        //            blockTM.SetTile((Vector3Int)cd, blockGroup.GetOneTile());
        //        }
        //        else if (value >= (int)TILE_TYPE.EDGE_VALUE)
        //        {
        //            int edgeType = value % 100;
        //            int type = value - edgeType;
        //            TileEdgeGroup eTG = null;
        //            Tilemap tM = groundTM;
        //            if (type == (int)TILE_TYPE.GRASS_EDGE)
        //            {
        //                eTG = grassEdgeGroup;
        //            }
        //            else if (type == (int)TILE_TYPE.DIRT_EDGE)
        //            {
        //                eTG = dirtEdgeGroup;
        //            }
        //            else if (type == (int)TILE_TYPE.BLOCK_EDGE)
        //            {
        //                //TODO: 要改用 Block 的 TM
        //                eTG = blockEdgeGroup;
        //                tM = blockTM;
        //            }

        //            tM.SetTile((Vector3Int)cd, eTG.GetTile((MAP_EDGE_TYPE)edgeType));
        //        }
        //    }
        //}
    }

}
