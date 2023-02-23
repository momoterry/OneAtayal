using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_PerlinNoise : MG_TerrainBase
{
    public int NoiseScaleOn256 = 5;
    public float highRatio = 0.35f;
    public float lowRatio = 0.35f;
    protected override void GenerateCellMap()
    {
        float noiseScale = (float)NoiseScaleOn256 * CellSize / 256.0f;
        float randomSscale = 10.0f;
        float xShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        float yShift = Random.Range(0, NoiseScaleOn256 * randomSscale);
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int y= theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
            {
                float rd = Mathf.PerlinNoise((float)x * noiseScale + xShift, (float)y * noiseScale + yShift);
                if ( rd > 1.0f - highRatio)
                {
                    theCellMap.SetValue(x, y, 3);
                }
                else if (rd < lowRatio)
                {
                    theCellMap.SetValue(x, y, 2);
                }
                else
                    theCellMap.SetValue(x, y, 1);
            }
        }
    }
}
