using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunGen_One : MapGeneratorBase
{
    public GameObject roomRef;
    public RoomDungeon initRD;

    protected int RoomNum = 3;

    protected List<GameObject> roomList = new List<GameObject>();

    int toBuild = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //TODO: 還不知道為什麼，但等多個 Frame 之後才產生 NavMesh 不然會出錯
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

        GameObject currRoom = initRD.gameObject;
        GameObject newRoom = null;

        for (int i= 0; i< RoomNum; i++)
        {
            newRoom = CreateRoom(roomRef, currRoom, DoorDir.N, DoorDir.S);
            if (newRoom)
            {
                print("NewRoom: " + newRoom.transform.position);
                roomList.Add(newRoom);
                //newRoom.transform.SetParent(theSurface2D.gameObject.transform);
            }
            else
            {
                print("ERROR !!!! CreatRoom Fail....");
                return;
            }
            currRoom = newRoom;

            if (i == 0)
            {
                //Try Branch
                roomList.Add(CreateRoom(roomRef, newRoom, DoorDir.W1, DoorDir.E2));
                roomList.Add(CreateRoom(roomRef, newRoom, DoorDir.E2, DoorDir.W1));
            }
        }

        foreach　(GameObject ro in roomList)
        {
            ro.transform.SetParent(theSurface2D.transform);
        }

        //Vector3 pos = transform.position;

        //if (initRD )
        //{
        //    pos = initRD.GetDoorPos(DoorDir.N);
        //}

        //if (roomRef == null)
        //    return;

        //for (int i = 0; i < RoomNum; i++)
        //{
        //    //GameObject ro = Instantiate(roomRef, pos, rm, null);
        //    GameObject ro = BattleSystem.GetInstance().SpawnGameplayObject(roomRef, pos);
        //    if (ro)
        //    {
        //        roomList.Add(ro);

        //        //RoomController rc = ro.GetComponent<RoomController>();
        //        RoomDungeon rd = ro.GetComponent<RoomDungeon>();
        //        if (rd)
        //        {
        //            pos += rd.transform.position - rd.GetDoorPos(DoorDir.S);
        //            ro.transform.position = pos;

        //            rd.SetDoorStatus(DoorDir.S, true);
        //        }
        //        else
        //        {
        //            print("Room Error !! No RoomDungeon !!");
        //        }
        //        ro.transform.SetParent(theSurface2D.gameObject.transform);

        //        //Gameplay
        //        //GameObject go = Instantiate(gameplayRefs[i], pos, rm, null);
        //        //if (go)
        //        //    go.transform.SetParent(ro.transform);

        //        if (i == 0)
        //        {
        //            //Try Branch
        //            CreateRoom(roomRef, ro, DoorDir.W1, DoorDir.E2);
        //            CreateRoom(roomRef, ro, DoorDir.E2, DoorDir.W1);
        //        }

        //        if (rd)
        //        {
        //            pos += rd.GetDoorPos(DoorDir.N) - rd.transform.position;
        //            if (i != RoomNum - 1)
        //            {
        //                rd.SetDoorStatus(DoorDir.N, true);
        //            }
        //        }
        //        else
        //        {
        //            print("Room Error !! No RoomDungeon !!");
        //        }

        //    }


        //}

    }

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
