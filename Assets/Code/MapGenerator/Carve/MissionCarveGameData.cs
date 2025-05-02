using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MazeGameManagerBase;

public class MissionCarveGameData : MonoBehaviour
{
    //這些是基本共同資訊
    public Vector2Int mapSize = new Vector2Int(120, 160);
    public Vector2Int initRoomSize = new Vector2Int(10, 12);
    public int pathWidth = 6;   //會改寫掉後面的 RoomSequenceInfo


    //這些是基本路徑資訊
    public CarveOne.RoomSequenceInfo mainPathInfo;
    public CarveOne.RoomSequenceInfo brainchPathInfo;
    public int branchCount = 2;

    public RoomGameplayBase[] defaultRoomGameplay;
    public RoomGameplayBase[] corridorGameplay;

    protected CarveOne myCarve;
    protected OneMap theMap;
    protected int border;

    protected class BranchSequence
    {
        public List<RoomInfo> rooms = new ();
    }
    protected List<BranchSequence> branches = new();

    public void SetupCarveOne( CarveOne carve)
    {
        myCarve = carve;

        //根據任務內容設定 Carve 參數
        myCarve.width = mapSize.x;
        myCarve.height = mapSize.y;
        myCarve.paths = new CarveOne.RoomSequenceInfo[branchCount + 1];
        myCarve.paths[0] = mainPathInfo;
        for (int i = 1; i < myCarve.paths.Length; i++)
        {
            myCarve.paths[i] = brainchPathInfo;

        }
        for (int i = 0; i < myCarve.paths.Length; i++)
        {
            myCarve.paths[i].corridorWidth = pathWidth;
        }
        myCarve.initRoomWidth = initRoomSize.x;
        myCarve.initRoomHeight = initRoomSize.y;
    }

    protected RoomInfo CreateGameRoom(CarveOne.Room room)
    {
        RoomInfo roomInfo = new RoomInfo();
        RectInt rect = new RectInt(room.x + theMap.xMin + border, room.y + theMap.yMin + border, room.w, room.h);
        roomInfo.mapRect = rect;
        roomInfo.vCenter = new Vector3(rect.x + (rect.width) * 0.5f, 0, rect.y + rect.height * 0.5f);
        roomInfo.width = room.w;
        roomInfo.height = room.h;
        roomInfo.doorWidth = roomInfo.doorHeight = pathWidth;
        roomInfo.wallWidth = 0;
        roomInfo.wallHeight = 0;
        //roomInfo.mainRatio = i / finalDepth;
        roomInfo.cell = new MG_MazeOneBase.CELL();
        roomInfo.cell.U = room.isPath[(int)DIRECTION.U];
        roomInfo.cell.D = room.isPath[(int)DIRECTION.D];
        roomInfo.cell.L = room.isPath[(int)DIRECTION.L];
        roomInfo.cell.R = room.isPath[(int)DIRECTION.R];
        //roomInfo.cell.x = roomInfo.cell.y = i;
        roomInfo.diffAddRatio = 1.0f;
        roomInfo.enemyLV = 1;

        return roomInfo;
    }

    protected RoomInfo CreateCorridorRoom( CarveOne.Corridor corridor )
    {
        RoomInfo room = new RoomInfo();
        RectInt rc = new RectInt(corridor.x + theMap.xMin + border,
            corridor.y + theMap.yMin + border,
            corridor.w,
            corridor.h);
        room.mapRect = rc;
        room.vCenter = new Vector3(rc.x + (rc.width) * 0.5f, 0, rc.y + rc.height * 0.5f);
        room.width = corridor.w;
        room.height = corridor.h;
        room.doorWidth = room.doorHeight = pathWidth;
        room.wallWidth = 0;
        room.wallHeight = 0;
        //room.mainRatio = i / finalDepth;
        room.cell = new MG_MazeOneBase.CELL();
        room.cell.U = false;
        room.cell.D = false;
        room.cell.L = false;
        room.cell.R = false;
        //room.cell.x = corridor.cell.y = i;
        room.cell.isPath = true;
        room.diffAddRatio = 1.0f;
        room.enemyLV = 1;

        return room;
    }

