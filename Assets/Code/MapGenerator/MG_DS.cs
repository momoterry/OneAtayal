using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Diamond Square 演算法

public class MG_DS : MG_TerrainBase
{
    protected float highRatio = 0.35f;
    protected float lowRatio = 0.35f;
    public bool outEdge = true;

    public float roughness = 0.6f;
    protected float[,] heightMap;
    protected int dsMapSize = 129;
    protected enum MY_VALUE
    {
        NORMAL = 1,
        LOW = 2,
        HIGH = 3,
        HIGH_2 = 4,
        HIGH_3 = 5,
        HIGH_4 = 6,
    }
    protected override void FillTiles()
    {
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.NORMAL, groundTM, planTG.baseTile);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.LOW, groundTM, groundTM, lowTG, lowEdgeTG, outEdge, (int)MY_VALUE.NORMAL);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.NORMAL);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH_2, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.HIGH);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH_3, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.HIGH_2);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH_4, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.HIGH_3);
    }
    protected virtual int GetMapValue(float perlinValue)
    {
        if (perlinValue > 1.0f - highRatio)
        {
            if (perlinValue > 1.0f - highRatio + 0.3f)
                return (int)MY_VALUE.HIGH_4;
            else if (perlinValue > 1.0f - highRatio + 0.2f)
                return (int)MY_VALUE.HIGH_3;
            else if (perlinValue > 1.0f - highRatio + 0.1f)
                return (int)MY_VALUE.HIGH_2;
            else
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
        dsMapSize = Mathf.NextPowerOfTwo( Mathf.Max(theCellMap.GetWidth(),theCellMap.GetHeight()) ) + 1;
        GenerateHeightMap();

        int xMin = theCellMap.GetXMin();
        int yMin = theCellMap.GetYMin();
        for (int x=0; x<theCellMap.GetWidth(); x++)
        {
            for (int y=0; y<theCellMap.GetHeight(); y++)
            {
                float rd = heightMap[x,y];
                theCellMap.SetValue(x+ xMin, y+ xMin, GetMapValue(rd));
            }
        }
    }


    void GenerateHeightMap()
    {
        int size = dsMapSize;
        //print("地圖 Size: " + size);
        heightMap = new float[size, size];

        // set initial corner values
        heightMap[0, 0] = Random.value;
        heightMap[0, size - 1] = Random.value;
        heightMap[size - 1, 0] = Random.value;
        heightMap[size - 1, size - 1] = Random.value;

        // run diamond-square algorithm
        int x, y, half, level;
        float avg, displacement;

        for (level = size - 1; level > 1; level /= 2)
        {
            half = level / 2;

            // square step
            for (x = 0; x < size - 1; x += level)
            {
                for (y = 0; y < size - 1; y += level)
                {
                    avg = heightMap[x, y] + heightMap[x + level, y] + heightMap[x, y + level] + heightMap[x + level, y + level];
                    avg /= 4.0f;

                    displacement = (Random.value - 0.5f) * roughness;
                    heightMap[x + half, y + half] = avg + displacement;
                }
            }

            // diamond step
            for (x = 0; x < size - 1; x += half)
            {
                for (y = (x + half) % level; y < size - 1; y += level)
                {
                    avg = heightMap[(x - half + size - 1) % (size - 1), y] + heightMap[(x + half) % (size - 1), y] + heightMap[x, (y + half) % (size - 1)] + heightMap[x, (y - half + size - 1) % (size - 1)];
                    avg /= 4.0f;

                    displacement = (Random.value - 0.5f) * roughness;
                    heightMap[x, y] = avg + displacement;

                    if (x == 0) heightMap[size - 1, y] = avg + displacement;
                    if (y == 0) heightMap[x, size - 1] = avg + displacement;
                }
            }

            // decrease roughness
            roughness *= 0.5f;
        }
        // normalize the height map
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;
        for (x = 0; x < size; x++)
        {
            for (y = 0; y < size; y++)
            {
                if (heightMap[x, y] < minHeight) minHeight = heightMap[x, y];
                if (heightMap[x, y] > maxHeight) maxHeight = heightMap[x, y];
            }
        }
        float range = maxHeight - minHeight;
        for (x = 0; x < size; x++)
        {
            for (y = 0; y < size; y++)
            {
                heightMap[x, y] = (heightMap[x,y] - minHeight) / range;
            }
        }
    }
}
