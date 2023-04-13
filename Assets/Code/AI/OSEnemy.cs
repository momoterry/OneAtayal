using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSEnemy : MonoBehaviour
{
    public float MaxHP = 100.0f;
    public GameObject deadFX;
    public float Attack = 20.0f;
    public int Score = 100;
    public int dropID = -1;

    public float SpawnWaitTime = 0.1f;
    public float SpawnWaitFlyingDistance = 0.0f;

    protected Damage myDamage;
    protected Vector3 flyingStartPos = new Vector3();

    protected enum PHASE
    {
        NONE,
        WAIT,
        BATTLE,
        STOP, //When Payer Fail
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;
    protected float phaseTime = 0;

    protected float hp;
    protected Hp_BarHandler myHPHandler;


    // Start is called before the first frame update
    protected virtual void Start()
    {

        myHPHandler = GetComponent<Hp_BarHandler>();

        int currLevel = BattleSystem.GetInstance().GetCurrLevel();
        SetUpLevel(currLevel);

        hp = MaxHP;
        nextPhase = PHASE.WAIT;
    }

    public virtual void SetUpLevel(int iLv = 1)
    {

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

        OSBattleSystem.GetInstance().OnOSEKilled(gameObject);
        if (DropManager.GetInstance())
        {
            DropManager.GetInstance().DoDropByID(dropID, transform.position);
        }

        Destroy(gameObject);
    }

    void OnGameFail()
    {
        nextPhase = PHASE.STOP;
    }

}
