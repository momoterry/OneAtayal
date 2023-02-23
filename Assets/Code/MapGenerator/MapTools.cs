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
    public int xMin, xMax;      //Min, Max ³£¬O Included
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
    protected bool IsValid(Vector2Int coord)
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
        //xMax = width / 2 + mapCenter.x;
        //yMax = height / 2 + mapCenter.y;
        //xMin = xMax - width;
        //yMin = yMax - height;
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

    public void FillTile( int _xMin , int _yMin, int width, int height, int checkValue, Tilemap tm, Tilemap egdeTM, TileGroup tg, TileEdge2LGroup te)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapArray[x + _xMin + arrayXshift][y + _yMin + arrayYshift] == checkValue)
                {
                    tm.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), tg.GetOneTile());
                }
                else
                {
                    CheckEdgeTile(_xMin + x, _yMin + y, checkValue, egdeTM, te);
                }
            }
        }
    }

    public void FillTile(int _xMin, int _yMin, int width, int height, int checkValue, Tilemap tm, Tilemap egdeTM, TileGroup tg, TileEdgeGroup te)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapArray[x + _xMin + arrayXshift][y + _yMin + arrayYshift] == checkValue)
                {
                    tm.SetTile(new Vector3Int(_xMin + x, _yMin + y, 0), tg.GetOneTile());
                }
                else
                {
                    CheckEdgeTile(_xMin + x, _yMin + y, checkValue, egdeTM, te);
                }
            }
        }
    }

    public void FillTileAll(int checkValue, Tilemap tm, Tile tile)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (mapArray[x + arrayXshift][y + arrayYshift] == checkValue)
                {
                    tm.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }

    public void FillTileAll(int checkValue, Tilemap tm, Tilemap egdeTM, TileGroup tg, TileEdge2LGroup te)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (mapArray[x + arrayXshift][y + arrayYshift] == checkValue)
                {
                    tm.SetTile(new Vector3Int(x, y, 0), tg.GetOneTile());
                }
                else
                {
                    CheckEdgeTile(x, y, checkValue, egdeTM, te);
                }
            }
        }
    }

    public void FillTileAll(int checkValue, Tilemap tm, Tilemap egdeTM, TileGroup tg, TileEdgeGroup te)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (mapArray[x + arrayXshift][y + arrayYshift] == checkValue)
                {
                    tm.SetTile(new Vector3Int(x, y, 0), tg.GetOneTile());
                }
                else
                {
                    CheckEdgeTile(x, y, checkValue, egdeTM, te);
                }
            }
        }
    }

    protected void CheckEdgeTile(int x, int y, int value, Tilemap tm, TileEdgeGroup te)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Vector3Int posD = new Vector3Int(x, y - 1, 0);
        bool UU = GetValue(x, y + 1) == value;
        bool DD = GetValue(x, y - 1) == value;
        bool LL = GetValue(x - 1, y) == value;
        bool RR = GetValue(x + 1, y) == value;
        if (UU)
        {
            if (LL)
            {
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RD_S));
            }
            else if (RR)
            {
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LD_S));
            }
            else
            {
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.DD));
            }
            return;
        }
        if (DD)
        {
            if (LL)
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RU_S));
            else if (RR)
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LU_S));
            else
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.UU));
            return;
        }

        if (LL)
        {
            tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RR));
            return;
        }
        if (RR)
        {
            tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LL));
            return;
        }

        bool LU = GetValue(x - 1, y + 1) == value;
        bool RU = GetValue(x + 1, y + 1) == value;
        bool LD = GetValue(x - 1, y - 1) == value;
        bool RD = GetValue(x + 1, y - 1) == value;
        if (LU)
        {
            tm.SetTile(pos, te.RD);
        }
        else if (RU)
        {
            tm.SetTile(pos, te.LD);
        }
        else if (LD)
            tm.SetTile(pos, te.RU);
        else if (RD)
            tm.SetTile(pos, te.LU);

    }
    protected void CheckEdgeTile(int x, int y, int value, Tilemap tm, TileEdge2LGroup te)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Vector3Int posD = new Vector3Int(x, y-1, 0);
        bool UU = GetValue(x, y + 1) == value;
        bool DD = GetValue(x, y - 1) == value;
        bool LL = GetValue(x - 1, y) == value;
        bool RR = GetValue(x + 1, y) == value;
        if (UU)
        {
            if (LL)
            {
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RD_S));
                tm.SetTile(posD, te.RD_S2);
            }
            else if (RR)
            {
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LD_S));
                tm.SetTile(posD, te.LD_S2);
            }
            else
            {
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.DD));
                tm.SetTile(posD, te.DD2);
            }
            return;
        }
        if (DD)
        {
            if (LL)
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RU_S));
            else if (RR)
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LU_S));
            else
                tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.UU));
            return;
        }

        if (LL)
        {
            tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.RR));
            return;
        }
        if (RR)
        {
            tm.SetTile(pos, te.GetTile(MAP_EDGE_TYPE.LL));
            return;
        }

        bool LU = GetValue(x - 1, y + 1) == value;
        bool RU = GetValue(x + 1, y + 1) == value;
        bool LD = GetValue(x - 1, y - 1) == value;
        bool RD = GetValue(x + 1, y - 1) == value;
        if (LU)
        {
            tm.SetTile(pos, te.RD);
            tm.SetTile(posD, te.RD2);
        }
        else if (RU)
        {
            tm.SetTile(pos, te.LD);
            tm.SetTile(posD, te.LD2);
        }
        else if (LD)
            tm.SetTile(pos, te.RU);
        else if (RD)
            tm.SetTile(pos, te.LU);

    }

}

