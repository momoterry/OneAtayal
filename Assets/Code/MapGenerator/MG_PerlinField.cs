using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MiniMap;

public class MG_PerlinField : MG_PerlinNoise
{
    public MapDecadeGenerator decadeGenerator;
    public int edgeWidth = 4;

    protected override int GetMapValue(float perlinValue)
    {
        if (perlinValue > 1.0f - highRatio)
        {
            return (int)MY_VALUE.HIGH;
        }
        else if (perlinValue < lowRatio)
        {
            return (int)MY_VALUE.LOW;
        }
        else
            return (int)MY_VALUE.NORMAL;
    }

    protected override void GenerateCellMap()
    {
        base.GenerateCellMap();

        //theCellMap.GetOneMap

        //暴力法挖營地，TODO: 用計算的方式放上營地
        for (int x = -3; x <= 3; x++)
        {
            for (int y = -3; y <= 3; y++)
            {
                theCellMap.SetValue(x, y, (int)MY_VALUE.NORMAL);
            }
        }

        ModifyMapEdge();
    }

    protected override void FillTiles()
    {
        base.FillTiles();

        if (decadeGenerator)
        {
            DecadeGenerateParameter p = new DecadeGenerateParameter();
            p.mapValue = (int)MY_VALUE.HIGH;
            decadeGenerator.BuildAll(theCellMap.GetOneMap(), p);
        }
    }

    protected void ModifyMapEdge()
    {
        //邊界
        if (edgeWidth >= mapCellWidthH || edgeWidth > mapCellHeightH)
            return;
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                theCellMap.SetValue(x, theCellMap.GetYMin() + i, (int)MY_VALUE.HIGH);
                theCellMap.SetValue(x, theCellMap.GetYMax() - i, (int)MY_VALUE.HIGH);
            }
            if (theCellMap.GetValue(x, theCellMap.GetYMin() + edgeWidth) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(x, theCellMap.GetYMin() + edgeWidth, (int)MY_VALUE.NORMAL);
            if (theCellMap.GetValue(x, theCellMap.GetYMax() - edgeWidth) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(x, theCellMap.GetYMax() - edgeWidth, (int)MY_VALUE.NORMAL);
        }

        for (int y = theCellMap.GetYMin() + edgeWidth; y <= theCellMap.GetYMax() - edgeWidth; y++)
        {
            for (int i = 0; i < edgeWidth; i++)
            {
                theCellMap.SetValue(theCellMap.GetXMin() +i, y, (int)MY_VALUE.HIGH);
                theCellMap.SetValue(theCellMap.GetXMax() -i, y, (int)MY_VALUE.HIGH);
            }
            if (theCellMap.GetValue(theCellMap.GetXMin() + edgeWidth, y) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(theCellMap.GetXMin() + edgeWidth, y, (int)MY_VALUE.NORMAL);
            if (theCellMap.GetValue(theCellMap.GetXMax() - edgeWidth, y) == (int)MY_VALUE.LOW)
                theCellMap.SetValue(theCellMap.GetXMax() - edgeWidth, y, (int)MY_VALUE.NORMAL);
        }
    }

}
