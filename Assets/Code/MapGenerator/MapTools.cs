using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class OneMap
{
    public int mapWidth;
    public int mapHeight;

    public const int EDGE_VALUE = 0;
    public const int DEFAULT_VALUE = 1;
    public const int INVALID_VALUE = -9999;

    public Vector2Int mapCenter;
    public int xMin, xMax;      //Min, Max 都是 Included
    public int yMin, yMax;

    const int edgeWidth = 2;
    int arrayWidth, arrayHeight;
    int arrayXshift, arrayYshift;

    int[][] mapArray;

    bool IsValid(int x, int y)
    {
        if (x < (xMin - edgeWidth) || x > (xMax + edgeWidth)
            || y < (yMin - edgeWidth) || y > (yMax + edgeWidth))
        {
            Debug.Log("OneMap Inavlid Coordinate !! ( " + x + ", " + y + " )");
            return false;
        }
        return true;
    }
    public bool IsValid(Vector2Int coord)
    {
        if (coord.x < (xMin - edgeWidth) || coord.x > (xMax + edgeWidth)
            || coord.y < (yMin - edgeWidth) || coord.y > (yMax + edgeWidth))
            return false;
        return true;
    }

    public void SetValue (int x, int y, int value)
    {
        if (!IsValid(x, y)) { return; }
        mapArray[x + arrayXshift][y + arrayYshift] = value;
    }
    public void SetValue(Vector2Int coord, int value)
    {
        if (!IsValid(coord))
        {
            return;
        }
        mapArray[coord.x + arrayXshift][coord.y + arrayYshift] = value;
    }

    public int GetValue(int x, int y)
    {
        if (IsValid(x, y))
        {
            return mapArray[x + arrayXshift][y + arrayYshift];
        }
        return INVALID_VALUE;
    }

    public int GetValue(Vector2Int coord)
    {
        if (IsValid(coord))
        {
            return mapArray[coord.x + arrayXshift][coord.y + arrayYshift];
        }
        return INVALID_VALUE;
    }

    public void InitMap(Vector2Int center, int width, int height, int initValue = DEFAULT_VALUE)
    {
        mapCenter = center;

        xMin = -width / 2 + mapCenter.x; yMin = -height / 2 + mapCenter.y;
        xMax = xMin + width - 1; yMax = yMin + height - 1;
        mapWidth = width;
        mapHeight = height;

        arrayWidth = mapWidth + edgeWidth + edgeWidth;
        arrayHeight = mapHeight + edgeWidth + edgeWidth;

        arrayXshift = -xMin + edgeWidth;
        arrayYshift = -yMin + edgeWidth;

        mapArray = new int[arrayWidth][];
        for (int i = 0; i < arrayWidth; i++)
        {
            mapArray[i] = new int[arrayHeight];
            for (int j = 0; j < arrayHeight; j++)
            {
                mapArray[i][j] = initValue;
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
                mapArray[arrayWidth - 1 - e][y] = EDGE_VALUE;
            }
        }
    }

    public void FillValue( int _xMin, int _yMin, int width, int height, int value)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y=0; y<height; y++)
            {
                mapArray[x + _xMin + arrayXshift][y + _yMin + arrayYshift] = value;
            }
        }
    }

    public void FillValueAll( int value)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                mapArray[x + arrayXshift][y + arrayYshift] = value;
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
                str += (mapArray[x][y].ToString() + ",");
            }
            str += "\n";
        }
        Debug.Log(str);
    }

    public void FillTile(int _xMin, int _yMin, int width, int height, int checkValue, Tilemap tm, Tile tile)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapArray[x + _xMin + arrayXshift][y + _yMin + arrayYshift] == checkValue)
                {
                    tm.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), tile);
                }
            }
        }
    }


    protected bool IsOut( int value, int inValue, int outValue)
    {
        return value != inValue && (outValue == OneMap.INVALID_VALUE || value == outValue);
    }

    public void FillTile(int _xMin, int _yMin, int width, int height, int checkValue, Tilemap tm, Tilemap egdeTM, TileGroup tg, TileEdgeGroup te, bool outEdge = true, int outValue = OneMap.INVALID_VALUE)
    {
        if (outEdge)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                //for (int y = height-1; y >=0 ; y--)
                {
                    int value = mapArray[x + _xMin + arrayXshift][y + _yMin + arrayYshift];
                    if (value == checkValue)
                    {
                        tm.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), tg.GetOneTile());
                    }
                    else
                    {
                        if (IsOut(value, checkValue, outValue))
                        {
                            //CheckAndSetOutsideEdgeTile(_xMin + x, _yMin + y, checkValue, egdeTM, te);
                            Tile t = te.GetOutEdgeTile(this, _xMin + x, _yMin + y, checkValue, outValue);
                            if (t != null)
                                egdeTM.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), t);
                        }
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapArray[x + _xMin + arrayXshift][y + _yMin + arrayYshift] == checkValue)
                    {
                        //if (!CheckAndSetInsideEdgeTile(_xMin + x, _yMin + y, checkValue, egdeTM, te, outValue))
                        //    tm.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), tg.GetOneTile());
                        Tile t = te.GetInEdgeTile(this, _xMin + x, _yMin + y, checkValue, outValue);
                        if (t == null)
                            tm.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), tg.GetOneTile());
                        else
                            egdeTM.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), t);
                    }
                }
            }
        }
    }

    public void FillTileAll(int checkValue, Tilemap tm, Tile tile)
    {
        FillTile(xMin, yMin, mapWidth, mapHeight, checkValue, tm, tile);
    }


    public void FillTileAll(int checkValue, Tilemap tm, Tilemap egdeTM, TileGroup tg, TileEdgeGroup te, bool outEdge = true, int outValue = OneMap.INVALID_VALUE)
    {
        FillTile(xMin, yMin, mapWidth, mapHeight, checkValue, tm, egdeTM, tg, te, outEdge, outValue);
    }

    //指定位置已經是外部 Tile ，檢查是否為邊界 Tile
    protected bool CheckAndSetOutsideEdgeTile(int x, int y, int inValue, Tilemap tm, TileEdgeGroup te)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Vector3Int posD = new Vector3Int(x, y - 1, 0);
        bool UU = GetValue(x, y + 1) == inValue;
        bool DD = GetValue(x, y - 1) == inValue;
        bool LL = GetValue(x - 1, y) == inValue;
        bool RR = GetValue(x + 1, y) == inValue;
        if (UU)
        {
            if (LL)
            {
                //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RD_S));
                te.SetTile(tm, MAP_EDGE_TYPE.RD_S, pos);
            }
            else if (RR)
            {
                //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LD_S));
                te.SetTile(tm, MAP_EDGE_TYPE.LD_S, pos);
            }
            else
            {
                //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.DD));
                te.SetTile(tm, MAP_EDGE_TYPE.DD, pos);
            }
            return true;
        }
        if (DD)
        {
            if (LL)
            {
                //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RU_S));
                te.SetTile(tm, MAP_EDGE_TYPE.RU_S, pos);
            }
            else if (RR)
            {
                //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LU_S));
                te.SetTile(tm, MAP_EDGE_TYPE.LU_S, pos);
            }
            else
            {
                //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.UU));
                te.SetTile(tm, MAP_EDGE_TYPE.UU, pos);
            }
            return true;
        }

        if (LL)
        {
            //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RR));
            te.SetTile(tm, MAP_EDGE_TYPE.RR, pos);
            return true;
        }
        if (RR)
        {
            //tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LL));
            te.SetTile(tm, MAP_EDGE_TYPE.LL, pos);
            return true;
        }

        bool LU = GetValue(x - 1, y + 1) == inValue;
        bool RU = GetValue(x + 1, y + 1) == inValue;
        bool LD = GetValue(x - 1, y - 1) == inValue;
        bool RD = GetValue(x + 1, y - 1) == inValue;
        if (LU)
        {
            //tm.SetTile(pos, te.RD);
            te.SetTile(tm, MAP_EDGE_TYPE.RD, pos);
            return true;
        }
        else if (RU)
        {
            //tm.SetTile(pos, te.LD);
            te.SetTile(tm, MAP_EDGE_TYPE.LD, pos);
            return true;
        }
        else if (LD)
        {
            //tm.SetTile(pos, te.RU);
            te.SetTile(tm, MAP_EDGE_TYPE.RU, pos);
            return true;
        }
        else if (RD)
        {
            //tm.SetTile(pos, te.LU);
            te.SetTile(tm, MAP_EDGE_TYPE.LU, pos);
            return true;
        }
        return false;
    }

    //指定位置已經是內部 Tile ，檢查是否為邊界 Tile
    protected bool CheckAndSetInsideEdgeTile(int x, int y, int inValue, Tilemap tm, TileEdgeGroup te, int outValue = OneMap.INVALID_VALUE)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Vector3Int posD = new Vector3Int(x, y - 1, 0);

        bool UU = IsOut(GetValue(x, y + 1), inValue, outValue);
        bool DD = IsOut(GetValue(x, y - 1), inValue, outValue);
        bool LL = IsOut(GetValue(x - 1, y), inValue, outValue);
        bool RR = IsOut(GetValue(x + 1, y), inValue, outValue);
        if (UU)
        {
            if (LL)
            {
                te.SetTile(tm, MAP_EDGE_TYPE.LU, pos);
            }
            else if (RR)
            {
                te.SetTile(tm, MAP_EDGE_TYPE.RU, pos);
            }
            else
            {
                te.SetTile(tm, MAP_EDGE_TYPE.UU, pos);
            }
            return true;
        }
        if (DD)
        {
            if (LL)
            {
                te.SetTile(tm, MAP_EDGE_TYPE.LD, pos);
            }
            else if (RR)
            {
                te.SetTile(tm, MAP_EDGE_TYPE.RD, pos);
            }
            else
            {
                te.SetTile(tm, MAP_EDGE_TYPE.DD, pos);
            }
            return true;
        }

        if (LL)
        {
            te.SetTile(tm, MAP_EDGE_TYPE.LL, pos);
            return true;
        }
        if (RR)
        {
            te.SetTile(tm, MAP_EDGE_TYPE.RR, pos);
            return true;
        }

        bool LU = IsOut(GetValue(x - 1, y + 1), inValue, outValue);
        bool RU = IsOut(GetValue(x + 1, y + 1), inValue, outValue);
        bool LD = IsOut(GetValue(x - 1, y - 1), inValue, outValue);
        bool RD = IsOut(GetValue(x + 1, y - 1), inValue, outValue);
        if (LU)
        {
            te.SetTile(tm, MAP_EDGE_TYPE.LU_S, pos);
            return true;
        }
        else if (RU)
        {
            te.SetTile(tm, MAP_EDGE_TYPE.RU_S, pos);
            return true;
        }
        else if (LD)
        {
            te.SetTile(tm, MAP_EDGE_TYPE.LD_S, pos);
            return true;
        }
        else if (RD)
        {
            te.SetTile(tm, MAP_EDGE_TYPE.RD_S, pos);
            return true;
        }
        return false;
    }

}

