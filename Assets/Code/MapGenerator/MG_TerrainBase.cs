using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_TerrainBase : MapGeneratorBase
{
    public int CellSize = 4;
    public int mapCellWidthH = 15;
    public int mapCellHeightH = 10;
    //protected int mapCellWidth;
    //protected int mapCellHeight;
    public TileGroupLibrary theTileGroupLib;

    public Tilemap groundTM;
    public Tilemap blockTM;

    public TILE_GROUP_ID planID = TILE_GROUP_ID.GRASS;
    public TILE_GROUP_ID lowID = TILE_GROUP_ID.DIRT;
    public TILE_GROUP_ID lowEdgeID = TILE_GROUP_ID.DIRT_EDGE;
    public TILE_GROUP_ID highID = TILE_GROUP_ID.GRASS;
    public TILE_GROUP_ID highEdgeID = TILE_GROUP_ID.HIGH_EDGE;

    protected TileGroup planTG;
    protected TileGroup highTG;
    protected TileGroup lowTG;
    protected TileEdgeGroup lowEdgeTG;
    protected TileEdgeGroup highEdgeTG;

    protected OneCellMap theCellMap = new OneCellMap();
    //protected OneMap theMap = new OneMap();

    protected virtual void GenerateCellMap()
    {
        theCellMap.GetOneMap().FillValueAll(1);

        for (int x = -3; x <= 3; x++)
        {
            for (int y = 2; y <= 3; y++)
            {
                theCellMap.SetValue(x, y, 2);
            }
        }
        theCellMap.SetValue(0, 0, 3);
    }

    protected virtual void FillTiles()
    {
        theCellMap.GetOneMap().FillTileAll(1, groundTM, planTG.baseTile);
        theCellMap.GetOneMap().FillTileAll(2, groundTM, groundTM, lowTG, lowEdgeTG);
        theCellMap.GetOneMap().FillTileAll(3, groundTM, groundTM, highTG, highEdgeTG);
    }

    public override void BuildAll(int buildLevel = 1)
    {
        theCellMap.InitCellMap(mapCellWidthH, mapCellHeightH, CellSize);

        planTG = theTileGroupLib.GetTileGroup(planID);
        lowTG = theTileGroupLib.GetTileGroup(lowID);
        highTG = theTileGroupLib.GetTileGroup(highID);
        lowEdgeTG = theTileGroupLib.GetTileEdgeGroup(lowEdgeID);
        highEdgeTG = theTileGroupLib.GetTileEdgeGroup(highEdgeID);


        GenerateCellMap();

        //theCellMap.GetOneMap().PrintMap();

        FillTiles();

        theSurface2D.BuildNavMesh();
    }
}
