using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyRef;

    public int num = 1;
    public float randomRangeWidth = 0;
    public float randomRangeHeight = 0;

    public GameObject[] triggerTargetWhenAllKilled;

    protected bool traceEnemies = false;
    protected GameObject[] spawnedEnemies;
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
                for (int i = 0; i < num; i++)
                {
                    if (spawnedEnemies[i] != null)
                    {
                        liveNum++;
                    }
                }
                if (liveNum == 0)
                {
                    //¥þ·À
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
        //DO Spawn
        if (enemyRef)
        {
            float rw, rh;
            traceEnemies = true;
            spawnedEnemies = new GameObject[num];
            for (int i = 0; i < num; i++)
            {
                rw = Random.Range(-randomRangeWidth, randomRangeWidth);
                rh = Random.Range(-randomRangeHeight, randomRangeHeight);
#if XZ_PLAN
                GameObject o = Instantiate(enemyRef, transform.position + new Vector3(rw, 0, rh), Quaternion.Euler(90.0f, 0, 0), null);
#else
                GameObject o = Instantiate(enemyRef, transform.position + new Vector3(rw, rhy, 0), Quaternion.identity, null);
#endif
                spawnedEnemies[i] = o;
            }
        }
    }

    
}
