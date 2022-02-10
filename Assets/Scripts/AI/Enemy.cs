using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int ID = -1;
    public float MaxHP = 100.0f;
    public GameObject attackFX;
    public GameObject damageFX;

    public float ChaseRangeIn = 4.0f;
    public float ChaseRangeOut = 7.0f;
    public float AttackRangeIn = 1.0f;
    public float AttackRangeOut = 1.2f;
    public float MeleeRange = 1.5f;
    public float AttackWait = 0.2f;
    public float AttackCD = 1.0f;

    public float Attack = 20.0f;

    protected Animator myAnimcator;
    protected NavMeshAgent myAgent;
    protected float hp;
    protected GameObject targetObj;
    protected Vector3 targetPos;
    protected float chaseCheckTime = 0.2f;
    protected float stateTime = 0.0f;

    protected Damage myDamage;
    protected Hp_BarHandler myHPHandler;

    //等級成長率
    protected float LvUpRatio = 1.4f;

    protected enum AI_STATE
    {
        NONE,
        SPAWN_WAIT,
        IDLE,
        CHASE,
        ATTACK,
    }
    protected AI_STATE currState = AI_STATE.NONE;
    protected AI_STATE nextState = AI_STATE.NONE;


    //private void OnGUI()
    //{
    //    Vector3 sPos = Camera.main.WorldToScreenPoint(transform.position);
    //    Rect debugR = new Rect(sPos.x, Camera.main.pixelHeight - sPos.y, 200, 50);
    //    GUI.TextArea(debugR, MaxHP.ToString() + " / " + Attack.ToString());
    //}

    // Public
    public int GetID() { return ID; }

    // Start is called before the first frame update
    protected void Start()
    {
        myAnimcator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.updateRotation = false;
        myAgent.updateUpAxis = false;
        hp = MaxHP;
        myDamage.damage = Attack;

        myHPHandler = GetComponent<Hp_BarHandler>();

        BattleSystem.GetInstance().AddEnemy(gameObject);

        nextState = AI_STATE.SPAWN_WAIT;
    }

    public virtual void SetUpLevel( int iLv = 1)
    {
        float r = Mathf.Pow(LvUpRatio, (float)(iLv - 1));
        Attack *= r;
        MaxHP *= r;
        hp = MaxHP;
        myDamage.damage = Attack;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (nextState != currState)
        {
            OnStateExit();
            OnStateEnter();
            currState = nextState;
        }
        else
        {
            switch (currState)
            {
                case AI_STATE.SPAWN_WAIT:
                    nextState = AI_STATE.IDLE;
                    break;
                case AI_STATE.IDLE:
                    CheckPlayerIn();
                    break;
                case AI_STATE.CHASE:
                    UpdateChase();
                    break;
                case AI_STATE.ATTACK:
                    UpdateAttack();
                    break;
            }
        }

        if (myHPHandler && currState != AI_STATE.SPAWN_WAIT && currState !=AI_STATE.NONE)
        {
            myHPHandler.SetHP(hp, MaxHP);
        }
    }

    protected virtual void OnStateEnter()
    {
        //print("AI Endter State : " + nextState);
        switch (nextState)
        {
            case AI_STATE.ATTACK:
                stateTime = AttackWait;
                break;
            case AI_STATE.CHASE:
                //至少追擊一次
                if (myAgent)
                    myAgent.SetDestination(targetPos);
                stateTime = chaseCheckTime;
                break;
        }
    }

    protected virtual void OnStateExit()
    {

    }

    private void SetTarget(GameObject o)
    {
        targetObj = o;
        targetPos = o.transform.position;
    }

    private void CheckPlayerIn()
    {
        //TODOL: PC 跟 PlayerCharacter 應該分開檢查
        GameObject po = BattleSystem.GetInstance().GetPlayer();
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if (po && pc && !pc.IsKilled())
        {
            Vector3 dv = po.transform.position - transform.position;
            dv.z = 0.0f;
            if ( dv.sqrMagnitude < ChaseRangeIn* ChaseRangeIn)
            {
                SetTarget(po);
                nextState = AI_STATE.CHASE;
            }
        }
    }

    protected virtual void UpdateChase()
    {
        if (!myAgent || !targetObj)
        {
            nextState = AI_STATE.IDLE;
            return;
        }
        PlayerController thePC = targetObj.GetComponent<PlayerController>();
        if (thePC && thePC.IsKilled())
        {
            nextState = AI_STATE.IDLE;
            return;
        }

        Vector3 dv = targetObj.transform.position - transform.position;
        dv.z = 0.0f;
        if (dv.sqrMagnitude > ChaseRangeOut * ChaseRangeOut)
        {
            //TODO: 如果玩家在視野外，至少追到最後一次目標點
            if ((targetPos - transform.position).sqrMagnitude < 1.0f)
            {
                nextState = AI_STATE.IDLE;
            }
        }
        else if (dv.sqrMagnitude < AttackRangeIn * AttackRangeIn)
        {
            nextState = AI_STATE.ATTACK;
            myAgent.SetDestination(transform.position); //Stop
        }
        else
        {
            //每 chaseCheckTime 更新一次 Target 位置
            stateTime -= Time.deltaTime;
            if (stateTime <= 0)
            {
                targetPos = targetObj.transform.position;
                myAgent.SetDestination(targetPos);
                stateTime = chaseCheckTime;
            }
        }

    }

    protected virtual void UpdateAttack()
    {
        if ( !myAgent || !targetObj )
        {
            nextState = AI_STATE.IDLE;
            return;
        }
        PlayerController thePC = targetObj.GetComponent<PlayerController>();
        if (thePC && thePC.IsKilled())
        {
            nextState = AI_STATE.IDLE;
            return;
        }

        stateTime -= Time.deltaTime;

        if (stateTime <= 0.0f)
        {


            Vector3 dv = targetObj.transform.position - transform.position;
            dv.z = 0.0f;
            if (dv.sqrMagnitude > AttackRangeOut * AttackRangeOut)
            {
                nextState = AI_STATE.CHASE;
            }
            else
            {
                DoOneAttack();
            }

            stateTime = AttackCD;
        }
    }

    //被傷害
    void DoDamage(Damage theDamage)
    {
        if (damageFX)
            Instantiate(damageFX, transform.position, Quaternion.identity, null);
        
        if (myAnimcator)
            myAnimcator.SetTrigger("Hit");

        hp -= theDamage.damage;
        if (hp < 0)
        {
            hp = 0;
            DoDeath();
        }

        //從 Idle 中醒來
        if (currState == AI_STATE.IDLE)
        {
            //TODO: 應該透過子彈來回追發射者
            GameObject po = BattleSystem.GetInstance().GetPlayer();
            SetTarget(po);
            nextState = AI_STATE.CHASE;
        }
    }

    void DoDeath()
    {
        BattleSystem.GetInstance().OnEnemyKilled(gameObject);
        DropManager.GetInstance().OnTryDropByEnemyKilled(this);
        Destroy(gameObject);
        // TODO 死亡演出
    }

    protected virtual void DoOneAttack()
    {
        DoMeleeAttack();
    }

    protected virtual void DoMeleeAttack()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, MeleeRange);
        if (attackFX)
            Instantiate(attackFX, transform.position, Quaternion.identity, null);
        foreach( Collider2D col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
                col.gameObject.SendMessage("DoDamage", myDamage);
        }
    }
}
