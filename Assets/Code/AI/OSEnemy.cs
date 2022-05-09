using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSEnemy : MonoBehaviour
{
    public float MaxHP = 100.0f;
    public GameObject deadFX;
    public float Attack = 20.0f;

    public float SpawnWaitTime = 0.1f;
    public float SpawnWaitFlyingDistance = 0.0f;

    protected Vector3 flyingStartPos = new Vector3();

    protected enum PHASE
    {
        NONE,
        WAIT,
        BATTLE,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;
    protected float phaseTime = 0;

    protected float hp;
    protected Hp_BarHandler myHPHandler;

    // Start is called before the first frame update
    void Start()
    {
        hp = MaxHP;
        myHPHandler = GetComponent<Hp_BarHandler>();

        nextPhase = PHASE.WAIT;
    }

    virtual protected void UpdateBattle()
    {
        myHPHandler.SetHP(hp, MaxHP);
    }

    virtual protected void StartBattle() 
    { 
    }

    // Update is called once per frame
    void Update()
    {
        if (nextPhase != currPhase)
        {
            switch (nextPhase)
            {
                case PHASE.WAIT:
                    flyingStartPos = transform.root.position;
                    break;
                case PHASE.BATTLE:
                    StartBattle();
                    break;
            }
            phaseTime = 0;
            currPhase = nextPhase;
        }
        else
        {
            switch (currPhase)
            {
                case PHASE.WAIT:
                    phaseTime += Time.deltaTime;
                    float ratio = phaseTime / SpawnWaitTime;
                    if (phaseTime >= SpawnWaitTime)
                    {
                        ratio = 1.0f;
                        nextPhase = PHASE.BATTLE;
                    }
                    Vector3 flyVec = ratio * SpawnWaitFlyingDistance * Vector3.back;
                    transform.root.position = flyingStartPos + flyVec;
                    break;
                case PHASE.BATTLE:
                    UpdateBattle();
                    break;

            }
        }
    }

    void OnDamage(Damage theDamage)
    {

        hp -= theDamage.damage;
        if (hp < 0)
        {
            hp = 0;
            DoDeath();
        }


    }

    protected void DoDeath()
    {

        Quaternion rm = Quaternion.Euler(90, 0, 0);
       
        if (deadFX)
        {

            Instantiate(deadFX, transform.position, rm, null);
        }

        Destroy(gameObject);
    }


}
