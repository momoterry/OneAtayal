using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MG_MazeOneBase;
using UnityEngine.Tilemaps;
using static MazeGameManagerBase;

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
        List<RoomInfo> mainGameRooms = new List<RoomInfo>();
        //從 Carve 產生的房間開始放置 Gameplay
        List<CarveOne.Room> mainRooms = myCarve.GetMainPathRooms();
        int finalDepth = mainRooms.Count;
        for (int i = 1; i < mainRooms.Count; i++)
        {
            RoomInfo roomInfo = new RoomInfo();
            RectInt rect = new RectInt(mainRooms[i].x + theMap.xMin + border, mainRooms[i].y + theMap.yMin + border, mainRooms[i].w, mainRooms[i].h);
            roomInfo.mapRect = rect;
            roomInfo.vCenter = new Vector3(rect.x + (rect.width)*0.5f, 0, rect.y + rect.height * 0.5f );
            roomInfo.width = mainRooms[i].w;
            roomInfo.height = mainRooms[i].h;
            roomInfo.doorWidth = 6;
            roomInfo.doorHeight = 6;
            roomInfo.wallWidth = 2;
            roomInfo.wallHeight = 2;
            roomInfo.mainRatio = i/finalDepth;
            roomInfo.cell = new CELL();
            roomInfo.cell.U = mainRooms[i].isPath[(int)DIRECTION.U];
            roomInfo.cell.D = mainRooms[i].isPath[(int)DIRECTION.D];
            roomInfo.cell.L = mainRooms[i].isPath[(int)DIRECTION.L];
            roomInfo.cell.R = mainRooms[i].isPath[(int)DIRECTION.R];
            roomInfo.cell.x = roomInfo.cell.y = i;
            roomInfo.diffAddRatio = 1.0f;
            roomInfo.enemyLV = 1;

            mainGameRooms.Add(roomInfo);
        }

        //開始設置 Game
        for (int i = 0; i< mainGameRooms.Count; i++)
        {
            RoomGameplayBase game = bossMission.defaultRoomGameplay[Random.Range(0, bossMission.defaultRoomGameplay.Length)];
            game.Build(mainGameRooms[i]);
        }
    }

    int width;
    int height;
    int border;
    public override void BuildAll(int buildLevel = 1)
    {
        if (!myCarve)
        {
            return;
        }

        InitCarveInfoByMission();

        map = myCarve.CreateCarveMap();

        width = map.GetLength(0);
        height = map.GetLength(1);
        border = 4;
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

        SetupGameplayByMission();

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
