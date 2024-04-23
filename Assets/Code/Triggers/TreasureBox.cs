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
    public RewardInfo rewards;
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
