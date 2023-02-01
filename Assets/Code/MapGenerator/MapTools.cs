using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OneMap
{
    public int mapWidth;
    public int mapHeight;

    public const int EDGE_VALUE = 0;
    public const int DEFAULT_VALUE = 1;
    public const int INVALID_VALUE = -9999;

    public Vector2Int mapCenter;
    public int xMin, xMax;
    public int yMin, yMax;

    const int edgeWidth = 2;
    int arrayWidth, arrayHeight;
    int arrayXshift, arrayYshift;

    int[][] mapArray;

    protected bool IsValid(Vector2Int coord)
    {
        if (coord.x < (xMin - edgeWidth) || coord.x > (xMax + edgeWidth)
            || coord.y < (yMin - edgeWidth) || coord.y > (yMax + edgeWidth))
            return false;
        return true;
    }

    public void SetValue(Vector2Int coord, int value)
    {
        if (!IsValid(coord))
        {
            return;
        }
        mapArray[coord.x + arrayXshift][coord.y + arrayYshift] = value;
    }

    public int GetValue(Vector2Int coord)
    {
        if (IsValid(coord))
        {
            return mapArray[coord.x + arrayXshift][coord.y + arrayYshift];
        }
        return INVALID_VALUE;
    }

    public void InitMap(Vector2Int center, int width, int height)
    {
        mapCenter = center;
        xMax = width / 2 + mapCenter.x;
        yMax = height / 2 + mapCenter.y;
        xMin = xMax - width;
        yMin = yMax - height;
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
                mapArray[i][j] = DEFAULT_VALUE;
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

