using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MazeGameManagerBase;

public class MissionCarveGameData : MonoBehaviour
{
    //這些是基本共同資訊
    [Space(10)]
    [Header("地圖資訊")]
    public Vector2Int mapSize = new Vector2Int(120, 160);
    public Vector2Int initRoomSize = new Vector2Int(10, 12);
    public int pathWidth = 6;   //會改寫掉後面的 RoomSequenceInfo


    //這些是基本路徑資訊
    [Space(10)]
    [Header("路徑資訊")]
    public CarveOne.RoomSequenceInfo mainPathInfo;
    public CarveOne.RoomSequenceInfo brainchPathInfo;
    public int branchCount = 2;

    //這些是 Gameplay 資訊
    [Space(10)]
    [Header("Gameplay 資訊")]
    public float difficultyMin = 1.0f;
    public float difficultyMax = 2.0f;
    public RoomGameplayBase[] defaultRoomGameplay;
    public RoomGameplayBase[] corridorGameplay;

    //特別 Game Room
    public enum SPECIAL_ROOM_TYPE
    {
        BOSS_END,
        EXPLORE_BATTLE,    //探索任務端點前大戰
        EXPLORE_REWARD, //探索任務端點獎勵
    }
    [System.Serializable]
    public class SPECIAL_ROOM_DEF
    {
        public SPECIAL_ROOM_TYPE type;
        public RoomGameplayBase roomGameplay;
    }
    [Space(10)]
    [Header("特別 Game Room")]
    public SPECIAL_ROOM_DEF[] specialRooms;

    //掉落物相關
    [Space(10)]
    [Header("掉落資訊")]
    public ObjectPlacementManager theOPM;
    public GameObject helpDollRef;
    public int helpDollNum = 10;

    protected CarveOne myCarve;
    protected OneMap theMap;
    protected int border;

    protected Dictionary<SPECIAL_ROOM_TYPE, RoomGameplayBase> specialRoomGames = new();

    //protected List<RoomInfo> mainGameRooms = new List<RoomInfo>();
    //protected List<RoomInfo> corridorRooms = new List<RoomInfo>();
    //protected class BranchSequence
    //{
    //    public List<RoomInfo> rooms = new();
    //}
    //protected List<BranchSequence> branches = new();

    protected class RoomGamePair
    {
        public RoomInfo room;
        public RoomGameplayBase gameplay;
    }

    protected List<RoomGamePair> mainPairs = new List<RoomGamePair>();
    protected List<List<RoomGamePair>> branchPairLists = new List<List<RoomGamePair>>();
    protected List<RoomGamePair> corridorPairs = new List<RoomGamePair>();

    virtual public void SetupCarveOne( CarveOne carve)
    {
        myCarve = carve;

        //根據任務內容設定 Carve 參數
        mainPathInfo.type = CarveOne.RoomSequenceInfo.TYPE.MAIN_ADD;
        brainchPathInfo.type = CarveOne.RoomSequenceInfo.TYPE.BRANCH_NEW;
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
        roomInfo.cell = new MG_MazeOneBase.CELL();
        roomInfo.cell.U = room.isPath[(int)DIRECTION.U];
        roomInfo.cell.D = room.isPath[(int)DIRECTION.D];
        roomInfo.cell.L = room.isPath[(int)DIRECTION.L];
        roomInfo.cell.R = room.isPath[(int)DIRECTION.R];
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
        room.doorWidth = corridor.w;
        room.doorHeight = corridor.h;
        room.wallWidth = 0;
        room.wallHeight = 0;
        room.cell = new MG_MazeOneBase.CELL();
        room.cell.U = false;
        room.cell.D = false;
        room.cell.L = false;
        room.cell.R = false;
        room.cell.isPath = true;
        room.diffAddRatio = 1.0f;
        room.enemyLV = 1;

        return room;
    }


    protected void InitSpecialGameplay() 
    {
        foreach (SPECIAL_ROOM_DEF special in specialRooms) 
        {
            specialRoomGames[special.type] = special.roomGameplay;
        }

    }

    //從 Carve 產生的房間開始放置 Gameplay 用的 RoomInfo
    virtual protected void SetupRoomGameMapping()
    {
        List<CarveOne.Room> mainRooms = myCarve.GetMainPathRooms();
        int finalDepth = mainRooms.Count - 1;
        for (int i = 1; i < mainRooms.Count; i++)
        {
            RoomInfo roomInfo = CreateGameRoom(mainRooms[i]);
            roomInfo.mainRatio = (float)i / finalDepth;
            roomInfo.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * roomInfo.mainRatio;
            roomInfo.cell.x = roomInfo.cell.y = i;

            //mainGameRooms.Add(roomInfo);
            mainPairs.Add(new RoomGamePair { room = roomInfo });

            //通道的部份
            if (mainRooms[i].corridorFrom != null)
            {
                RoomInfo corridor = CreateCorridorRoom(mainRooms[i].corridorFrom);
                corridor.mainRatio = (float)i / finalDepth;
                corridor.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * corridor.mainRatio;
                corridor.cell.x = corridor.cell.y = i;
                //corridorRooms.Add(corridor);
                corridorPairs.Add(new RoomGamePair { room = corridor });
            }
        }

        //所有支線的部份
        List<CarveOne.Branch> allBranches = myCarve.GetAllBranches();
        foreach (CarveOne.Branch b in allBranches)
        {
            //BranchSequence newBranch = new BranchSequence();
            //branches.Add(newBranch);

            List<RoomGamePair> newPairs = new List<RoomGamePair>();
            branchPairLists.Add(newPairs);

            for (int i = 0; i < b.rooms.Count; i++)
            {
                RoomInfo roomInfo = CreateGameRoom(b.rooms[i]);
                roomInfo.mainRatio = (float)(b.mainDepth + i) / finalDepth;
                roomInfo.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * roomInfo.mainRatio;
                roomInfo.cell.x = b.mainDepth;
                roomInfo.cell.y = i;
                //newBranch.rooms.Add(roomInfo);
                newPairs.Add(new RoomGamePair { room = roomInfo });

                if (b.rooms[i].corridorFrom != null)
                {
                    RoomInfo corridor = CreateCorridorRoom(b.rooms[i].corridorFrom);
                    corridor.mainRatio = (float)(b.mainDepth + i) / finalDepth;
                    corridor.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * corridor.mainRatio;
                    corridor.cell.x = b.mainDepth;
                    corridor.cell.y = i;
                    //corridorRooms.Add(corridor);
                    corridorPairs.Add(new RoomGamePair { room = corridor });
                }
            }
        }
    }


