using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyRef;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTG(GameObject whoTG)
    {
        //DO Spawn
        if (enemyRef)
        {
            Instantiate(enemyRef, transform.position, Quaternion.identity, null);
        }
    }

    
}
