using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_PuzzleDisjointSet : MG_ForestRD
{
    public int cellSize = 4;
    public int puzzleHeight = 6;
    public int puzzleWidth = 6;

    protected struct cellInfo
    {
        bool U, D, L, R;
    }

    private void Awake()
    {
        mapHeight = puzzleHeight * cellSize;
        mapWidth = puzzleWidth * cellSize;

        mapCenter.y = puzzleHeight / 2 * cellSize - cellSize / 2;
    }

    protected void FillCell(cellInfo cell, int x1, int y1, int width, int height)
    {
        for (int x = x1; x<x1+width; x++)
        {
            theMap.SetValue(x, y1, (int)TILE_TYPE.BLOCK);
            theMap.SetValue(x, y1+height-1, (int)TILE_TYPE.BLOCK);
        }
        for (int y = y1+1; y < y1 + height - 1; y++)
        {
            theMap.SetValue(x1, y, (int)TILE_TYPE.BLOCK);
            theMap.SetValue(x1+width-1, y, (int)TILE_TYPE.BLOCK);
        }
    }

    protected override void CreateForestMap()
    {
        cellInfo temp = new cellInfo();
        int mapX1 = mapCenter.x - mapWidth / 2;
        int mapY1 = mapCenter.y - mapHeight / 2;
        int hCellSize = cellSize / 2;
        for (int i=0; i< puzzleWidth; i++)
        {
            for (int j=0; j<puzzleHeight; j++)
            {
                int x1 = mapX1 + i * cellSize;
                int y1 = mapY1 + j * cellSize;
                FillCell(temp, x1, y1, cellSize, cellSize);
            }
        }
    }
}


public class DisjointSetUnion
{
    int size;
    int[] P;
    public void Init(int _size)
    {
        P = new int[_size];
        for (int i = 0; i < _size; i++)
            P[i] = i;
    }

    public int Find(int _id)
    {
        if (_id == P[_id])
            return _id;
        else
            return Find(P[_id]);
    }

    public void Union(int a, int b)
    {
        int Fa = Find(a);
        int Fb = Find(b);
        if (Fa != Fb)
        {
            P[Fb] = P[Fa];
        }
    }
}

