using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMultiGame : RoomGameplayBase
{
    public override void Build(MazeGameManagerBase.RoomInfo room)
    {
        base.Build(room);

        RoomGameplayBase[] rooms = GetComponentsInChildren<RoomGameplayBase>();
        foreach (RoomGameplayBase ro in rooms)
        {
            if (ro == this)
            {
                //print("天啊，抓到自己了......");
                continue;
            }
            ro.Build(room);
        }
    }

    public override void BuildLayout(MazeGameManagerBase.RoomInfo room, OneMap oMap)
    {
        base.BuildLayout(room, oMap);

        RoomGameplayBase[] rooms = GetComponentsInChildren<RoomGameplayBase>();
        foreach (RoomGameplayBase ro in rooms)
        {
            if (ro == this)
            {
                continue;
            }
            ro.BuildLayout(room, oMap);
        }
    }
}
