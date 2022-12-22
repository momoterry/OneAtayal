using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int ID = -1;
    public float MaxHP = 100.0f;

    public GameObject spawnFX;

    public GameObject deadFX;
    public GameObject debris;

    public float SpawnWaitTime = 0.1f;    
    public float ChaseRangeIn = 10.0f;
    public float ChaseRangeOut = 12.0f;
    public float AttackRangeIn = 1.0f;
    public float AttackRangeOut = 1.2f;
   // public float MeleeRange = 1.5f;     //TODO 不再需要
    public float AttackWait = 0.2f;
    public float AttackCD = 1.0f;

    public float Attack = 20.0f;

    public Animator myAnimator;         //可以指直接外部指定
    public SPAnimator mySPAimator;
    protected NavMeshAgent myAgent;
    protected float hp;
    protected GameObject targetObj;
    protected Vector3 targetPos;

    protected float chaseCheckTime = 0.2f;
    protected float stateTime = 0.0f;

    protected Damage myDamage;
    protected Hp_BarHandler myHPHandler;

    //2D 面向相關
    protected Vector3 faceDir;

    //等級成長率
    protected float LvUpRatio = 1.4f;

    protected enum AI_STATE
    {
        NONE,
        SPAWN_WAIT,
        IDLE,
        CHASE,
        ATTACK,
        STOP,   //Whem Game Fail
    }
    protected AI_STATE currState = AI_STATE.NONE;
    protected AI_STATE nextState = AI_STATE.NONE;


    // Public
    public int GetID() { return ID; }

    private void Awake()
    {
        if (spawnFX)
        {
            BattleSystem.SpawnGameObj(spawnFX, transform.position);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!myAnimator)
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

#if XZ_PLAN
        faceDir = Vector3.back;
#else
        faceDir = Vector3.down;
#endif
        SetupAnimationDirection();

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
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //用 Y 值設定Z
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
                //至少追擊一次
                if (myAgent)
                    myAgent.SetDestination(targetPos);
                stateTime = 0; //確保一開始先找一次目標
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



    protected virtual bool SearchTarget()
    {
        //尋找 Enemy
        GameObject foundTarget = null;
        float minDistance = Mathf.Infinity;
        Collider[] cols = Physics.OverlapSphere(transform.position, ChaseRangeIn, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player")|| col.gameObject.CompareTag("Doll"))
            {

                float dis = (col.gameObject.transform.position - gameObject.transform.position).magnitude;

                if (dis < minDistance)
                {
                    minDistance = dis;
                    foundTarget = col.gameObject;
                }
            }
        }

        if (foundTarget)
        {
            //print("Enemy Found Target: " + foundTarget);
            SetTarget(foundTarget);
            return true;
        }

        return false;
    }

    protected virtual void UpdateIdle()
    {
        stateTime -= Time.deltaTime;

        if (stateTime <= 0)
        {
            stateTime = 0.1f;
            if (SearchTarget())
            {
                nextState = AI_STATE.CHASE;
            }
        }
    }

    protected void SetupAnimationDirection()
    {
        if (myAnimator)
        {
            myAnimator.SetFloat("X", faceDir.x);
#if XZ_PLAN
            myAnimator.SetFloat("Y", faceDir.z);
#else
                    myAnimator.SetFloat("Y", faceDir.y);
#endif
        }
    }

    protected virtual void UpdateChase()
    {
        if (!myAgent || !targetObj)
        {
            nextState = AI_STATE.IDLE;
            return;
        }
        PlayerControllerBase thePC = targetObj.GetComponent<PlayerControllerBase>();
        if (thePC && thePC.IsKilled())
        {
            nextState = AI_STATE.IDLE;
            return;
        }

        Vector3 dv = targetObj.transform.position - transform.position;
#if XZ_PLAN
        dv.y = 0.0f;
#else
        dv.z = 0.0f;
#endif
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
                //重新找最佳 Target
                if (SearchTarget())
                {
                    targetPos = targetObj.transform.position;
                    myAgent.SetDestination(targetPos);
                    stateTime = chaseCheckTime;

                    //更新面向
                    faceDir = (targetPos - transform.position);
#if XZ_PLAN
                    faceDir.y = 0;
#else
                    faceDir.z = 0;
#endif
                    faceDir.Normalize();
                    SetupAnimationDirection();
                }
            }
        }

    }

    protected virtual void UpdateAttack()
    {
        if ( !myAgent || !targetObj || !targetObj.activeInHierarchy)
        {
            nextState = AI_STATE.IDLE;
            return;
        }
        PlayerControllerBase thePC = targetObj.GetComponent<PlayerControllerBase>();
        if (thePC && thePC.IsKilled())
        {
            nextState = AI_STATE.IDLE;
            return;
        }

        stateTime -= Time.deltaTime;

        if (stateTime <= 0.0f)
        {


            Vector3 dv = targetObj.transform.position - transform.position;
#if XZ_PLAN
            dv.y = 0.0f;
#else
            dv.z = 0.0f;
#endif
            if (dv.sqrMagnitude > AttackRangeOut * AttackRangeOut)
            {
                nextState = AI_STATE.CHASE;
            }
            else
            {
                //修正面向
                faceDir = dv.normalized;
                SetupAnimationDirection();
                DoOneAttack();
            }

            stateTime = AttackCD;
        }
    }

    //被傷害
    void OnDamage(Damage theDamage)
    {
        //if (damageFX)
        //    Instantiate(damageFX, transform.position, Quaternion.identity, null);

        //if (myAnimator)
        //    myAnimator.SetTrigger("Hit");

        //已經死了，不要再處理以避免重覆擊殺 !!
        if (hp <= 0)
        {
            //print("你已經死了....");
            return;
        }

        hp -= theDamage.damage;
        if (hp <= 0)
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

    protected void DoDeath()
    {
        BattleSystem.GetInstance().OnEnemyKilled(gameObject);
        //DropManager.GetInstance().OnTryDropByEnemyKilled(this);
        
#if XZ_PLAN
        Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
        Quaternion rm = Quaternion.identity;
#endif        
        if (deadFX)
        {

            Instantiate(deadFX, transform.position, rm, null);
        }
        if (debris)
        {
            Instantiate(debris, transform.position, rm, null);
        }

        if (DropManager.GetInstance())
        {
            DropManager.GetInstance().DoDropByID(ID, transform.position);
        }

        Destroy(gameObject);
        // TODO 死亡演出
    }

    protected virtual void DoOneAttack()
    {
        //DoMeleeAttack();
    }

    void OnGameFail()
    {
        //TODO: 再想怎麼處理
        //nextState = AI_STATE.STOP;
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currState.ToString());

    //}
}
