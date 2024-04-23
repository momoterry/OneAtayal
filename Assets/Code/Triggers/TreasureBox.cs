using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TreasureBox : MonoBehaviour
{
    [System.Serializable]
    public class RewardInfo
    {
        public GameObject objRef;
        public float numMin;
        public float numMax;
    }
    public RewardInfo[] rewards;
    public float timeToSpawn = 0.5f;
    public Vector2 spawnAreaMax = new Vector2(6, 6);
    public Vector2 spawnAreaIn = new Vector2(2, 2);

    protected float waitTime;

    protected enum Phase
    {
        NONE,
        WAIT,
        TO_SPAWN,
        DONE,
    }
    protected Phase currPhase = Phase.NONE;
    protected Phase nextPhase = Phase.NONE;

    void Start()
    {
        nextPhase = Phase.WAIT;
    }

    // Update is called once per frame
    void Update()
    {
        currPhase = nextPhase;
        if (currPhase == Phase.TO_SPAWN)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                DoSpawnReward();
                nextPhase = Phase.DONE;
            }
        }
    }

    protected void DoSpawnReward()
    {
        print("DoSpawnReward !!!!");
        int totalSpawn = 0;
        int[] spawnNum = new int[rewards.Length];
        for (int i=0; i<rewards.Length; i++)
        {
            spawnNum [i] = OneUtility.FloatToRandomInt(Random.Range(rewards[i].numMin, rewards[i].numMax));
            totalSpawn += spawnNum[i];
        }

        Vector3[] posList = new Vector3[totalSpawn];
        for (int i=0; i<totalSpawn; i++)
        {
            posList[i] = transform.position;
            //posList[i].x += Random.Range(-spawnAreaMax.x * 0.5f, spawnAreaMax.x * 0.5f);
            //posList[i].z += Random.Range(-spawnAreaMax.y * 0.5f, spawnAreaMax.y * 0.5f);
        }
    }

    public void OnTG(GameObject whoTG)
    {
        if (currPhase == Phase.WAIT)
        {
            waitTime = timeToSpawn;
            nextPhase = Phase.TO_SPAWN;
        }
    }

}
