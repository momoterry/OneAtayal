using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

//���I���b (0, 0) �������a��
public class CellMap
{
    protected int mapSizeX;
    protected int mapSizeY;
    protected int cellSize;
    protected int cellSizeH;    //Cell ���@�b�j�p�ACell ���w�O 2 ������
    protected int xMin, xMax;
    protected int yMin, yMax;

    protected int[,] cells;

    public int GetXMin() { return xMin; }
    public int GetXMax() { return xMax; }
    public int GetYMin() { return yMin; }
    public int GetYMax() { return yMax; }
    public int GetCellSize() { return cellSize; }

    public void InitMap(int mapHalfSizeWithoutCenter, int cellHalfSize)
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
    public void SetValue(int x, int y, int value) { cells[x - xMin, y - yMin] = value; }
}
