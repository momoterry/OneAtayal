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

    protected MazeGameManagerBase.RoomInfo theRoom;
    protected float diffRatio = 1.0f;
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
        GameObject o = SpawnEnemyGroupObject(eInfo, transform.position, width, height, diffRatio, enemyLV, forceAlertDistance);
        //o.transform.position = room.vCenter;
        o.name = "MR_EnemyGroup_" + name + "_" + (int)(diffRatio * 100.0f);
    }


    public override void OnSetupByRoom(MazeGameManagerBase.RoomInfo room)
    {
        theRoom = room;
        base.OnSetupByRoom(room);
        enemyLV = room.enemyLV;
        diffRatio = room.diffAddRatio;
        width = Mathf.Max(Mathf.RoundToInt(width * widthRatio), 1);
        height = Mathf.Max(Mathf.RoundToInt(height * heightRatio), 1);
    }
}
