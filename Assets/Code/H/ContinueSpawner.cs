using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class ContinueSpawner : MonoBehaviour
{
    public GameObject spawnRef;
    public int totalCount = 2;
    public float stepTime = 0.2f;
    public Vector3 stepShift;

    protected float waitTime = 0;
    protected int currIndex = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currIndex >= 0)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                DoOneSpawn();
                waitTime += stepTime;

                currIndex++;
                if (currIndex >= totalCount)
                {
                    currIndex = -1; //µ²§ô
                }
            }
        }
    }

    protected void DoOneSpawn()
    {
        if (spawnRef)
        {
            BattleSystem.SpawnGameObj(spawnRef, transform.position + currIndex * stepShift);
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (currIndex < 0)
        {
            currIndex = 0;
        }
    }
}
