using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


public class DungeonEnemyManagerBase : MonoBehaviour
{
    public class PosData
    {
        public Vector3 pos;
        public Vector2 area;
        public float diffAdd;
    }
    public float diffAddRatio = 0;
    public virtual void AddNormalPosition(PosData pData) { }
    public virtual void AddNormalPosition(Vector3 pos, float diffAdd = 0) { }

    public virtual void BuildAllGameplay( float difficultRate = 1.0f ) { }
}

public class DungeonEnemyManager : DungeonEnemyManagerBase
{
    //���ϥμɤO�k�����A�p�G�� Leader ���ܡAenemys �����ǩT�w��
    // 0 => �e�� 1= > ���� 2 => ���
    [System.Serializable]
    public class GameplayInfo
    {
        public float ratePercent;              //�ʤ���
        public GameObject leader;
        public GameObject[] enemys;
        public bool isSurround = false;
        public float totalNum;
    }
    public GameplayInfo[] allGameplays;

    public GameObject[]  randomLeaderAuraRefs;

    //protected class NormalPosData
    //{
    //    public Vector3 pos;
    //    public float diffAdd;
    //}

    protected List<PosData> normalPosList = new List<PosData>();
    //protected List<Vector3> normalPosList = new List<Vector3>();
    protected float difficultRate = 1.0f;

    public override void AddNormalPosition(Vector3 pos, float diffAdd = 0)
    {
        PosData data = new PosData();
        data.pos = pos;
        data.diffAdd = diffAdd;
        normalPosList.Add(data);
    }

    public override void AddNormalPosition(PosData pData)
    {
        normalPosList.Add(pData);
    }

    protected void SpawnEnemyGroup(int index, PosData data, GameplayInfo gameInfo)
    {
        GameObject o = new GameObject("NormalGroup " + index);
        o.transform.position = data.pos;
        EnemyGroup enemyGroup = o.AddComponent<EnemyGroup>();
        enemyGroup.width = (int)data.area.x > 0 ? (int)data.area.x : 4;
        enemyGroup.height = (int)data.area.y > 0 ? (int)data.area.y : 4;
        enemyGroup.isRandomEnemyTotal = true;
        enemyGroup.randomEnemyTotal = Mathf.FloorToInt(gameInfo.totalNum * difficultRate * (data.diffAdd * diffAddRatio + 1.0f));
        enemyGroup.enemyInfos = new EnemyGroup.EnemyInfo[gameInfo.enemys.Length];
        for (int i=0; i<gameInfo.enemys.Length; i++)
        {
            enemyGroup.enemyInfos[i] = new EnemyGroup.EnemyInfo();
            enemyGroup.enemyInfos[i].enemyRef = gameInfo.enemys[i];
        }

    }

    protected void SpawnEnemyFormation(int index, PosData data, GameplayInfo gameInfo)
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

        //TODO: ���ɤO�k�B�z�p�L����
        float dr = difficultRate * (data.diffAdd * diffAddRatio + 1.0f);
        eF.frontCount = Mathf.FloorToInt(gameInfo.totalNum * dr * Random.Range(0.3f, 0.5f));
        eF.middleCount = Mathf.FloorToInt((gameInfo.totalNum * dr - eF.frontCount) * Random.Range(0.5f, 0.7f));
        eF.backCount = Mathf.FloorToInt(gameInfo.totalNum * dr - eF.frontCount - eF.middleCount);

        if (randomLeaderAuraRefs.Length > 0)
        {
            eF.randomAttachRefs = randomLeaderAuraRefs;
        }
    }

    public void SpawnSurroundObject(int index, PosData data, GameplayInfo gameInfo)
    {
        if (data.area.x + data.area.y < 8)
        {
            print("ERROR!! SpawnSurroundObject �ϰ�Ӥp: " + data.area.x + data.area.y);
            return;
        }
        GameObject o = new GameObject("SurroundObject " + index);
        float bandWidth = 1.0f;

        float totalLength = (data.area.x + data.area.y - bandWidth - bandWidth);
        float hHeight = data.area.y * 0.5f - bandWidth;
        float hWidth = data.area.x * 0.5f - bandWidth;

        int totalNum = Mathf.FloorToInt(gameInfo.totalNum * difficultRate * (data.diffAdd * diffAddRatio + 1.0f));
        //totalNum = 100; //����
        float WIDTH_PART = data.area.x - bandWidth;
        for (int i = 0; i< totalNum; i++)
        {
            float l = Random.Range(0, totalLength);
            float w = Random.Range(0, bandWidth);
            float x = l < WIDTH_PART ? (l - hWidth - bandWidth) : (w + hWidth);
            float y = l < WIDTH_PART ? (w + hHeight) : (l - WIDTH_PART - hHeight);
            Vector3 pos = new Vector3(x, 0, y);
            pos = Random.Range(0, 2) == 0 ? pos : -pos;
            GameObject oRef = gameInfo.enemys[Random.Range(0, gameInfo.enemys.Length)];
            GameObject os = BattleSystem.SpawnGameObj(oRef, data.pos + pos);
            os.transform.parent = o.transform;
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
                if (info.isSurround)
                {
                    SpawnSurroundObject(usedNum, normalPosList[usedNum], info);
                }
                else if (info.leader)
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
