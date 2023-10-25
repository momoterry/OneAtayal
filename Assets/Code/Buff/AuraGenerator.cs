using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] randomAuras;

    protected GameObject theAura;

    void Start()
    {
        if (randomAuras.Length > 0)
            theAura = BattleSystem.SpawnGameObj(randomAuras[Random.Range(0, randomAuras.Length)], transform.position);

        //if (theAura)
        //{
        //    theAura.gameObject.transform.parent = transform;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (theAura)
        {
            theAura.gameObject.transform.position = transform.position;
        }
    }
}
