using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;

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

    protected TILE_GROUP_ID planID = TILE_GROUP_ID.GRASS;
    protected TILE_GROUP_ID lowID = TILE_GROUP_ID.DIRT;
    protected TILE_GROUP_ID lowEdgeID = TILE_GROUP_ID.DIRT_EDGE;
    protected TILE_GROUP_ID highID = TILE_GROUP_ID.GRASS;
    protected TILE_GROUP_ID highEdgeID = TILE_GROUP_ID.HIGH_EDGE;
    
    
    protected TileGroupBase planTG;
    protected TileGroupBase highTG;
    protected TileGroupBase lowTG;
    protected TileEdgeGroup lowEdgeTG;
    protected TileEdgeGroup highEdgeTG;

    //以下如果有設定，就無視 the TileGroupLib
    public TileGroupDataBase planTGData;
    public TileGroupDataBase highTGData;
    public TileGroupDataBase lowTGData;
    public TileEdgeGroupDataBase highEdgeTileGroupData; 
    public TileEdgeGroupDataBase lowEdgeTileGroupData;

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
        theCellMap.GetOneMap().FillTileAll(1, groundTM, planTG);
        theCellMap.GetOneMap().FillTileAll(2, groundTM, groundTM, lowTG, lowEdgeTG);
        theCellMap.GetOneMap().FillTileAll(3, blockTM, blockTM, highTG, highEdgeTG);
    }

    protected virtual void PreBuild() { }
    protected virtual void PostBuild() { }

    public override void BuildAll(int buildLevel = 1)
    {
        PreBuild();

        theCellMap.InitCellMap(mapCellWidthH, mapCellHeightH, CellSize);

        if (theTileGroupLib)
        {
            planTG = theTileGroupLib.GetTileGroup(planID);
            lowTG = theTileGroupLib.GetTileGroup(lowID);
            highTG = theTileGroupLib.GetTileGroup(highID);
            lowEdgeTG = theTileGroupLib.GetTileEdgeGroup(lowEdgeID);
            highEdgeTG = theTileGroupLib.GetTileEdgeGroup(highEdgeID);
        }
        if (planTGData)
            planTG = planTGData.GetTileGroup();
        if (lowTGData)
            lowTG = lowTGData.GetTileGroup();
        if (highTGData)
            highTG = highTGData.GetTileGroup();
        if (highEdgeTileGroupData)
            highEdgeTG = highEdgeTileGroupData.GetTileEdgeGroup();
        if (lowEdgeTileGroupData)
            lowEdgeTG = lowEdgeTileGroupData.GetTileEdgeGroup();

        GenerateCellMap();

        //theCellMap.GetOneMap().PrintMap();

        FillTiles();

        PostBuild();

        GenerateNavMesh(theSurface2D);
    }
}
