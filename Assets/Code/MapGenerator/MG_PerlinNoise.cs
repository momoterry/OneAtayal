using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_PerlinNoise : MapGeneratorBase
{
    public int CellSize = 4;
    public int cellMapWidthH = 15;
    public int cellMapHeightH = 10;
    public TileGroupLibrary theTileGroupLib;

    public Tilemap groundTM;

    public TILE_GROUP_ID planID = TILE_GROUP_ID.GRASS;
    public TILE_GROUP_ID lowAreaID = TILE_GROUP_ID.DIRT;
    public TILE_GROUP_ID lowAreaEdgeID = TILE_GROUP_ID.DIRT_EDGE;
    public TILE_GROUP_ID highAreaID = TILE_GROUP_ID.GRASS;
    public TILE_GROUP_ID highAreaEdgeID = TILE_GROUP_ID.HIGH_EDGE;

    protected TileGroup planTG;

    public override void BuildAll(int buildLevel = 1)
    {
        planTG = theTileGroupLib.GetTileGroup(planID);

        for (int x = -10; x < 10; x++)
        {
            for (int y = -10; y < 10; y++)
            {
                groundTM.SetTile(new Vector3Int(x, y, 0), planTG.GetOneTile());
            }
        }
    }
}
