using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockRef;

    public int num = 1;
    public float numAddPerLevel = 0;    //每關卡等級增加隻數，可為小數，累積到 1.0 以上加一隻
    public float randomRangeWidth = 0;
    public float randomRangeHeight = 0;

    public GameObject[] triggerTargetWhenAllKilled;


    protected int numToSpawn;


    // Start is called before the first frame update
    void Start()
    {
        SpawnRocks();
    }

    // Update is called once per frame


    void SpawnRocks()
    {
        //根據關卡等級增加數量
        float fLevelAdd = (float)(BattleSystem.GetInstance().GetCurrLevel() - 1);

        numToSpawn = num + (int)(fLevelAdd * numAddPerLevel);

        //DO Spawn
        if (rockRef)
        {
            float rw, rh;

            for (int i = 0; i < numToSpawn; i++)
            {
                rw = Random.Range(-randomRangeWidth, randomRangeWidth);
                rh = Random.Range(-randomRangeHeight, randomRangeHeight);

                GameObject o = Instantiate(rockRef, transform.position + new Vector3(rw, 0, rh), Quaternion.Euler(90.0f, 0, 0), transform);
            }
        }
    }
}
