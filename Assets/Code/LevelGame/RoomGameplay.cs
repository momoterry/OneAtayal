using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class RoomGameplayBase : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroupInfo
    {
        public GameObject[] enemys;
        public string[] enemyIDs;       //如果有指定 ID 的時候，會無視 GameObject 的指定
        public float totalNumMin = 2;
        public float totalNumMax = 3;
        public bool finishWhenEngaged = false;
    }

    public virtual void BuildLayout( MazeGameManagerBase.RoomInfo room, OneMap oMap, RectInt roomRect)
    {}

    public virtual void Build( MazeGameManagerBase.RoomInfo room ) 
    {}

    static public GameObject SpawnEnemyGroupObject(EnemyGroupInfo info, Vector3 vCenter, 
        int width=4, int height=4, float diffAddRate=0, int enemyLV = 1, float forceAlertDistance = -1.0f, bool diffToSingle = false, GameObject[] triggerTargets = null)
    {
        float numF = Random.Range(info.totalNumMin, info.totalNumMax);
        if (!diffToSingle)
            numF *= ( 1 + diffAddRate );
        int num = OneUtility.FloatToRandomInt(numF);
        //print("EG: float: " + numF + " int: "+ num + " diffAddRate: " + diffAddRate);
        //print("EG LV: " + enemyLV);
        GameObject o = new GameObject();
        o.transform.position = vCenter;
        EnemyGroup enemyGroup = o.AddComponent<EnemyGroup>();

        enemyGroup.width = width;
        enemyGroup.height = height;
        if (forceAlertDistance > 0)
        {
            enemyGroup.spwanDistance = Mathf.Max(enemyGroup.spwanDistance, forceAlertDistance);
            enemyGroup.alertDistance = Mathf.Max(enemyGroup.alertDistance, forceAlertDistance);
            enemyGroup.stopDistance = Mathf.Max(enemyGroup.stopDistance, forceAlertDistance);
        }
        else
        {
            enemyGroup.spwanDistance = Mathf.Max(enemyGroup.spwanDistance, (width + height) * 0.75f);
            enemyGroup.alertDistance = Mathf.Max(enemyGroup.alertDistance, (width + height) * 0.5f);
            enemyGroup.stopDistance = Mathf.Max(enemyGroup.stopDistance, (width + height) * 1.0f);
        }
        enemyGroup.isRandomEnemyTotal = true;
        enemyGroup.randomEnemyTotal = num;
        if (diffToSingle)
            enemyGroup.difficulty = 1.0f + diffAddRate;
        int eNum = info.enemys == null ? 0 : info.enemys.Length;
        if (info.enemyIDs != null)
            eNum = Mathf.Max(eNum, info.enemyIDs.Length);
        enemyGroup.enemyInfos = new EnemyGroup.EnemyInfo[eNum];

        if (info.enemys != null)
        {
            for (int i = 0; i < info.enemys.Length; i++)
            {
                enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
                enemyGroup.enemyInfos[i].enemyRef = info.enemys[i];
            }
        }
        if (info.enemyIDs != null)
        {
            for (int i = 0; i < info.enemyIDs.Length; i++)
            {
                if (enemyGroup.enemyInfos[i] == null)
                    enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
                enemyGroup.enemyInfos[i].enemyID = info.enemyIDs[i];
            }
        }
        for (int i=0; i< eNum; i++)
        {
            enemyGroup.enemyInfos[i].LV = enemyLV;
        }

        enemyGroup.SetFinishWhenEngaged(info.finishWhenEngaged);
        enemyGroup.triggerTargetWhenAllKilled = triggerTargets;

        return o;
    }
}

//public class MR_NodeBase : MonoBehaviour
//{
//    public const float ROOM_RELATIVE_SIZE = 10.0f;     //縮放等的基準
//    protected float widthRatio = 1;
//    protected float heightRatio = 1;
//    virtual public void OnSetupByRoom(MazeGameManager.RoomInfo room)
//    {
//        //TODO: Local 位置校正
//        widthRatio = room.width / ROOM_RELATIVE_SIZE;
//        heightRatio = room.height / ROOM_RELATIVE_SIZE;
//    }
//}

public class RoomGameplay : RoomGameplayBase
{
    public GameObject centerGame;

    private void Awake()
    {
        //centerGame.SetActive(false);
    }
    public override void Build( MazeGameManagerBase.RoomInfo room ) 
    {
        base.Build(room);
        if (!centerGame)
            return;
        //print(".... To Spawn Game Object !! " + centerGame.gameObject.name);
        GameObject o = BattleSystem.SpawnGameObj(centerGame, room.vCenter);
        o.SetActive(true);

        foreach (MR_Node node in o.GetComponentsInChildren<MR_Node>())
        {
            node.OnSetupByRoom(room);
        }
    }

}
