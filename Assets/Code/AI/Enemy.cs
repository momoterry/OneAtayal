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
    public float AttackWait = 0.2f;
    public float AttackCD = 1.0f;

    public float Attack = 20.0f;

    public float SlotOut = 12.0f;   //如果有指定 Slot 時
    //protected float BattleSlotRangeOut = 12.0f;   //如果有指定 Slot 時
    protected float SlotRangeIn = 0.5f;

    public Animator myAnimator;         //可以指直接外部指定
    public SPAnimator mySPAimator;
    protected NavMeshAgent myAgent;
    protected float hp;
    protected GameObject targetObj;
    protected Vector3 targetPos;

    protected Transform mySlot = null;

    protected float chaseCheckTime = 0.2f;
    protected float stateTime = 0.0f;

    protected Damage myDamage;
    protected Hp_BarHandler myHPHandler;

    //2D 面向相關
    protected Vector3 faceDir;

    //等級成長率
    protected float LvUpRatio = 1.4f;

    //Buff 系統相關
    protected float AttackOriginal;
    protected float HPMaxOriginal;
    protected float AttackCDOriginal;
    protected float AttackWaitOriginal;
    protected float moveSpeedOriginal;

    //因應關卡難度提升時的數值加成
    public void SetDiffcult(float diffRatio)
    {
        MaxHP += (diffRatio - 1.0f) * MaxHP * 1.2f;     //HP 增加率略快
        Attack += (diffRatio - 1.0f) * Attack * 0.5f;   //攻擊增加率一半
    }

    protected enum AI_STATE
    {
        NONE,
        SPAWN_WAIT,
        IDLE,
        CHASE,
        ATTACK,
        TO_SLOT,
        STOP,   //Whem Game Fail
    }
    protected AI_STATE currState = AI_STATE.NONE;
    protected AI_STATE nextState = AI_STATE.NONE;


    // Public
    public int GetDropID() { return ID; }
    public void SetSlot( Transform slot) { mySlot = slot; }

    private void Awake()
    {
        if (!myAnimator)
            myAnimator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();
        if (myAgent)
        {
            myAgent.updateRotation = false;
            myAgent.updateUpAxis = false;
        }

        if (spawnFX)
        {
            BattleSystem.SpawnGameObj(spawnFX, transform.position);
        }

        if (gameObject.GetComponent<EnemyBuffReceiver>() == null)
            gameObject.AddComponent<EnemyBuffReceiver>();

        //Buff 系統用的初始化
        AttackOriginal = Attack;
        HPMaxOriginal = MaxHP;
        AttackCDOriginal = AttackCD;
        AttackWaitOriginal = AttackWait;
        if (myAgent)
            moveSpeedOriginal = myAgent.speed;

        //為了能進入敵人用的 Aura 等 ColliderTrigger
        Rigidbody rd = gameObject.AddComponent<Rigidbody>();
        rd.isKinematic = true;
        rd.useGravity = false;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //if (!myAnimator)
        //    myAnimator = GetComponent<Animator>();
        //myAgent = GetComponent<NavMeshAgent>();
        //if (myAgent)
        //{
        //    myAgent.updateRotation = false;
        //    myAgent.updateUpAxis = false;
        //}
        hp = MaxHP;
        myDamage.Init(Attack, Damage.OwnerType.ENEMY, gameObject.name, gameObject);

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

    //Buff 系統相關
    virtual public void SetAttackSpeedRate(float ratio) 
    {
        AttackCD = AttackCDOriginal / ratio;
        AttackWait = AttackWaitOriginal / ratio;
    }

    virtual public void SetHPRate(float ratio) 
    {
        float hpOld = MaxHP;
        float hpNew = HPMaxOriginal * ratio;
        MaxHP = hpNew;
        if (hpNew > hpOld)
        {
            //最大血量增加的情況，原血量跟著提升
            hp += (hpNew - hpOld);
        }
        hp = Mathf.Min(hp, MaxHP);
    }

    virtual public void SetDamageRate(float ratio)
    {
        Attack = AttackOriginal * ratio;
        myDamage.Init(Attack, Damage.OwnerType.ENEMY, gameObject.name, gameObject);
    }

    virtual public void SetMoveSpeedRate(float ratio)
    {
        if (myAgent)
            myAgent.speed = moveSpeedOriginal * ratio;
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
                case AI_STATE.TO_SLOT:
                    UpdateMoveToSlot();
                    break;
            }
        }

        if (myHPHandler && currState != AI_STATE.SPAWN_WAIT && currState !=AI_STATE.NONE)
        {
            myHPHandler.SetHP(hp, MaxHP);
        }

        PostUpdate();
    }

    protected virtual void PostUpdate() { }

    protected virtual void OnStartAttack()
    {
        if (myAnimator)
            myAnimator.SetBool("Run", false);
        if (mySPAimator)
            mySPAimator.SetIsRun(false);
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
                if (mySPAimator)
                    mySPAimator.SetIsRun(false);
                SetTarget(null);
                break;
            case AI_STATE.ATTACK:
                stateTime = AttackWait;
                //if (myAnimator)
                //    myAnimator.SetBool("Run", false);
                //if (mySPAimator)
                //    mySPAimator.SetIsRun(false);
                OnStartAttack();
                break;
            case AI_STATE.CHASE:
                //至少追擊一次
                if (myAgent)
                    myAgent.SetDestination(targetPos);
                if (myAnimator)
                    myAnimator.SetBool("Run", true);
                if (mySPAimator)
                    mySPAimator.SetIsRun(true);
                stateTime = 0; //確保一開始先找一次目標
                break;
            //case AI_STATE.TO_SLOT:
            //    stateTime = 0;
            //    if (myAnimator)
            //        myAnimator.SetBool("Run", true);
            //    if (mySPAimator)
            //        mySPAimator.SetIsRun(true);
            //    break;
        }
    }

    protected virtual void OnStateExit()
    {

    }

    protected void SetTarget(GameObject o)
    {
        targetObj = o;
        if (o)
            targetPos = o.transform.position;
    }



    protected virtual bool SearchTarget( float givenRange = -1.0f )
    {
        float searchRange = givenRange > 0 ? givenRange : ChaseRangeIn;

        //尋找 Enemy
        GameObject foundTarget = null;
        float minDistance = Mathf.Infinity;
        Collider[] cols = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));
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
            //print("foundTarget Pos: " + foundTarget.transform.position);
            return true;
        }

        return false;
    }

    protected virtual void UpdateIdle()
    {
        stateTime -= Time.deltaTime;
        //if (CheckSlotTooFar())
        //{
        //    nextState = AI_STATE.TO_SLOT;
        //    return;
        //}
        if (stateTime <= 0)
        {
            stateTime = 0.1f;
            if (SearchTarget(AttackRangeIn))
            {
                //已經在射程內，直接攻擊
                nextState = AI_STATE.ATTACK;
            }
            else if (SearchTarget(ChaseRangeIn))
            {
                nextState = AI_STATE.CHASE;
            }
            else
            {
                if (myAgent && mySlot)
                {
                    myAgent.SetDestination(mySlot.transform.position);
                    bool isRun = Vector3.Distance(mySlot.transform.position, transform.position) > 0.1f;
                    if (myAnimator)
                        myAnimator.SetBool("Run", isRun);
                    if (mySPAimator)
                        mySPAimator.SetIsRun(isRun);
                }
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
        if (mySPAimator)
        {
            mySPAimator.SetXY(faceDir.x, faceDir.z);
        }
    }

    protected bool CheckSlotTooFar()
    {
        return mySlot && Vector3.Distance(mySlot.transform.position, transform.position) > SlotOut;
    }

    protected virtual void UpdateMoveToSlot()
    {
        if (!mySlot)
        {
            nextState = AI_STATE.IDLE;
            return;
        }
        stateTime -= Time.deltaTime;

        if (stateTime <= 0)
        {
            stateTime = 0.1f;
            if (Vector3.Distance(transform.position, mySlot.position) > SlotRangeIn)
            {
                if (myAgent)
                    myAgent.SetDestination(mySlot.transform.position);
            }
            else
            {
                nextState = AI_STATE.IDLE;
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

        if (CheckSlotTooFar())
        {
            nextState = AI_STATE.TO_SLOT;
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
        if ( !targetObj || !targetObj.activeInHierarchy)
        {
            nextState = AI_STATE.IDLE;
            return;
        }
        if (CheckSlotTooFar())
        {
            nextState = AI_STATE.TO_SLOT;
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
                //重新找一次目標，因為在攻擊中，使用 AttackRangeOut 的範圍找
                SearchTarget( AttackRangeOut);
                //print("SearchTarget....");
            }

            stateTime = AttackCD;
        }
    }


    public float DoHeal(float healAbsoluteNum, float healRatio)
    {
        float newHp = hp + healAbsoluteNum + MaxHP * healRatio;
        if (newHp >= MaxHP)
            newHp = MaxHP;
        float healResult = newHp - hp;
        hp = newHp;
        return healResult;
    }

    //被傷害
    void OnDamage(Damage theDamage)
    {
        //已經死了，不要再處理以避免重覆擊殺 !!
        if (hp <= 0)
        {
            //print("你已經死了....");
            return;
        }

        float hpPrev = hp;
        hp -= theDamage.damage * BattleSystem.GetAllFriendlyDamageRate();
        if (hp <= 0)
        {
            hp = 0;
            DoDeath();
        }
        //if (theDamage.type == Damage.OwnerType.DOLL)
        //{
        //    print(theDamage.ID + " 造成了 " + (hpPrev - hp) + "的傷害, 它的類型是: " + theDamage.type);
        //}
        BattleStat.AddOneDamage(theDamage, hpPrev - hp);

        //從 Idle 中醒來
        if (currState == AI_STATE.IDLE)
        {
            //TODO: 應該透過子彈來回追發射者
            //GameObject po = BattleSystem.GetInstance().GetPlayer();
            //SetTarget(po);
            //nextState = AI_STATE.CHASE;
            if (theDamage.Owner && (theDamage.Owner.CompareTag("Player") || theDamage.Owner.CompareTag("Doll")))
            {
                SetTarget(theDamage.Owner);
                if (Vector3.Distance(theDamage.Owner.transform.position, transform.position) <= AttackRangeIn)
                {
                    nextState = AI_STATE.ATTACK;
                }
                else
                    nextState = AI_STATE.CHASE;
            }
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
        StopMove();
        nextState = AI_STATE.STOP;
    }

    public void StopMove()
    {
        if (myAgent)
            myAgent.SetDestination(transform.position);
    }

    public float GetHP() { return hp; }

    private void OnGUI()
    {
        if (DebugMenu.IsDebugBattle())
        {
            Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
            thePoint.y = Camera.main.pixelHeight - thePoint.y;
            GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currState.ToString());
        }
    }

    private void OnDrawGizmos()
    {
        if (targetObj)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetObj.transform.position);
        }
        if (mySlot && currState == AI_STATE.TO_SLOT)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, mySlot.transform.position);
        }
    }
}
