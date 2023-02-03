using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_ForeAlpha : MG_ForestRD
{
    public float blockRatio = 0.2f;
    public float dirtRatio = 0.1f;
    protected int blockSize = 4;

    protected void CrossFixInMap()
    {
        for (int x = theMap.xMin; x <= theMap.xMax-1; x++)
        {
            for (int y = theMap.yMin; y <= theMap.yMax-1; y++)
            {
                bool b11 = theMap.GetValue(x, y) == (int)TILE_TYPE.BLOCK;
                bool b12 = theMap.GetValue(x + 1, y) == (int)TILE_TYPE.BLOCK;
                bool b21 = theMap.GetValue(x, y + 1) == (int)TILE_TYPE.BLOCK;
                bool b22 = theMap.GetValue(x + 1, y + 1) == (int)TILE_TYPE.BLOCK;

                if (b11 && b22 && !b21 && !b12)
                {
                    theMap.SetValue(x, y, (int)TILE_TYPE.GRASS);
                    theMap.SetValue(x + 1, y + 1, (int)TILE_TYPE.GRASS);
                }
                else if (!b11 && !b22 && b21 && b12)
                {
                    theMap.SetValue(x, y + 1, (int)TILE_TYPE.GRASS);
                    theMap.SetValue(x + 1, y, (int)TILE_TYPE.GRASS);
                }
            }
            
        }
    }

    protected override void CreateForestMap()
    {
        int xNum = mapWidth / blockSize;
        int yNum = mapHeight / blockSize;

        int x = -mapWidth / 2 + blockSize / 2;
        for (int ix =0; ix < xNum; ix++)
        {
            int y = -mapHeight / 2 + blockSize / 2;
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
                y += blockSize;
            }
            x += blockSize;
        }

        //�T�O�����O�Ū�
        FillSquareInMap((int)TILE_TYPE.GRASS, mapCenter, blockSize, blockSize);

        //�ץ������ﱵ���D
        CrossFixInMap();
    }
}
