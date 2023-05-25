using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileGroupBeta : TileGroupBase
{
    [System.Serializable]
    public class TileInfo
    {
        public Tile tile;
        public float rate;
    }
    public Tile baseTile;
    //public Tile[] decorateTiles;
    //public float[] decorateRates;
    public TileInfo[] decorateTiles;
    public override Tile GetOneTile()
    {
        if (decorateTiles.Length > 0 )
        {
            float rd = Random.Range(0, 1.0f);
            float rdSum = 0;

            for (int i=0; i < decorateTiles.Length; i++)
            {
                rdSum += decorateTiles[i].rate;
                if (rd < rdSum)
                    return decorateTiles[i].tile;
            }
        }
        return baseTile;
    }
}

public class TileGroupBetaData : TileGroupDataBase
{
    public TileGroupBeta betaData;


    public override TileGroupBase GetTileGroup()
    {
        return betaData;
    }
}
