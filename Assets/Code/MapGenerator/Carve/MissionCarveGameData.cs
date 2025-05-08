using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MazeGameManagerBase;

public class MissionCarveGameData : MonoBehaviour
{
    //�o�ǬO�򥻦@�P��T
    [Space(10)]
    [Header("�a�ϸ�T")]
    public Vector2Int mapSize = new Vector2Int(120, 160);
    public Vector2Int initRoomSize = new Vector2Int(10, 12);
    public int pathWidth = 6;   //�|��g���᭱�� RoomSequenceInfo


    //�o�ǬO�򥻸��|��T
    [Space(10)]
    [Header("���|��T")]
    public CarveOne.RoomSequenceInfo mainPathInfo;
    public CarveOne.RoomSequenceInfo brainchPathInfo;
    public int branchCount = 2;

    //�o�ǬO Gameplay ��T
    [Space(10)]
    [Header("Gameplay ��T")]
    public float difficultyMin = 1.0f;
    public float difficultyMax = 2.0f;
    public RoomGameplayBase[] defaultRoomGameplay;
    public RoomGameplayBase[] corridorGameplay;

    //�S�O Game Room
    public enum SPECIAL_ROOM_TYPE
    {
        BOSS_END,
        EXPLORE_BATTLE,    //�������Ⱥ��I�e�j��
        EXPLORE_REWARD, //�������Ⱥ��I���y
    }
    [System.Serializable]
    public class SPECIAL_ROOM_DEF
    {
        public SPECIAL_ROOM_TYPE type;
        public RoomGameplayBase roomGameplay;
    }
    [Space(10)]
    [Header("�S�O Game Room")]
    public SPECIAL_ROOM_DEF[] specialRooms;

    //����������
    [Space(10)]
    [Header("������T")]
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

        //�ھڥ��Ȥ��e�]�w Carve �Ѽ�
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

    //�q Carve ���ͪ��ж��}�l��m Gameplay �Ϊ� RoomInfo
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

            //�q�D������
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

        //�Ҧ���u������
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
        //  �}�l�]�m�ж� Gameplay
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

        //�}�l��m�q�D�W�� Gameplay
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

        //���ȥίS��Х��ǳ�
        SetupSpecialGames();

        SetupDefaultGames();

        //��Ҧ� Game Build �X��
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
                //    print("�S�� Game ���ж� " + pair.room.mainRatio);
                //}
            }
        }

        //�Ҧ���m���~
        if (theOPM)
        {
            //���B�z�����F
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
