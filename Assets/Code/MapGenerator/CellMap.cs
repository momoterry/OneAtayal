using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

//翴辅 (0, 0) い丁瓜
public class CellMap
{
    protected int mapSizeX;
    protected int mapSizeY;
    protected int cellSize;
    protected int cellSizeH;    //Cell Cell ゲ﹚琌 2 计
    protected int xMin, xMax;
    protected int yMin, yMax;

    protected int[,] cells;

    public int GetWidth() { return mapSizeX; }
    public int GetHeight() { return mapSizeY; }
    public int GetXMin() { return xMin; }
    public int GetXMax() { return xMax; }
    public int GetYMin() { return yMin; }
    public int GetYMax() { return yMax; }
    public int GetCellSize() { return cellSize; }

    public virtual void InitMap(int mapHalfSizeWithoutCenter, int cellHalfSize)
    {
        mapSizeX = mapHalfSizeWithoutCenter + mapHalfSizeWithoutCenter + 1;
        mapSizeY = mapHalfSizeWithoutCenter + mapHalfSizeWithoutCenter + 1;
        cellSizeH = cellHalfSize;
        cellSize = cellSizeH + cellSizeH;
        xMin = yMin = -mapHalfSizeWithoutCenter;
        xMax = yMax = mapHalfSizeWithoutCenter;

        cells = new int[mapSizeX,mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y= 0; y < mapSizeY; y++)
            {
                cells[x, y] = 0;
            }
        }
    }

    public Vector2Int GetCellCenterCoord(int x, int y) { return new Vector2Int(x * cellSize, y * cellSize); }
    public Vector2Int GetCellMinCoord(int x, int y) { return new Vector2Int(x * cellSize - cellSizeH, y * cellSize - cellSizeH); }
    public int GetValue(int x, int y) { return cells[x-xMin,y-yMin]; }
    public virtual void SetValue(int x, int y, int value) { cells[x - xMin, y - yMin] = value; }

}

// 讽 CellSize > 1 絋玂 Cell(0, 0)  ゲ﹚いみ璝 CellSize 琌案计いみ Cell(0,0) いァ
// 讽 cellMapWidth ┪ cellMapHeight 计穦キ颗 案计オ跋办穦祔
// 

public class OneCellMap : CellMap
{
    protected OneMap myOneMap;

    protected int oneMapXMin, oneMapXMax;
    protected int oneMapYMin, oneMapYMax;

    public int GetOneMapXMin() { return oneMapXMin; }
    public int GetOneMapXMax() { return oneMapXMax; }
    public int GetOneMapYMin() { return oneMapYMin; }
    public int GetOneMapYMax() { return oneMapYMax; }

    public OneMap GetOneMap() { return myOneMap; }
    public void InitCellMap(int cellMapWidthH, int cellMapHeightH, int _cellSize)
    {
        mapSizeX = cellMapWidthH + cellMapWidthH + 1;
        mapSizeY = cellMapHeightH + cellMapHeightH + 1;
        cellSizeH = _cellSize / 2;
        cellSize = _cellSize;
        //xMax =  cellMapWidth / 2;
        //yMax = cellMapHeight / 2;
        //xMin = xMax - mapSizeX;
        //yMin = yMax - mapSizeY;
        xMin = -cellMapWidthH;
        yMin = -cellMapHeightH;
        xMax = cellMapWidthH;
        yMax = cellMapHeightH;

        cells = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                cells[x, y] = 0;
            }
        }

        myOneMap = new OneMap();
        myOneMap.InitMap(Vector2Int.zero, mapSizeX * cellSize, mapSizeY * cellSize, 0);
        oneMapXMin = myOneMap.xMin;
        oneMapXMax = myOneMap.xMax;
        oneMapYMin = myOneMap.yMin;
        oneMapYMax = myOneMap.yMax;
    }

    public override void SetValue(int x, int y, int value)
    {
        base.SetValue(x, y, value);
        int x1 = x * cellSize - cellSizeH, y1 = y * cellSize - cellSizeH;
        //Debug.Log("To Fill Value From: " + x1 +", " + y1);
        myOneMap.FillValue(x1, y1, cellSize, cellSize, value);
    }

    public Vector3 GetCellCenterPosition(int x, int y)
    {
        return myOneMap.GetCenterPosition(GetCellCenterCoord(x, y));
    }
}
