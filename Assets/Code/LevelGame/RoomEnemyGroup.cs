using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomEnemyGroup : RoomGameplayBase
{
    //public GameObject[] enemys;
    //public float totalNumMin = 3;
    //public float totalNumMax = 8;
    public EnemyGroupInfo eInfo;
    public bool isPath;

    public override void Build(MazeGameManager.RoomInfo room)
    {
        //GameObject o = new GameObject("RoomEnemyGroup_ " + (int)(room.mainRatio * 100.0f));
        //o.transform.position = room.vCenter;
        //EnemyGroup enemyGroup = o.AddComponent<EnemyGroup>();
        //if (isPath)
        //{
        //    enemyGroup.width = (int)room.doorWidth;
        //    enemyGroup.height = (int)room.doorHeight;
        //}
        //else
        //{
        //    enemyGroup.width = (int)room.width;
        //    enemyGroup.height = (int)room.height;
        //}
        //enemyGroup.isRandomEnemyTotal = true;
        //enemyGroup.randomEnemyTotal = Mathf.RoundToInt((totalNumMax - totalNumMin) * room.mainRatio + totalNumMin);
        //enemyGroup.enemyInfos = new EnemyGroup.EnemyInfo[enemys.Length];
        //for (int i = 0; i < enemys.Length; i++)
        //{
        //    enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
        //    enemyGroup.enemyInfos[i].enemyRef = enemys[i];
        //}
        int width, height;
        int num = Mathf.RoundToInt((eInfo.totalNumMax - eInfo.totalNumMin) * room.mainRatio + eInfo.totalNumMin);
        if (isPath)
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
        GameObject o = SpawnEnemyGroupObject(eInfo, room.vCenter, width, height, room.mainRatio);
        //o.transform.position = room.vCenter;
        o.name = "RoomEnemyGroup_ " + (int)(room.mainRatio * 100.0f);
    }
}
