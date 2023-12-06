using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


public class DungeonEnemyManagerBase : MonoBehaviour
{
    public float diffAddRatio = 0;
    public virtual void AddNormalPosition(Vector3 pos, float diffAdd = 0) { }

    public virtual void BuildAllGameplay( float difficultRate = 1.0f ) { }
}

public class DungeonEnemyManager : DungeonEnemyManagerBase
{
    //先使用暴力法分類，如果有 Leader 的話，enemys 的順序固定為
    // 0 => 前排 1= > 中排 2 => 後排
    [System.Serializable]
    public class GameplayInfo
    {
        public float ratePercent;              //百分比
        public GameObject leader;
        public GameObject[] enemys;
        public float totalNum;
    }
    public GameplayInfo[] allGameplays;

    public GameObject[]  randomLeaderAuraRefs;

    protected class NormalPosData
    {
        public Vector3 pos;
        public float diffAdd;
    }

    protected List<NormalPosData> normalPosList = new List<NormalPosData>();
    //protected List<Vector3> normalPosList = new List<Vector3>();
    protected float difficultRate = 1.0f;

    public override void AddNormalPosition(Vector3 pos, float diffAdd = 0)
    {
        NormalPosData data = new NormalPosData();
        data.pos = pos;
        data.diffAdd = diffAdd;
        normalPosList.Add(data);
    }

    protected void SpawnEnemyGroup(int index, NormalPosData data, GameplayInfo gameInfo)
    {
        GameObject o = new GameObject("NormalGroup " + index);
        o.transform.position = data.pos;
        EnemyGroup enemyGroup = o.AddComponent<EnemyGroup>();
        enemyGroup.width = 4;
        enemyGroup.height = 4;
        enemyGroup.isRandomEnemyTotal = true;
        enemyGroup.randomEnemyTotal = Mathf.FloorToInt(gameInfo.totalNum * difficultRate * (data.diffAdd * diffAddRatio + 1.0f));
        enemyGroup.enemyInfos = new EnemyGroup.EnemyInfo[gameInfo.enemys.Length];
        for (int i=0; i<gameInfo.enemys.Length; i++)
        {
            enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
            enemyGroup.enemyInfos[i].enemyRef = gameInfo.enemys[i];
        }

    }

    protected void SpawnEnemyFormation(int index, NormalPosData data, GameplayInfo gameInfo)
    {
        GameObject o = new GameObject("FormationGroup " + index);
        o.transform.position = data.pos;
        EFormation eF = o.AddComponent<EFormation>();
        eF.leaderRef = gameInfo.leader;
        eF.frontEnemyRef = gameInfo.enemys[0];
        eF.middleEnemyRef = gameInfo.enemys[1];
        eF.backEnemyRef = gameInfo.enemys[2];
        //eF.frontCount = Mathf.FloorToInt(gameInfo.totalNum * difficultRate * 0.4f);
        //eF.middleCount = Mathf.FloorToInt(gameInfo.totalNum * difficultRate * 0.4f);
        //eF.backCount = Mathf.FloorToInt(gameInfo.totalNum * difficultRate * 0.3f);

        //TODO: 先暴力法處理小兵分布
        float dr = difficultRate * (data.diffAdd * diffAddRatio + 1.0f);
        eF.frontCount = Mathf.FloorToInt(gameInfo.totalNum * dr * Random.Range(0.3f, 0.5f));
        eF.middleCount = Mathf.FloorToInt((gameInfo.totalNum * dr - eF.frontCount) * Random.Range(0.5f, 0.7f));
        eF.backCount = Mathf.FloorToInt(gameInfo.totalNum * dr - eF.frontCount - eF.middleCount);

        if (randomLeaderAuraRefs.Length > 0)
        {
            eF.randomAttachRefs = randomLeaderAuraRefs;
        }
    }

    public override void BuildAllGameplay(float _difficultRate = 1)
    {
        difficultRate = _difficultRate;
        OneUtility.Shuffle(normalPosList);
        int maxPosNum = normalPosList.Count;
        //print("BuildAllGameplay : maxPosNum = " + maxPosNum);

        int usedNum = 0;
        foreach (GameplayInfo info in allGameplays)
        {
            int needNum = Mathf.RoundToInt(maxPosNum * info.ratePercent * 0.01f);
            needNum = Mathf.Min(needNum, maxPosNum - usedNum);
            for (int i=0; i<needNum; i++)
            {
                if (info.leader)
                {
                    SpawnEnemyFormation(usedNum, normalPosList[usedNum], info);
                }
                else
                {
                    SpawnEnemyGroup(usedNum, normalPosList[usedNum], info);
                }
                usedNum++;
            }
        }
        //foreach (Vector3 pos in normalPosList)
        //{

        //}
    }
}
