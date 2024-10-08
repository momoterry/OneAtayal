using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomGameplayBase;


public class MR_EnemyGroup : MR_Node
{
    public EnemyGroupInfo eInfo;
    public int width;
    public int height;
    public bool spawnOnStart = false;
    public bool forceAlert = false;
    //public bool diffToLV = false;   //如果是 true 的話，難度會反映在等級而不是數量
    public bool diffToSingle = false;   //如果是 true 的話，難度會反映在單體強度而不是數量

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
        float forceAlertDistance = forceAlert? 999.0f : - 1.0f;
        //float _diffAdd = diffToLV ? 0: diffAddRatio;
        //int _LV = diffToLV ? enemyLV + Mathf.RoundToInt(diffAddRatio) : enemyLV;
        GameObject o = SpawnEnemyGroupObject(eInfo, transform.position, width, height, diffAddRatio, enemyLV, forceAlertDistance, diffToSingle, triggerTargetWhenAllKilled);
        //o.transform.position = room.vCenter;
        o.name = "MR_EnemyGroup_" + name + "_" + (int)(diffAddRatio * 100.0f + 100.0f);
    }


    public override void OnSetupByRoom(MazeGameManagerBase.RoomInfo room)
    {
        theRoom = room;
        base.OnSetupByRoom(room);
        enemyLV = room.enemyLV;
        diffAddRatio = room.diffAddRatio;
        width = Mathf.Max(Mathf.RoundToInt(width * widthRatio), 1);
        height = Mathf.Max(Mathf.RoundToInt(height * heightRatio), 1);
    }
}
