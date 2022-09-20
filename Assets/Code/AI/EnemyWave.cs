using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyWave : MonoBehaviour
{
    [System.Serializable]
    public class SpawnerInfo
    {
        public float timeToWait = 0;
        public GameObject enemyRef;
        public int num = 1;
        public float randomWidth = 0;
        public float randomHeight = 0;
        public float posShiftX = 0;
        public float posShiftZ = 0;
    }

    public SpawnerInfo[] spawners;

    public GameObject[] triggerTargetWhenAllKilled;

    protected bool traceEnemies = false;
    protected GameObject[] spawnedEnemies;
    protected int numToSpawn;
    protected float traceTime = 0;      //檢查是否全滅的間隔

    protected enum Phase
    {
        NONE,
        WAIT,
        WAVE,
        TRACE,  //全 wave 
        FINISH,
    }
    protected Phase currPhase = Phase.NONE;
    protected Phase nextPhase = Phase.NONE;
    protected int currWave = 0;
    protected float waveTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        numToSpawn = 0;
        for (int i=0; i<spawners.Length; i++)
        {
            if (spawners[i].enemyRef)
            {
                numToSpawn += spawners[i].num;
            }
        }

        nextPhase = Phase.WAIT;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currPhase)
        {
            case Phase.WAVE:
                UpdateWave();
                break;
            case Phase.TRACE:
                UpdateTraceEnemy();
                break;
        }
        currPhase = nextPhase;
        //if (traceEnemies)
        //{
        //    traceTime -= Time.deltaTime;
        //    if (traceTime <= 0.0f)
        //    {
        //        traceTime = 0.2f;
        //        int liveNum = 0;
        //        for (int i = 0; i < numToSpawn; i++)
        //        {
        //            if (spawnedEnemies[i] != null)
        //            {
        //                liveNum++;
        //            }
        //        }
        //        if (liveNum == 0)
        //        {
        //            //全滅
        //            traceEnemies = false;
        //            foreach (GameObject o in triggerTargetWhenAllKilled)
        //            {
        //                o.SendMessage("OnTG", gameObject);
        //            }
        //        }
        //    }
        //}
    }

    void DoOneWave( SpawnerInfo spawner)
    {
        if (!spawner.enemyRef)
            return;
        Vector3 pos = new Vector3(spawner.posShiftX, 0, spawner.posShiftZ) + transform.position;
        for (int i = 0; i < spawner.num; i++)
        {
            float rw = Random.Range(-spawner.randomWidth * 0.5f, spawner.randomWidth * 0.5f);
            float rh = Random.Range(-spawner.randomHeight * 0.5f, spawner.randomHeight * 0.5f);
            GameObject o = BattleSystem.GetInstance().SpawnGameplayObject(spawner.enemyRef, pos + new Vector3(rw, 0, rh));

            //TODO: 記錄
        }
    }

    void UpdateWave()
    {
        SpawnerInfo sp = spawners[currWave];
        waveTime += Time.deltaTime;
        if (waveTime > sp.timeToWait)
        {
            //print("Wave No. " + currWave);
            DoOneWave(sp);
            waveTime = 0;
            currWave++;
            if (currWave == spawners.Length)
            {
                nextPhase = Phase.FINISH;
                //print("全部 Spawn 完!!");
                //TODO: 如果要 Trace............
            }
        }
    }

    void UpdateTraceEnemy()
    {

    }

    void OnTG(GameObject whoTG)
    {
        if (currPhase == Phase.WAIT)
        {
            if (numToSpawn == 0)
                nextPhase = Phase.FINISH;
            else
                nextPhase = Phase.WAVE;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < spawners.Length; i++)
        {
            SpawnerInfo sp = spawners[i];
            Vector3 shift = new Vector3(sp.posShiftX, 0, sp.posShiftZ);
            Vector3 size = new Vector3(sp.randomWidth, 2.0f, sp.randomHeight);
            Gizmos.DrawWireCube(transform.position + shift, size);
        }
    }

}
