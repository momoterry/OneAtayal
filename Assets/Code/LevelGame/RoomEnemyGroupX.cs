using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyGroupX : RoomGameplayBase
{
    public EnemyGroupInfo eInfos1;
    public EnemyGroupInfo eInfos2;

    public override void Build(MazeGameManager.RoomInfo room)
    {
        int width = Mathf.FloorToInt(room.width * 0.5f);
        int height = Mathf.FloorToInt(room.height * 0.5f);
        float qWidth = room.width * 0.25f;
        float qHeight = room.height * 0.25f;
        Vector3[] shifts =
        {
            new Vector3(qWidth, 0, qHeight),
            new Vector3(-qWidth, 0, qHeight),
            new Vector3(qWidth, 0, -qHeight),
            new Vector3(-qWidth, 0, -qHeight),
        };
        int r1 = Random.Range(0, 3);
        int r2 = Random.Range(0, 2);
        if (r2 == r1)
            r2 = 3;
        //print("X: " + r1 + "_" + r2);
        GameObject o1 = SpawnEnemyGroupObject(eInfos1, room.vCenter + shifts[r1], width, height, room.diffAddRatio);
        GameObject o2 = SpawnEnemyGroupObject(eInfos2, room.vCenter + shifts[r2], width, height, room.diffAddRatio);
        o1.name = "RoomEnemyGroupX_ " + (int)(room.mainRatio * 100.0f) + "_A";
        o2.name = "RoomEnemyGroupX_ " + (int)(room.mainRatio * 100.0f) + "_B";
    }
}
