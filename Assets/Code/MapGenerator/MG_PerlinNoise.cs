using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_PerlinNoise : MapGeneratorBase
{
    public int CellSize = 4;
    public int mapCellWidthH = 15;
    public int mapCellHeightH = 10;
    //protected int mapCellWidth;
    //protected int mapCellHeight;
    public TileGroupLibrary theTileGroupLib;

    public Tilemap groundTM;

    public TILE_GROUP_ID planID = TILE_GROUP_ID.GRASS;
    public TILE_GROUP_ID lowID = TILE_GROUP_ID.DIRT;
    public TILE_GROUP_ID lowEdgeID = TILE_GROUP_ID.DIRT_EDGE;
    public TILE_GROUP_ID highID = TILE_GROUP_ID.GRASS;
    public TILE_GROUP_ID highEdgeID = TILE_GROUP_ID.HIGH_EDGE;

    protected TileGroup planTG;
    protected TileGroup highTG;
    protected TileGroup lowTG;

    protected OneCellMap theCellMap = new OneCellMap();
    //protected OneMap theMap = new OneMap();

    public override void BuildAll(int buildLevel = 1)
    {
        //theMap.InitMap(cellMapWidthH, cellMapHeightH)
        theCellMap.InitCellMap(mapCellWidthH, mapCellHeightH, CellSize);

        planTG = theTileGroupLib.GetTileGroup(planID);
        lowTG = theTileGroupLib.GetTileGroup(lowID);
        highTG = theTileGroupLib.GetTileGroup(highID);

        //¥ý´ú¸Õ CellMap
        for (int x = theCellMap.GetXMin(); x <= theCellMap.GetXMax(); x++)
        {
            for (int y = theCellMap.GetYMin(); y <= theCellMap.GetYMax(); y++)
            {
                //groundTM.SetTile(new Vector3Int(x, y, 0), planTG.GetOneTile());
                theCellMap.SetValue(x, y, 1);
            }
        }

        for (int x = -3; x <= 3; x++)
        {
            for (int y = -3; y <= 3; y++)
            {
                theCellMap.SetValue(x, y, 2);
            }
        }
        theCellMap.SetValue(0, 0, 3);

        theCellMap.GetOneMap().PrintMap();

        theCellMap.GetOneMap().FillTileAll(2, groundTM, lowTG.baseTile);
        theCellMap.GetOneMap().FillTileAll(1, groundTM, planTG.baseTile);
        theCellMap.GetOneMap().FillTileAll(3, groundTM, highTG.baseTile);

        theSurface2D.BuildNavMesh();
    }
}
