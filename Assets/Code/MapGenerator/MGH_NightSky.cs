using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MGH_NightSky : MapGeneratorBase
{
    public Tilemap bgTM;
    public TileGroupDataBase bgTileGroupData;

    protected int yMin = -16;
    protected int yMax = 16;
    protected int xMin = -32;
    protected int xMax = 32;

    public override void BuildAll(int buildLevel = 1)
    {
        TileGroupBase tg = bgTileGroupData.GetTileGroup();
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                bgTM.SetTile(new Vector3Int(x, y, 0), tg.GetOneTile());
            }
        }
    }
}
