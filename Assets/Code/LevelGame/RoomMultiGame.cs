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
                //print("�ѰڡA���ۤv�F......");
                continue;
            }
            ro.Build(room);
        }
    }
}
