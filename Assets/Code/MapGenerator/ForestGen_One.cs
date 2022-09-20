using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForestGen_One : MapGeneratorBase
{
    //public NavMeshSurface2d theSurface2D;
    public GameObject[] roomRefs;
    public GameObject[] gameplayRefs;
    public GameObject startGameRef;     //�_�l Room �ئb���W, �u�� Spawn Gameplay
    public GameObject endRoomRef;       //���� Room Gameplay �T�w, �� Room �@�_ Spawn �Y�i

    public RoomController startRC;

    int toBuild = 5;

    protected List<GameObject> roomList;

    // Start is called before the first frame update
    void Start()
    {
        roomList = new List<GameObject>();
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
        print("ForestGen_One Build All " + buildLevel);
        ClearAll();

        base.BuildAll(buildLevel);

#if XZ_PLAN
        Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
        Quaternion rm = Quaternion.identity;
#endif

        Vector3 pos = transform.position;

        if (startGameRef && buildLevel == 1)
        {
            GameObject ro = Instantiate(startGameRef, pos, rm, null);
            roomList.Add(ro);
        }

        if (startRC && startRC.northDoor)
        {
            pos = startRC.northDoor.position;
        }

        //TODO:  �Ȯɤ��
        for (int i=0; i< roomRefs.Length; i++)
        {
            //pos = pos + new Vector3(0, 20.0f, 0);
            if (roomRefs[0])
            {
                //TODO: �Ȯɤ��
                GameObject ro = Instantiate(roomRefs[i], pos, rm, null);
                if (ro)
                {
                    roomList.Add(ro);

                    RoomController rc = ro.GetComponent<RoomController>();
                    if (rc && rc.southDoor)
                    {
                        pos += rc.transform.position - rc.southDoor.position;
                        ro.transform.position = pos;
                    }
                    else
                    {
                        print("Room Error !! No RoomController or SouthDoor !!");
                    }
                    ro.transform.SetParent(theSurface2D.gameObject.transform);

                    //Gameplay
                    if (gameplayRefs.Length > i && gameplayRefs[i])
                    {
                        GameObject go = Instantiate(gameplayRefs[i], pos, rm, null);
                        if (go)
                            go.transform.SetParent(ro.transform);
                    }

                    if (rc && rc.northDoor)
                    {
                        pos += rc.northDoor.position - rc.transform.position;
                    }
                    else
                    {
                        print("Room Error !! No RoomController or NorthDoor !!");
                    }

                }
            }
        }

        if (endRoomRef)
        {
            GameObject ro = Instantiate(endRoomRef, pos, rm, null);
            if (ro)
            {
                roomList.Add(ro);

                RoomController rc = ro.GetComponent<RoomController>();
                if (rc && rc.southDoor)
                {
                    pos += rc.transform.position - rc.southDoor.position;
                    ro.transform.position = pos;
                }
                else
                {
                    print("Room Error !! No RoomController or SouthDoor !!");
                }
                ro.transform.SetParent(theSurface2D.gameObject.transform);
            }
        }

    }

    void ClearAll()
    {
        foreach (GameObject ro in roomList)
        {
            Destroy(ro);
        }
        roomList.Clear();
    }
}
