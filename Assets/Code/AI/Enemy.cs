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

    public float SpawnWaitTime = 0.1f;    
    public float ChaseRangeIn = 4.0f;
    public float ChaseRangeOut = 7.0f;
    public float AttackRangeIn = 1.0f;
    public float AttackRangeOut = 1.2f;
    public float MeleeRange = 1.5f;
    public float AttackWait = 0.2f;
    public float AttackCD = 1.0f;

    public float Attack = 20.0f;

    protected Animator myAnimator;
    protected NavMeshAgent myAgent;
    protected float hp;
    protected GameObject targetObj;
    protected Vector3 targetPos;

    protected float chaseCheckTime = 0.2f;
    protected float stateTime = 0.0f;

    protected Damage myDamage;
    protected Hp_BarHandler myHPHandler;

    //2D ���V����
    protected Vector3 faceDir;

    //���Ŧ����v
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


    // Public
    public int GetID() { return ID; }

    // Start is called before the first frame update
    protected void Start()
    {
        myAnimator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();
        if (myAgent)
        {
            myAgent.updateRotation = false;
            myAgent.updateUpAxis = false;
        }
        hp = MaxHP;
        myDamage.damage = Attack;

        myHPHandler = GetComponent<Hp_BarHandler>();

        BattleSystem.GetInstance().AddEnemy(gameObject);

        nextState = AI_STATE.SPAWN_WAIT;

        faceDir = Vector3.down;
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
                    stateTime -= Time.deltaTime;
                    if (stateTime <= 0)
                    { 
                        nextState = AI_STATE.IDLE; 
                    }
                    break;
                case AI_STATE.IDLE:
                    UpdateIdle();
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
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //�� Y �ȳ]�wZ
    }

    protected virtual void OnStateEnter()
    {
        //print("AI Endter State : " + nextState);
        switch (nextState)
        {
            case AI_STATE.SPAWN_WAIT:
                stateTime = SpawnWaitTime;
                break;
            case AI_STATE.IDLE:
                if (myAnimator)
                    myAnimator.SetBool("Run", false);
                break;
            case AI_STATE.ATTACK:
                stateTime = AttackWait;
                if (myAnimator)
                    myAnimator.SetBool("Run", false);
                break;
            case AI_STATE.CHASE:
                //�ܤְl���@��
                if (myAgent)
                    myAgent.SetDestination(targetPos);
                stateTime = chaseCheckTime;
                if (myAnimator)
                    myAnimator.SetBool("Run", true);
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

    protected virtual void UpdateIdle()
    {
        //CheckPlayerIn

        //TODOL: PC �� PlayerCharacter ���Ӥ��}�ˬd
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
            //TODO: �p�G���a�b�����~�A�ܤְl��̫�@���ؼ��I
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
            //�C chaseCheckTime ��s�@�� Target ��m
            stateTime -= Time.deltaTime;
            if (stateTime <= 0)
            {
                targetPos = targetObj.transform.position;
                myAgent.SetDestination(targetPos);
                stateTime = chaseCheckTime;

                //��s���V
                faceDir = (targetPos - transform.position);
                faceDir.z = 0;
                faceDir.Normalize();
                if (myAnimator)
                {
                    myAnimator.SetFloat("X", faceDir.x);
                    myAnimator.SetFloat("Y", faceDir.y);
                }
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
                //�ץ����V
                faceDir = dv.normalized;
                if (myAnimator)
                {
                    myAnimator.SetFloat("X", faceDir.x);
                    myAnimator.SetFloat("Y", faceDir.y);
                }
                DoOneAttack();
            }

            stateTime = AttackCD;
        }
    }

    //�Q�ˮ`
    void DoDamage(Damage theDamage)
    {
        if (damageFX)
            Instantiate(damageFX, transform.position, Quaternion.identity, null);
        
        if (myAnimator)
            myAnimator.SetTrigger("Hit");

        hp -= theDamage.damage;
        if (hp < 0)
        {
            hp = 0;
            DoDeath();
        }

        //�q Idle ������
        if (currState == AI_STATE.IDLE)
        {
            //TODO: ���ӳz�L�l�u�Ӧ^�l�o�g��
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
        // TODO ���`�t�X

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
