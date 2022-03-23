using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForestGen_One : MapGeneratorBase
{
    //public NavMeshSurface2d theSurface2D;
    public GameObject[] roomRefs;

    public int roomToGen = 2;

    public RoomController startRC;

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

#if XZ_PLAN
        Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
        Quaternion rm = Quaternion.identity;
#endif

        Vector3 pos = transform.position;
        if (startRC && startRC.northDoor)
        {
            pos = startRC.northDoor.position;
        }

        for (int i=0; i<roomToGen; i++)
        {
            //pos = pos + new Vector3(0, 20.0f, 0);
            if (roomRefs[0])
            {
                GameObject ro = Instantiate(roomRefs[0], pos, rm, null);
                if (ro)
                {
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

    }
}
