using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonEnemyManagerBase : MonoBehaviour
{
    public virtual void AddNormalPosition(Vector3 pos) { }

    public virtual void BuildAllGameplay( float difficultRate = 1.0f ) { }
}

public class DungeonEnemyManager : DungeonEnemyManagerBase
{
    [System.Serializable]
    public class GameplayInfo
    {
        public float rate;
        public GameObject[] enemys;
        public float totalNum;
    }
    public GameplayInfo[] allGameplays;

    protected List<Vector3> normalPosList = new List<Vector3>();
    protected float difficultRate = 1.0f;

    public override void AddNormalPosition(Vector3 pos)
    {
        normalPosList.Add(pos);
    }

    protected void SpawnEnemyGroup(int index, Vector3 pos, GameplayInfo gameInfo)
    {
        GameObject o = new GameObject("NormalGroup " + index);
        o.transform.position = pos;
        EnemyGroup enemyGroup = o.AddComponent<EnemyGroup>();
        enemyGroup.width = 4;
        enemyGroup.height = 4;
        enemyGroup.isRandomEnemyTotal = true;
        enemyGroup.randomEnemyTotal = Mathf.FloorToInt(gameInfo.totalNum * difficultRate);
        enemyGroup.enemyInfos = new EnemyGroup.EnemyInfo[gameInfo.enemys.Length];
        for (int i=0; i<gameInfo.enemys.Length; i++)
        {
            enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
            enemyGroup.enemyInfos[i].enemyRef = gameInfo.enemys[i];
        }
    }

    public override void BuildAllGameplay(float _difficultRate = 1)
    {
        difficultRate = _difficultRate;
        OneUtility.Shuffle(normalPosList);
        int maxPosNum = normalPosList.Count;
        //print("BuildAllGameplay diffculty: " + difficultRate);
        print("BuildAllGameplay : maxPosNum = " + maxPosNum);

        int usedNum = 0;
        foreach (GameplayInfo info in allGameplays)
        {
            int needNum = Mathf.RoundToInt(maxPosNum * info.rate * 0.01f);
            needNum = Mathf.Min(needNum, maxPosNum - usedNum);
            for (int i=0; i<needNum; i++)
            {
                SpawnEnemyGroup(usedNum, normalPosList[usedNum], info);
                usedNum++;
            }
        }
        //foreach (Vector3 pos in normalPosList)
        //{

        //}
    }
}
