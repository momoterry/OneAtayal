using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MG_MazeOneBase;
using UnityEngine.Tilemaps;



public class MissionCarveLevelGenerator : MapGeneratorBase
{
    //Carve ��Ƭ���
    [Space(10)]
    [Header("Carve �]�w")]
    public CarveOne myCarve;

    [Space(10)]
    [Header("Mission �]�w")]
    [Tooltip("Mission �]�w")]
    //public MissionGameplayData missionGameData;
    public MissionCarveGameData missionGameData;

    //Tile ��Ƭ���
    [Space(10)]
    [Header("Tile �]�w")]
    [Tooltip("Tile �]�w")]
    public TileGroupDataBase groundTileGroup;
    public TileEdgeGroupDataBase groundEdgeTileGroup;
    public TileEdgeGroupDataBase groundOutEdgeTileGroup;
    public TileGroupDataBase blockTileGroup;                //�Φb�D��ɪ� Block �ϰ�
    public TileEdgeGroupDataBase blockTileEdgeGroup;
    public TileGroupDataBase holeTileGroup;
    public TileEdgeGroupDataBase holeTileEdgeGroup;
    public TileGroupDataBase defautTileGroup;               //�Φb�a�Ϫ��~���

    public Tilemap groundTM;
    public Tilemap blockTM;

    //OneMap
    protected Vector3Int mapCenter;
    protected OneMap theMap = new OneMap();

    //Carve Map
    protected int[,] map;


    //protected void InitCarveInfoByMission()
    //{
    //    //�۰ʭץ�
    //    //�ھڥ��Ȥ��e�]�w Carve �Ѽ�
    //    myCarve.width = missionGameData.mapSize.x;
    //    myCarve.height = missionGameData.mapSize.y;
    //    myCarve.paths = new CarveOne.RoomSequenceInfo[missionGameData.branchCount + 1];
    //    myCarve.paths[0] = missionGameData.mainPathInfo;
    //    for (int i = 1; i < myCarve.paths.Length; i++) 
    //    {
    //        myCarve.paths[i] = missionGameData.brainchPathInfo;

    //    }
    //    for (int i=0; i< myCarve.paths.Length; i++)
    //    {
    //        myCarve.paths[i].corridorWidth = missionGameData.pathWidth;
    //    }
    //    myCarve.initRoomWidth = missionGameData.initRoomSize.x;
    //    myCarve.initRoomHeight = missionGameData.initRoomSize.y;
    //}

    //protected void SetupGameplayByMission()
    //{
    //    List<RoomInfo> mainGameRooms = new List<RoomInfo>();
    //    List<RoomInfo> corridorRooms = new List<RoomInfo>();
    //    //�q Carve ���ͪ��ж��}�l��m Gameplay
    //    List<CarveOne.Room> mainRooms = myCarve.GetMainPathRooms();
    //    int finalDepth = mainRooms.Count;
    //    for (int i = 1; i < mainRooms.Count; i++)
    //    {
    //        RoomInfo roomInfo = new RoomInfo();
    //        RectInt rect = new RectInt(mainRooms[i].x + theMap.xMin + border, mainRooms[i].y + theMap.yMin + border, mainRooms[i].w, mainRooms[i].h);
    //        roomInfo.mapRect = rect;
    //        roomInfo.vCenter = new Vector3(rect.x + (rect.width)*0.5f, 0, rect.y + rect.height * 0.5f );
    //        roomInfo.width = mainRooms[i].w;
    //        roomInfo.height = mainRooms[i].h;
    //        roomInfo.doorWidth = roomInfo.doorHeight = missionGameData.pathWidth;
    //        roomInfo.wallWidth = 0;
    //        roomInfo.wallHeight = 0;
    //        roomInfo.mainRatio = i/finalDepth;
    //        roomInfo.cell = new CELL();
    //        roomInfo.cell.U = mainRooms[i].isPath[(int)DIRECTION.U];
    //        roomInfo.cell.D = mainRooms[i].isPath[(int)DIRECTION.D];
    //        roomInfo.cell.L = mainRooms[i].isPath[(int)DIRECTION.L];
    //        roomInfo.cell.R = mainRooms[i].isPath[(int)DIRECTION.R];
    //        roomInfo.cell.x = roomInfo.cell.y = i;
    //        roomInfo.diffAddRatio = 1.0f;
    //        roomInfo.enemyLV = 1;

    //        mainGameRooms.Add(roomInfo);

    //        //�q�D������
    //        if (mainRooms[i].corridorFrom != null)
    //        {
    //            RoomInfo corridor = new RoomInfo();
    //            RectInt rc = new RectInt(mainRooms[i].corridorFrom.x + theMap.xMin + border, 
    //                mainRooms[i].corridorFrom.y + theMap.yMin + border, 
    //                mainRooms[i].corridorFrom.w, 
    //                mainRooms[i].corridorFrom.h);
    //            corridor.mapRect = rc;
    //            corridor.vCenter = new Vector3(rc.x + (rc.width) * 0.5f, 0, rc.y + rc.height * 0.5f);
    //            corridor.width = mainRooms[i].corridorFrom.w;
    //            corridor.height = mainRooms[i].corridorFrom.h;
    //            corridor.doorWidth = corridor.doorHeight = missionGameData.pathWidth;
    //            corridor.wallWidth = 0;
    //            corridor.wallHeight = 0;
    //            corridor.mainRatio = i / finalDepth;
    //            corridor.cell = new CELL();
    //            corridor.cell.U = false;
    //            corridor.cell.D = false;
    //            corridor.cell.L = false;
    //            corridor.cell.R = false;
    //            corridor.cell.x = corridor.cell.y = i;
    //            corridor.cell.isPath = true;
    //            corridor.diffAddRatio = 1.0f;
    //            corridor.enemyLV = 1;
    //            corridorRooms.Add(corridor);
    //        }
    //    }

    //    //�}�l�]�m�ж� Gameplay
    //    for (int i = 0; i< mainGameRooms.Count; i++)
    //    {
    //        RoomGameplayBase game = missionGameData.defaultRoomGameplay[Random.Range(0, missionGameData.defaultRoomGameplay.Length)];
    //        game.Build(mainGameRooms[i]);
    //    }

    //    //�}�l��m�q�D�W�� Gameplay
    //    for (int i = 0;i< corridorRooms.Count; i++)
    //    {
    //        RoomGameplayBase game = missionGameData.corridorGameplay[Random.Range(0, missionGameData.corridorGameplay.Length)];
    //        game.Build(corridorRooms[i]);
    //    }
    //}

    int width;
    int height;
    int border;
    public override void BuildAll(int buildLevel = 1)
    {
        if (!myCarve)
        {
            return;
        }

        //InitCarveInfoByMission();
        missionGameData.SetupCarveOne(myCarve);

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

        //SetupGameplayByMission();
        missionGameData.SetupGameplay(theMap, border);

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
