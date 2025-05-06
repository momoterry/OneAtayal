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

    //����������
    public ObjectPlacementManager theOPM;
    public GameObject helpDollRef;
    public int helpDollNum = 10;

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

        //�ھڥ��Ȥ��e�]�w Carve �Ѽ�
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

    public void SetupGameplay(OneMap _theMap, int _border)
    {
        theMap = _theMap;
        border = _border;
        List<RoomInfo> mainGameRooms = new List<RoomInfo>();
        List<RoomInfo> corridorRooms = new List<RoomInfo>();
        //�q Carve ���ͪ��ж��}�l��m Gameplay
        List<CarveOne.Room> mainRooms = myCarve.GetMainPathRooms();
        int finalDepth = mainRooms.Count - 1;
        for (int i = 1; i < mainRooms.Count; i++)
        {
            RoomInfo roomInfo = CreateGameRoom(mainRooms[i]);
            roomInfo.mainRatio = (float)i / finalDepth;
            roomInfo.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * roomInfo.mainRatio;
            roomInfo.cell.x = roomInfo.cell.y = i;

            mainGameRooms.Add(roomInfo);

            //�q�D������
            if (mainRooms[i].corridorFrom != null)
            {
                RoomInfo corridor = CreateCorridorRoom(mainRooms[i].corridorFrom);
                corridor.mainRatio = (float)i / finalDepth;
                corridor.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * corridor.mainRatio;
                corridor.cell.x = corridor.cell.y = i;
                corridorRooms.Add(corridor);
            }
        }

        //�Ҧ���u������
        List<CarveOne.Branch> allBranches = myCarve.GetAllBranches();
        foreach (CarveOne.Branch b in allBranches)
        {
            BranchSequence newBranch = new BranchSequence();
            branches.Add(newBranch);

            for (int i = 0; i < b.rooms.Count; i++)
            {
                RoomInfo roomInfo = CreateGameRoom(b.rooms[i]);
                roomInfo.mainRatio = (float)(b.mainDepth + i) / finalDepth;
                roomInfo.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin ) * roomInfo.mainRatio;
                roomInfo.cell.x = b.mainDepth;
                roomInfo.cell.y = i;
                newBranch.rooms.Add(roomInfo);

                if (b.rooms[i].corridorFrom != null)
                {
                    RoomInfo corridor = CreateCorridorRoom(b.rooms[i].corridorFrom);
                    corridor.mainRatio = (float)(b.mainDepth + i) / finalDepth;
                    corridor.diffAddRatio = -1.0f + difficultyMin + (difficultyMax - difficultyMin) * corridor.mainRatio;
                    corridor.cell.x = b.mainDepth;
                    corridor.cell.y = i;
                    corridorRooms.Add(corridor);
                }
            }
        }

        //�}�l�]�m�ж� Gameplay
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

        //�}�l��m�q�D�W�� Gameplay
        for (int i = 0; i < corridorRooms.Count; i++)
        {
            if (corridorGameplay.Length > 0)
            {
                RoomGameplayBase game = corridorGameplay[Random.Range(0, corridorGameplay.Length)];
                game.Build(corridorRooms[i]);
            }

            if (theOPM)
                theOPM.AddRoom(corridorRooms[i]);
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
