using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomEnemyGroup : RoomGameplayBase
{
    public EnemyGroupInfo eInfo;
    //public bool isPath;

    public override void Build(MazeGameManager.RoomInfo room)
    {
        int width, height;
        //int num = Mathf.RoundToInt((eInfo.totalNumMax - eInfo.totalNumMin) * room.mainRatio + eInfo.totalNumMin);
        if (room.cell.isPath)
        {
            width = (int)room.doorWidth;
            height = (int)room.doorHeight;
        }
        else
        {
            width = (int)room.width;
            height = (int)room.height;
        }
        //GameObject o = EnemyGroup.SpawnEnemyGroupObject(enemys, num, width, height);
        GameObject o = SpawnEnemyGroupObject(eInfo, room.vCenter, width, height, room.diffAddRatio, room.enemyLV);
        //o.transform.position = room.vCenter;
        o.name = "RoomEnemyGroup_ " + (int)(room.mainRatio * 100.0f);
    }
}