    public void SetupGameplay(OneMap _theMap, int _border)
    {
        theMap = _theMap;
        border = _border;
        List<RoomInfo> mainGameRooms = new List<RoomInfo>();
        List<RoomInfo> corridorRooms = new List<RoomInfo>();
        //從 Carve 產生的房間開始放置 Gameplay
        List<CarveOne.Room> mainRooms = myCarve.GetMainPathRooms();
        int finalDepth = mainRooms.Count;
        for (int i = 1; i < mainRooms.Count; i++)
        {
            //RoomInfo roomInfo = new RoomInfo();
            //RectInt rect = new RectInt(mainRooms[i].x + theMap.xMin + border, mainRooms[i].y + theMap.yMin + border, mainRooms[i].w, mainRooms[i].h);
            //roomInfo.mapRect = rect;
            //roomInfo.vCenter = new Vector3(rect.x + (rect.width) * 0.5f, 0, rect.y + rect.height * 0.5f);
            //roomInfo.width = mainRooms[i].w;
            //roomInfo.height = mainRooms[i].h;
            //roomInfo.doorWidth = roomInfo.doorHeight = pathWidth;
            //roomInfo.wallWidth = 0;
            //roomInfo.wallHeight = 0;
            //roomInfo.mainRatio = i / finalDepth;
            //roomInfo.cell = new MG_MazeOneBase.CELL();
            //roomInfo.cell.U = mainRooms[i].isPath[(int)DIRECTION.U];
            //roomInfo.cell.D = mainRooms[i].isPath[(int)DIRECTION.D];
            //roomInfo.cell.L = mainRooms[i].isPath[(int)DIRECTION.L];
            //roomInfo.cell.R = mainRooms[i].isPath[(int)DIRECTION.R];
            //roomInfo.cell.x = roomInfo.cell.y = i;
            //roomInfo.diffAddRatio = 1.0f;
            //roomInfo.enemyLV = 1;

            RoomInfo roomInfo = CreateGameRoom(mainRooms[i]);
            roomInfo.mainRatio = i / finalDepth;
            roomInfo.cell.x = roomInfo.cell.y = i;

            mainGameRooms.Add(roomInfo);

            //通道的部份
            if (mainRooms[i].corridorFrom != null)
            {
                //RoomInfo corridor = new RoomInfo();
                //RectInt rc = new RectInt(mainRooms[i].corridorFrom.x + theMap.xMin + border,
                //    mainRooms[i].corridorFrom.y + theMap.yMin + border,
                //    mainRooms[i].corridorFrom.w,
                //    mainRooms[i].corridorFrom.h);
                //corridor.mapRect = rc;
                //corridor.vCenter = new Vector3(rc.x + (rc.width) * 0.5f, 0, rc.y + rc.height * 0.5f);
                //corridor.width = mainRooms[i].corridorFrom.w;
                //corridor.height = mainRooms[i].corridorFrom.h;
                //corridor.doorWidth = corridor.doorHeight = pathWidth;
                //corridor.wallWidth = 0;
                //corridor.wallHeight = 0;
                //corridor.mainRatio = i / finalDepth;
                //corridor.cell = new MG_MazeOneBase.CELL();
                //corridor.cell.U = false;
                //corridor.cell.D = false;
                //corridor.cell.L = false;
                //corridor.cell.R = false;
                //corridor.cell.x = corridor.cell.y = i;
                //corridor.cell.isPath = true;
                //corridor.diffAddRatio = 1.0f;
                //corridor.enemyLV = 1;
                RoomInfo corridor = CreateCorridorRoom(mainRooms[i].corridorFrom);
                corridor.mainRatio = i / finalDepth;
                corridor.cell.x = corridor.cell.y = i;
                corridorRooms.Add(corridor);
            }
        }

        //所有支線的部份
        List<CarveOne.Branch> allBranches = myCarve.GetAllBranches();
        foreach (CarveOne.Branch b in allBranches)
        {
            BranchSequence newBranch = new BranchSequence();
            branches.Add(newBranch);

            for (int i = 0; i < b.rooms.Count; i++)
            {
                RoomInfo roomInfo = CreateGameRoom(b.rooms[i]);
                roomInfo.mainRatio = (b.mainDepth + i) / finalDepth;
                roomInfo.cell.x = b.mainDepth;
                roomInfo.cell.y = i;
                newBranch.rooms.Add(roomInfo);

                if (b.rooms[i].corridorFrom != null)
                {
                    RoomInfo corridor = CreateCorridorRoom(b.rooms[i].corridorFrom);
                    corridor.mainRatio = (b.mainDepth + i) / finalDepth;
                    corridor.cell.x = b.mainDepth;
                    corridor.cell.y = i;
                    corridorRooms.Add(corridor);
                }
            }
        }

        //開始設置房間 Gameplay
        for (int i = 0; i < mainGameRooms.Count; i++)
        {
            RoomGameplayBase game = defaultRoomGameplay[Random.Range(0, defaultRoomGameplay.Length)];
            game.Build(mainGameRooms[i]);
        }

        foreach (BranchSequence bs in branches)
        {
            for (int i=0; i<bs.rooms.Count; i++)
            {
                RoomGameplayBase game = defaultRoomGameplay[Random.Range(0, defaultRoomGameplay.Length)];
                game.Build(bs.rooms[i]);
            }
        }

        //開始放置通道上的 Gameplay
        for (int i = 0; i < corridorRooms.Count; i++)
        {
            RoomGameplayBase game = corridorGameplay[Random.Range(0, corridorGameplay.Length)];
            game.Build(corridorRooms[i]);
        }
    }

}
