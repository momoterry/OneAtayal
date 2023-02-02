using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_ForeAlpha : MG_ForestRD
{
    public float blockRatio = 0.2f;
    public float dirtRatio = 0.1f;
    protected int blockSize = 4;

    protected override void CreateForestMap()
    {
        int xNum = mapWidth / blockSize;
        int yNum = mapHeight / blockSize;

        int x = 0;
        int y = -mapHeight / 2 + blockSize / 2;
        for (int ix =0; ix < xNum; ix++)
        {
            x = -mapWidth / 2 + blockSize / 2;
            for (int iy=0; iy < yNum; iy++)
            {
                float rd = Random.Range(0, 1.0f);
                if (rd < blockRatio)
                {
                    FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(x, y, 0), 4, 4);
                }
                else if (rd < blockRatio + dirtRatio)
                {
                    FillSquareInMap((int)TILE_TYPE.DIRT, mapCenter + new Vector3Int(x, y, 0), 4, 4);
                }
                x += blockSize;
            }
            y += blockSize;
        }
    }
}
