using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MG_MazeOneBase;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MissionRoomInfo_Boss
{
    public int width = 160;
    public int height = 200;
    public CarveOne.PathInfo mainPathInfo;
    public CarveOne.PathInfo brainchPathInfo;

    public RoomGameplayBase[] defaultRoomGameplay;
}


public class MissionCarveLevelGenerator : MapGeneratorBase
{
    //Carve 資料相關
    [Space(10)]
    [Header("Carve 設定")]
    public CarveOne myCarve;

    //測試的任務資料
    public MissionRoomInfo_Boss bossMission;

    //Tile 資料相關
    [Space(10)]
    [Header("Tile 設定")]
    [Tooltip("Tile 設定")]
    public TileGroupDataBase groundTileGroup;
    public TileEdgeGroupDataBase groundEdgeTileGroup;
    public TileEdgeGroupDataBase groundOutEdgeTileGroup;
    public TileGroupDataBase blockTileGroup;                //用在非邊界的 Block 區域
    public TileEdgeGroupDataBase blockTileEdgeGroup;
    public TileGroupDataBase holeTileGroup;
    public TileEdgeGroupDataBase holeTileEdgeGroup;
    public TileGroupDataBase defautTileGroup;               //用在地圖的外邊界

    public Tilemap groundTM;
    public Tilemap blockTM;

    //OneMap
    protected Vector3Int mapCenter;
    protected OneMap theMap = new OneMap();

    //Carve Map
    protected int[,] map;


    protected void InitCarveInfoByMission()
    {
        //根據任務內容設定 Carve 參數
        //TODO: 目前只是暫代
        myCarve.width = bossMission.width;
        myCarve.height = bossMission.height;
        myCarve.paths = new CarveOne.PathInfo[3];
        myCarve.paths[0] = bossMission.mainPathInfo;
        myCarve.paths[1] = bossMission.brainchPathInfo;
        myCarve.paths[2] = bossMission.brainchPathInfo;
    }

    protected void SetupGameplayByMission()
    {
        //從 Carve 產生的房間開始放置 Gameplay
        List<CarveOne.Room> mainRooms = myCarve.GetMainPathRooms();
    }

    public override void BuildAll(int buildLevel = 1)
    {
        if (!myCarve)
        {
            return;
        }

        InitCarveInfoByMission();

        map = myCarve.CreateCarveMap();

        SetupGameplayByMission();

        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int border = 4;
        theMap = new OneMap();
        //Vector2Int carveCenter = new Vector2Int(0, height/2 - 2);
        theMap.InitMap(new Vector2Int(0, height / 2 - 4), width + border * 2, height + border * 2, (int)MAP_TYPE.BLOCK);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != 0)
                {
                    theMap.SetValue(theMap.xMin + x + border, theMap.yMin + y + border, (int)MAP_TYPE.GROUND);
                }
            }
        }

        FillAllTiles();

        if (theSurface2D)
            GenerateNavMesh(theSurface2D);
    }

    virtual protected void InitMap()
    {

    }

    protected void FillAllTiles()
    {
        if (defautTileGroup)
            theMap.FillTileAll(OneMap.DEFAULT_VALUE, blockTM, defautTileGroup.GetTileGroup());

        if (groundEdgeTileGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTM, groundTileGroup.GetTileGroup(), groundEdgeTileGroup.GetTileEdgeGroup(), false, (int)MAP_TYPE.BLOCK);
        else
            theMap.FillTileAll((int)MAP_TYPE.GROUND, groundTM, groundTileGroup.GetTileGroup());

        if (blockTileGroup)
        {
            if (blockTileEdgeGroup)
                theMap.FillTileAll((int)MAP_TYPE.BLOCK, blockTM, blockTM, blockTileGroup.GetTileGroup(), blockTileEdgeGroup.GetTileEdgeGroup(), false);
            else
                theMap.FillTileAll((int)MAP_TYPE.BLOCK, blockTM, blockTileGroup.GetTileGroup());
        }

        if (groundOutEdgeTileGroup && !blockTileEdgeGroup)
            theMap.FillTileAll((int)MAP_TYPE.GROUND, null, blockTM, null, groundOutEdgeTileGroup.GetTileEdgeGroup(), true, (int)MAP_TYPE.BLOCK);

        if (holeTileGroup)
        {
            if (holeTileEdgeGroup)
                theMap.FillTileAll((int)MAP_TYPE.HOLE, blockTM, blockTM, holeTileGroup.GetTileGroup(), holeTileEdgeGroup.GetTileEdgeGroup(), false);
            else
                theMap.FillTileAll((int)MAP_TYPE.HOLE, blockTM, holeTileGroup.GetTileGroup());
        }
    }
}
