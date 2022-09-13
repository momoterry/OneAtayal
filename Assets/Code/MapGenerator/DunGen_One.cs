using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunGen_One : MapGeneratorBase
{
    public GameObject roomRef;
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
        //if (toBuild > 0)
        //{
        //    toBuild--;
        //    if (toBuild == 0)
        //        theSurface2D.BuildNavMesh();
        //}
    }

    public override void BuildAll(int buildLevel = 1)
    {
        base.BuildAll(buildLevel);
    }
}
