using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomGameplayBase;


public class MR_EnemyGroup : MR_Node
{
    public EnemyGroupInfo eInfo;
    public int width;
    public int height;
    public bool spawnOnStart = false;               //只是生成 EnemyGroup ，而不是直接生怪

    //public bool forceAlert = false;
    //public bool diffToSingle = false;               //如果是 true 的話，難度會反映在單體強度而不是數量
    //public bool spawnWithoutConnect = false;        //如果是 true 的話，Spawn 後同類怪不會相連 //TODO: 最好可以一類一類設定
    //public EnemyGroup.FINISH_TYPE groupFinishType = EnemyGroup.FINISH_TYPE.NORMAL;

    public GameObject[] triggerTargetWhenAllKilled;

    protected MazeGameManagerBase.RoomInfo theRoom;
    protected float diffAddRatio = 1.0f;
    protected int enemyLV = 1;

    void Start()
    {
        if (spawnOnStart)
            CreateEnemyGroup();
    }

    public void OnTG(GameObject whoTG)
    {
        if (!spawnOnStart)
            CreateEnemyGroup();
    }

    protected void CreateEnemyGroup()
    {
        //float forceAlertDistance = forceAlert? 999.0f : - 1.0f;

        //GameObject o = SpawnEnemyGroupObject(eInfo, transform.position, width, height, diffAddRatio, enemyLV, 
        //    forceAlertDistance, diffToSingle, triggerTargetWhenAllKilled, spawnWithoutConnect, groupFinishType);
        GameObject o = SpawnEnemyGroupObject(eInfo, transform.position, width, height, diffAddRatio, enemyLV, triggerTargetWhenAllKilled);

        o.name = "MR_EnemyGroup_" + name + "_" + (int)(diffAddRatio * 100.0f + 100.0f);
    }


    public override void OnSetupByRoom(MazeGameManagerBase.RoomInfo room)
    {
        theRoom = room;
        base.OnSetupByRoom(room);
        enemyLV = room.enemyLV;
        diffAddRatio = room.diffAddRatio;
        int originalWidth = width;
        int originalHeight = height;
        switch (shiftDir) 
        {
            case DIRECTION.U:
            case DIRECTION.D:
                width = Mathf.Max(Mathf.RoundToInt(originalWidth * widthRatio), 1);
                height = Mathf.Max(Mathf.RoundToInt(originalHeight * heightRatio), 1);
                break;
            case DIRECTION.L:
            case DIRECTION.R:
                width = Mathf.Max(Mathf.RoundToInt(originalHeight * widthRatio), 1);
                height = Mathf.Max(Mathf.RoundToInt(originalWidth * heightRatio), 1);
                break;
        }
    }
}