    virtual protected void SetupSpecialGames()
    {
        if (specialRoomGames.ContainsKey(SPECIAL_ROOM_TYPE.BOSS_END))
        {
            mainPairs[mainPairs.Count - 1].gameplay = specialRoomGames[SPECIAL_ROOM_TYPE.BOSS_END];
        }

    }

    virtual protected void SetupDefaultGames() 
    {
        // ============================================================================================
        //  開始設置房間 Gameplay
        // ============================================================================================
        //for (int i = 0; i < mainGameRooms.Count; i++)
        //{
        //    RoomGameplayBase game;
        //    if (i == mainGameRooms.Count - 1 && specialRoomGames.ContainsKey(SPECIAL_ROOM_TYPE.BOSS_END))
        //    {
        //        game = specialRoomGames[SPECIAL_ROOM_TYPE.BOSS_END];
        //    }
        //    else
        //    {
        //        game = defaultRoomGameplay[Random.Range(0, defaultRoomGameplay.Length)];
        //    }
        //    game.Build(mainGameRooms[i]);
        //}
        for (int i=0; i<mainPairs.Count; i++)
        {
            if (mainPairs[i].gameplay == null)
            {
                mainPairs[i].gameplay = defaultRoomGameplay[Random.Range(0, defaultRoomGameplay.Length)];
            }
        }

        //foreach (BranchSequence bs in branches)
        //{
        //    for (int i = 0; i < bs.rooms.Count; i++)
        //    {
        //        RoomGameplayBase game = defaultRoomGameplay[Random.Range(0, defaultRoomGameplay.Length)];
        //        game.Build(bs.rooms[i]);
        //    }
        //}
        foreach (List<RoomGamePair> bList in branchPairLists)
        {
            for (int i=0; i<bList.Count; i++)
            {
                if (bList[i].gameplay == null)
                {
                    bList[i].gameplay = defaultRoomGameplay[Random.Range(0, defaultRoomGameplay.Length)];
                }
            }
        }

        //開始放置通道上的 Gameplay
        //for (int i = 0; i < corridorRooms.Count; i++)
        //{
        //    if (corridorGameplay.Length > 0)
        //    {
        //        RoomGameplayBase game = corridorGameplay[Random.Range(0, corridorGameplay.Length)];
        //        game.Build(corridorRooms[i]);
        //    }

        //    //if (theOPM)
        //    //    theOPM.AddRoom(corridorRooms[i]);
        //}
        for (int i=0; i< corridorPairs.Count; i++)
        {
            if (corridorPairs[i].gameplay = null)
            {
                corridorPairs[i].gameplay = corridorGameplay[Random.Range(0, corridorGameplay.Length)];
            }
            if (theOPM)
                theOPM.AddRoom(corridorPairs[i].room);
        }
    }


    public void SetupGameplay(OneMap _theMap, int _border)
    {
        InitSpecialGameplay();

        theMap = _theMap;
        border = _border;

        SetupRoomGameMapping();

        //任務用特殊房先準備
        SetupSpecialGames();

        SetupDefaultGames();

        //把所有 Game Build 出來
        List<List<RoomGamePair>> allList = new();
        allList.Add(mainPairs);
        foreach (List<RoomGamePair> list in branchPairLists)
        {
            allList.Add(list);
        }
        allList.Add(corridorPairs);

        foreach (List<RoomGamePair> list in allList)
        {
            foreach (RoomGamePair pair in list)
            {
                if (pair.gameplay != null)
                {
                    pair.gameplay.Build(pair.room);
                }
                //else
                //{
                //    print("沒有 Game 的房間 " + pair.room.mainRatio);
                //}
            }
        }

        //所有放置物品
        if (theOPM)
        {
            //先處理野巫靈
            if (helpDollRef)
            {
                theOPM.randomObjects = new ObjectPlacementManager.DollObjectInfo[1];
                theOPM.randomObjects[0] = new ObjectPlacementManager.DollObjectInfo();
                theOPM.randomObjects[0].objRef = helpDollRef;
                theOPM.forceRandomNum = helpDollNum;
            }
            theOPM.BuildAll();
        }
    }

}
