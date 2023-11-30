using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_PerlinNoise : MG_TerrainBase
{
    public int NoiseScaleOn256 = 5;
    public float highRatio = 0.35f;
    public float lowRatio = 0.35f;
    public bool outEdge = true;

    protected enum MY_VALUE     //注意不要跟 OneMap 的預設值衝突
    {
        NORMAL = 11,
        LOW = 12,
        HIGH = 13,
        HIGH_2 = 14,
        HIGH_3 = 15,
        HIGH_4 = 16,
    }


    protected virtual int GetMapValue(float perlinValue)
    {
        if (perlinValue > 1.0f - highRatio)
        {
            if (perlinValue > 1.0f - highRatio + 0.3f)
                return (int)MY_VALUE.HIGH_4;
            else if (perlinValue > 1.0f - highRatio + 0.2f)
                return  (int)MY_VALUE.HIGH_3;
            else if (perlinValue > 1.0f - highRatio + 0.1f)
                return  (int)MY_VALUE.HIGH_2;
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
        float noiseScale = (float)NoiseScaleOn256  / 256.0f;
        float randomSscale = 10.0f;
        float xShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        float yShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int y= theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
            {
                float rd = Mathf.PerlinNoise((float)x * noiseScale + xShift, (float)y * noiseScale + yShift);
                theCellMap.SetValue(x, y, GetMapValue(rd));
            }
        }
    }

    protected override void FillTiles()
    {
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.NORMAL, groundTM, planTG);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.LOW, groundTM, groundTM, lowTG, lowEdgeTG, outEdge, (int)MY_VALUE.NORMAL);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.NORMAL);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH_2, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.HIGH);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH_3, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.HIGH_2);
        theCellMap.GetOneMap().FillTileAll((int)MY_VALUE.HIGH_4, blockTM, blockTM, highTG, highEdgeTG, outEdge, (int)MY_VALUE.HIGH_3);
    }

}
