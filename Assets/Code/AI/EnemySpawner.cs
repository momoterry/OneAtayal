using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyRef;

    public int num = 1;
    public float numAddPerLevel = 0;    //每關卡等級增加隻數，可為小數，累積到 1.0 以上加一隻
    public float randomRangeWidth = 0;
    public float randomRangeHeight = 0;

    public GameObject[] triggerTargetWhenAllKilled;

    protected bool traceEnemies = false;
    protected GameObject[] spawnedEnemies;
    protected int numToSpawn;
    protected float traceTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (traceEnemies)
        {
            traceTime -= Time.deltaTime;
            if (traceTime <= 0.0f)
            {
                traceTime = 0.2f;
                int liveNum = 0;
                for (int i = 0; i < numToSpawn; i++)
                {
                    if (spawnedEnemies[i] != null)
                    {
                        liveNum++;
                    }
                }
                if (liveNum == 0)
                {
                    //全滅
                    traceEnemies = false;
                    foreach (GameObject o in triggerTargetWhenAllKilled)
                    {
                        o.SendMessage("OnTG", gameObject);
                    }
                }
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        //根據關卡等級增加數量
        float fLevelAdd = (float)(BattleSystem.GetInstance().GetCurrLevel() - 1);

        numToSpawn = num + (int)(fLevelAdd * numAddPerLevel);

        //DO Spawn
        if (enemyRef)
        {
            float rw, rh;
            traceEnemies = true;
            spawnedEnemies = new GameObject[numToSpawn];
            for (int i = 0; i < numToSpawn; i++)
            {
                rw = Random.Range(-randomRangeWidth, randomRangeWidth);
                rh = Random.Range(-randomRangeHeight, randomRangeHeight);
#if XZ_PLAN
                GameObject o = Instantiate(enemyRef, transform.position + new Vector3(rw, 0, rh), Quaternion.Euler(90.0f, 0, 0), null);
#else
                GameObject o = Instantiate(enemyRef, transform.position + new Vector3(rw, rh, 0), Quaternion.identity, null);
#endif
                spawnedEnemies[i] = o;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(randomRangeWidth + randomRangeWidth, 2.0f, randomRangeHeight + randomRangeHeight));
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(randomRangeWidth + randomRangeWidth, 2.0f, randomRangeHeight + randomRangeHeight));

    }

}
