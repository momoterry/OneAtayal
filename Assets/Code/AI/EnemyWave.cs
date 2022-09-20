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
    }

    // Update is called once per frame
    void Update()
    {
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

    void OnTG(GameObject whoTG)
    {
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
