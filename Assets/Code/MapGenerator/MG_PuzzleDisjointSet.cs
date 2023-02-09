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
        public int ID;
        public bool U, D, L, R;
    }

    protected DisjointSetUnion puzzleDSU = new DisjointSetUnion();
    protected cellInfo[][] puzzleMap;

    protected class wallInfo
    {
        public wallInfo( int _id1, int _id2)
        {
            cell_ID_1 = _id1;
            cell_ID_2 = _id2;
        }
        public int cell_ID_1;
        public int cell_ID_2;
    }
    protected List<wallInfo> wallList = new List<wallInfo>();


    private void Awake()
    {
        mapHeight = puzzleHeight * cellSize;
        mapWidth = puzzleWidth * cellSize;

        mapCenter.y = mapHeight/2 - cellSize / 2;
    }

    protected void FillCell(cellInfo cell, int x1, int y1, int width, int height)
    {
        int x2 = x1 + width - 1;
        int y2 = y1 + height - 1;
        theMap.SetValue(x1, y1, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x1, y2, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x2, y1, (int)TILE_TYPE.BLOCK);
        theMap.SetValue(x2, y2, (int)TILE_TYPE.BLOCK);
        if (!cell.U)
        {
            for (int x = x1 + 1; x < x2; x++)
                theMap.SetValue(x, y1, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.D)
        {
            for (int x = x1 + 1; x < x2; x++)
                theMap.SetValue(x, y2, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.L)
        {
            for (int y = y1 + 1; y < y2; y++)
                theMap.SetValue(x1, y, (int)TILE_TYPE.BLOCK);
        }
        if (!cell.R)
        {
            for (int y = y1 + 1; y < y2; y++)
                theMap.SetValue(x2, y, (int)TILE_TYPE.BLOCK);
        }
    }

    protected int GetCellID(int x, int y) { return y * puzzleWidth + x; }
    protected int GetCellX(int id) { return id % puzzleWidth; }
    protected int GetCellY(int id) { return id / puzzleWidth; }

    protected override void CreateForestMap()
    {
        //==== Init Puzzle Map
        puzzleDSU.Init(puzzleHeight * puzzleWidth);
        puzzleMap = new cellInfo[puzzleWidth][];
        for (int i=0; i<puzzleWidth; i++)
        {
            puzzleMap[i] = new cellInfo[puzzleHeight];
        }
        //==== Init Connection Info
        for (int x=0; x < puzzleWidth-1; x++)
        {
            for (int y=0; y < puzzleHeight-1; y++)
            {
                wallList.Add(new wallInfo(GetCellID(x, y), GetCellID(x+1,y)));
                wallList.Add(new wallInfo(GetCellID(x, y), GetCellID(x, y+1)));
            }
        }

        foreach (wallInfo w in wallList)
        {
            print("wall: " + w.cell_ID_1 + " -- " + w.cell_ID_2);
        }

        //==== 開始隨機連結 !!
        for (int loop=0; loop<10; loop++)
        {
            int rd = Random.Range(0, wallList.Count);
            //待續 ......

        }

        //==== Set up all cells
        int mapX1 = mapCenter.x - mapWidth / 2;
        int mapY1 = mapCenter.y - mapHeight / 2;
        for (int i=0; i< puzzleWidth; i++)
        {
            for (int j=0; j<puzzleHeight; j++)
            {
                int x1 = mapX1 + i * cellSize;
                int y1 = mapY1 + j * cellSize;
                FillCell(puzzleMap[i][j], x1, y1, cellSize, cellSize);
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

