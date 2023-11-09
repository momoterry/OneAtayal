using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollAuraGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] randomAuras;

    protected GameObject theAura;

    private void Awake()
    {
        //EnsureSpawnAura();

        //if (theAura)
        //    theAura.SetActive(false);
    }

    void Start()
    {
        //EnsureSpawnAura();

        //if (theAura)
        //    theAura.SetActive(false);
    }

    protected void EnsureSpawnAura()
    {
        if (theAura == null && randomAuras.Length > 0)
            theAura = BattleSystem.SpawnGameObj(randomAuras[Random.Range(0, randomAuras.Length)], transform.position);
    }

    public void OnJoinPlayer()
    {
        EnsureSpawnAura();

        if (theAura)
            theAura.SetActive(true);
    }

    public void OnLeavePlayer()
    {
        if (theAura)
            theAura.SetActive(false);
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
