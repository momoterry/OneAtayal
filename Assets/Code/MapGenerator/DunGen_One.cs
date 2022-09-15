using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunGen_One : MapGeneratorBase
{
    public GameObject roomRef;
    public RoomDungeon initRD;

    protected int RoomNum = 3;

    protected enum ExtendType
    {
        N,W,E,
        NB, WB, EB
    }

    protected List<GameObject> roomList = new List<GameObject>();

    int toBuild = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TODO: �٤����D������A�����h�� Frame ����~���� NavMesh ���M�|�X��
        if (toBuild > 0)
        {
            toBuild--;
            if (toBuild == 0)
                theSurface2D.BuildNavMesh();
        }

    }

    public override void BuildAll(int buildLevel = 1)
    {
        base.BuildAll(buildLevel);

        if (!initRD)
            return;

        ExtendType[,] extendData = new ExtendType[,] {
            { ExtendType.N, ExtendType.W, ExtendType.WB, ExtendType.N },
            { ExtendType.N, ExtendType.W, ExtendType.NB, ExtendType.W },
            { ExtendType.N, ExtendType.E, ExtendType.EB, ExtendType.N },
            { ExtendType.N, ExtendType.E, ExtendType.NB, ExtendType.E },
            { ExtendType.N, ExtendType.N, ExtendType.WB, ExtendType.N },
            { ExtendType.N, ExtendType.N, ExtendType.EB, ExtendType.N },
        };

        int rdPath = Random.Range(0, extendData.GetLength(0)-1);

        print("DATA " + extendData.GetLength(0) + "  " + extendData.GetLength(1));

        //ExtendType[] extendList = { ExtendType.N, ExtendType.W, ExtendType.WB, ExtendType.N };


        GameObject currRoom = initRD.gameObject;
        GameObject newRoom;

        for (int i = 0; i < extendData.GetLength(1); i++)
        {
            DoorDir dFron = DoorDir.N;
            DoorDir dTo = DoorDir.S;
            bool isMain = true;
            switch (extendData[rdPath, i])
            {
                case ExtendType.N:
                    dFron = DoorDir.N;
                    dTo = DoorDir.S;
                    break;
                case ExtendType.W:
                    dFron = DoorDir.W1;
                    dTo = DoorDir.E2;
                    break;
                case ExtendType.E:
                    dFron = DoorDir.E1;
                    dTo = DoorDir.W2;
                    break;
                case ExtendType.NB:
                    dFron = DoorDir.N;
                    dTo = DoorDir.S;
                    isMain = false;
                    break;
                case ExtendType.WB:
                    dFron = DoorDir.W1;
                    dTo = DoorDir.E2;
                    isMain = false;
                    break;
                case ExtendType.EB:
                    dFron = DoorDir.E1;
                    dTo = DoorDir.W2;
                    isMain = false;
                    break;
            }

            newRoom = CreateRoom(roomRef, currRoom, dFron, dTo);
            if (newRoom)
            {
                print("NewRoom: " + newRoom.transform.position);
                roomList.Add(newRoom);
            }
            else
            {
                print("ERROR !!!! CreatRoom Fail....");
                return;
            }
            if (isMain)
            {
                currRoom = newRoom;
            }

        }

        foreach (GameObject ro in roomList)
        {
            ro.transform.SetParent(theSurface2D.transform);
        }

    }

    //public override void BuildAll(int buildLevel = 1)
    //{
    //    base.BuildAll(buildLevel);

    //    if (!initRD)
    //        return;

    //    GameObject currRoom = initRD.gameObject;
    //    GameObject newRoom = null;

    //    for (int i= 0; i< RoomNum; i++)
    //    {
    //        newRoom = CreateRoom(roomRef, currRoom, DoorDir.N, DoorDir.S);
    //        if (newRoom)
    //        {
    //            print("NewRoom: " + newRoom.transform.position);
    //            roomList.Add(newRoom);
    //            //newRoom.transform.SetParent(theSurface2D.gameObject.transform);
    //        }
    //        else
    //        {
    //            print("ERROR !!!! CreatRoom Fail....");
    //            return;
    //        }
    //        currRoom = newRoom;

    //        if (i == 0)
    //        {
    //            //Try Branch
    //            roomList.Add(CreateRoom(roomRef, newRoom, DoorDir.W1, DoorDir.E2));
    //            roomList.Add(CreateRoom(roomRef, newRoom, DoorDir.E2, DoorDir.W1));
    //        }
    //    }

    //    foreach�@(GameObject ro in roomList)
    //    {
    //        ro.transform.SetParent(theSurface2D.transform);
    //    }

    //}

    protected GameObject CreateRoom( GameObject roRef, GameObject fromRoom, DoorDir fromDoor, DoorDir toDoor)
    {
        RoomDungeon rd = fromRoom.GetComponent<RoomDungeon>();
        if (!rd)
        {
            print("ERROR !! formRoom has no RoomDungeon !!");
            return null;
        }

        Vector3 pos = rd.GetDoorPos(fromDoor);

        RoomDungeon rdNew = roRef.GetComponent<RoomDungeon>();
        if (!rdNew)
        {
            print("ERROR !! roRef has no RoomDungeon !!");
            return null;
        }
        Vector3 toDoorVec = rdNew.GetDoorPos(toDoor) - roRef.transform.position;

        GameObject ro = BattleSystem.GetInstance().SpawnGameplayObject(roRef, pos - toDoorVec);
        rdNew = ro.GetComponent<RoomDungeon>();

        rd.SetDoorStatus(fromDoor, true);
        rdNew.SetDoorStatus(toDoor, true);

        return ro;
    }
}
