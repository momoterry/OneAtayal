using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomGameplayBase;


public class MR_EnemyGroup : MR_Node
{
    public EnemyGroupInfo eInfo;
    public int width;
    public int height;
    public bool spawnOnStart = false;               //�u�O�ͦ� EnemyGroup �A�Ӥ��O�����ͩ�

    //public bool forceAlert = false;
    //public bool diffToSingle = false;               //�p�G�O true ���ܡA���׷|�ϬM�b����j�צӤ��O�ƶq
    //public bool spawnWithoutConnect = false;        //�p�G�O true ���ܡASpawn ��P���Ǥ��|�۳s //TODO: �̦n�i�H�@���@���]�w
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
