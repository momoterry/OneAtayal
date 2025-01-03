using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollAuto : Doll
{
    [System.Serializable]
    public enum SEARCH_CENTER_TYPE
    {
        PLAYER,
        MYSELF,
        SLOT,
    }
    public SEARCH_CENTER_TYPE searchCenter = SEARCH_CENTER_TYPE.PLAYER;

    [System.NonSerialized]
    public float PositionRangeIn = 0.1f;
    public float PositionRangeOut = 9.0f;
    public float AttackRangeIn = 3.0f;
    public float AttackRangeOut = 4.0f;

    public float attackWait = 0.2f;
    public float attackCD = 0.5f;
    [System.NonSerialized]
    public float RunSpeed = 14.0f;

    public GameObject deathFX;

    public Animator myAnimator;
    public SPAnimatorUD mySpAnimator;

    public bool useDefaultAgentSetting = true;  // 為 true 的話，NavMeshAgent 的值會被統一修改
    
    //== 以上其實是 public
    protected float timeToAttack = 0;
    protected float attackCDLeft = 0;

    protected GameObject myMaster;

    protected enum AutoState
    {
        NONE,
        FOLLOW,
        RUNBACK,
        CHASE,
        ATTACK,
        HOLD,
        //WAIT_REVIVE,
    }

    protected AutoState currAutoState = AutoState.NONE;
    protected AutoState nextAutoState = AutoState.NONE;

    protected GameObject myTarget;
    protected NavMeshAgent myAgent;

    //只是為了一開始關掉
    protected HitBody myBody;
    protected Hp_BarHandler myHpHandler;
    protected Collider myCollider;

    //面向
    protected Vector3 myFace = Vector3.back;

    protected float autoStateTime;

    //Buff 系統相關
    protected float attackCDInit;
    protected float maxHPOriginal;
    protected float RunSpeedOriginal;


    public override void SetFace( Vector3 face)
    {
        myFace = face;
    }

    public override float GetAttackCD() { return attackCD; }

    public override void SetAttackSpeedRate(float ratio)
    {
        attackCD = attackCDInit / ratio;
    }

    public override void SetMoveSpeedRate(float ratio) 
    {
        RunSpeed = RunSpeedOriginal * ratio;
    }

    public override void SetHPRate(float ratio)
    {
        float hpOld = myBody.HP_Max;
        float hpNew = maxHPOriginal * ratio;
        myBody.HP_Max = hpNew;
        if (hpNew > hpOld)
        {
            //血量增加的情況，原血量跟著提升
            myBody.DoHeal(hpNew - hpOld);
        }
        else
            myBody.DoHeal(0);    //暴力法，確保 hp <= hpMax
    }

    protected override void Awake()
    {
        base.Awake();
        RunSpeedOriginal = RunSpeed;
        myAgent = GetComponent<NavMeshAgent>();
        if (myAgent)
        {
            myAgent.updateRotation = false;
            myAgent.updateUpAxis = false;
            myAgent.enabled = false;
            if (useDefaultAgentSetting)
            {
                //myAgent.angularSpeed = 1800.0f;
                myAgent.acceleration = 200.0f;
                //myAgent.autoBraking = true;
                //myAgent.stoppingDistance = 0.25f;
                myAgent.radius = 0.1f;
            }
        }

        myBody = GetComponent<HitBody>();
        if (myBody)
            myBody.enabled = false;
        myHpHandler = GetComponent<Hp_BarHandler>();
        if (myHpHandler)
            myHpHandler.enabled = false;
        myCollider = GetComponent<Collider>();
        if (myCollider)
            myCollider.enabled = false;

        attackCDInit = attackCD;
        maxHPOriginal = myBody.HP_Max;
    }

    //// Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        nextAutoState = AutoState.FOLLOW;
        //myAgent = GetComponent<NavMeshAgent>();
        //if (myAgent)
        //{
        //    myAgent.updateRotation = false;
        //    myAgent.updateUpAxis = false;
        //    myAgent.enabled = false;
        //}

        //myBody = GetComponent<HitBody>();
        //if (myBody)
        //    myBody.enabled = false;
        //myHpHandler = GetComponent<Hp_BarHandler>();
        //if (myHpHandler)
        //    myHpHandler.enabled = false;
        //myCollider = GetComponent<Collider>();
        //if (myCollider)
        //    myCollider.enabled = false;

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    protected override void OnStateEnterBattle()
    {
        if (myAgent)
            myAgent.enabled = true;
        if (myBody)
            myBody.enabled = true;
        if (myHpHandler)
            myHpHandler.enabled = true;
        if (myCollider)
            myCollider.enabled = true;
        myMaster = BattleSystem.GetInstance().GetPlayer();
    }

    protected void EnterAutoState(AutoState state)
    {
        switch (state)
        {
            case AutoState.FOLLOW:
                if (myAgent)
                {
                    //myAgent.speed = BattleSystem.GetInstance().GetPlayerController().WalkSpeed;
                    myAgent.speed = RunSpeed;
                }
                break;
            case AutoState.RUNBACK:
            case AutoState.CHASE:
                if (myAgent)
                {
                    myAgent.speed = RunSpeed;
                }              
                break;
            case AutoState.ATTACK:
                timeToAttack = Mathf.Max( attackWait, attackCDLeft);    //如果上一個 CD 還沒結束, 就要等 (不能用甩槍加速)
                StopMove();
                break;
            //case AutoState.WAIT_REVIVE:
            //    gameObject.SetActive(false);
            //    break;
        }
    }
    protected void ExitAutoState(AutoState state)
    {
        //switch (state)
        //{
        //    case AutoState.WAIT_REVIVE:
        //        gameObject.SetActive(true);
        //        transform.position = mySlot.position;
        //        break;
        //}
    }

    protected override void UpdateBattle()
    {
        //base.UpdateBattle();

        if (currAutoState != nextAutoState)
        {
            ExitAutoState(currAutoState);
            EnterAutoState(nextAutoState);
            currAutoState = nextAutoState;
            autoStateTime = 0;
        }
        else
        {
            autoStateTime += Time.deltaTime;
            switch (currAutoState)
            {
                case AutoState.FOLLOW:
                    UpdateFollow();
                    break;
                case AutoState.RUNBACK:
                    UpdateGoBack();
                    break;
                case AutoState.CHASE:
                    UpdateChase();
                    break;
                case AutoState.ATTACK:
                    UpdateAttack();
                    break;
                case AutoState.HOLD:
                    UpdateHold();
                    break;
            }
        }

        attackCDLeft -= Time.deltaTime;
        if (attackCDLeft < 0)
            attackCDLeft = 0;

        if (myAnimator)
        {
            myAnimator.SetFloat("X", myFace.x);
            myAnimator.SetFloat("Y", myFace.z);
        }
        if (mySpAnimator)
        {
            mySpAnimator.SetXY(myFace.x, myFace.z);
        }
    }

    protected virtual bool SearchTarget()
    {
        GameObject foundEnemy = null;
        //float minDistance = Mathf.Infinity;

        Vector3 vCenter = transform.position;
        switch (searchCenter)
        {
            case SEARCH_CENTER_TYPE.PLAYER:
                vCenter = myMaster.transform.position;
                break;
            case SEARCH_CENTER_TYPE.SLOT:
                vCenter = mySlot.position;
                break;
        }

        foundEnemy =  BattleUtility.SearchClosestTargetForPlayer(vCenter, SearchRange);
        if (!foundEnemy)
            foundEnemy = BattleSystem.GetPC().GetHittableTarget();

        //Collider[] cols = Physics.OverlapSphere(vCenter, SearchRange, LayerMask.GetMask("Character"));
        //foreach (Collider col in cols)
        //{
        //    //print("I Found: "+ col.gameObject.name);
        //    if (col.gameObject.CompareTag("Enemy"))
        //    {
        //        float dis = Vector3.Distance(col.gameObject.transform.position, vCenter);

            //        if (dis < minDistance)
            //        {
            //            minDistance = dis;
            //            foundEnemy = col.gameObject;
            //        }
            //    }
            //}

        myTarget = foundEnemy;

        return (foundEnemy!=null);
    }

    protected bool CheckIfRunBack()
    {
        //if (myMaster)
        //{
        //    float dis = Vector3.Distance(myMaster.transform.position, transform.position);
        //    if ( dis > PositionRangeOut)
        //    {
        //        //print("Need To Run Back, Distance: " + dis);
        //        nextAutoState = AutoState.RUNBACK;
        //        return true;
        //    }
        //}
        float dis = Vector3.Distance(mySlot.position, transform.position);
        if (dis > PositionRangeOut)
        {
            //print("Need To Run Back, Distance: " + dis);
            nextAutoState = AutoState.RUNBACK;
            return true;
        }

        return false;
    }

    virtual protected void UpdateFollow()
    {
        float predictTime = Time.deltaTime; //前置量
        if (myAgent)
            myAgent.SetDestination(mySlot.position + BattleSystem.GetPC().GetVelocity() * predictTime) ;

        myFace = BattleSystem.GetPC().GetFaceDir();

        if (autoStateTime > 0.1f) 
        { 

            autoStateTime = 0;

            if (SearchTarget())
            {
                nextAutoState = AutoState.CHASE;
                //直接進攻擊
                float disT = (myTarget.transform.position - transform.position).magnitude;
                if (disT < AttackRangeIn)
                {
                    nextAutoState = AutoState.ATTACK;
                }
            }
        }
    }

    protected void UpdateChase()
    {
        if (myTarget && myTarget.activeInHierarchy)
        {
            if (myAgent)
                myAgent.SetDestination(myTarget.transform.position);

            myFace = (myTarget.transform.position - transform.position).normalized;
        }
        else
        {
            nextAutoState = AutoState.FOLLOW;
            return;
        }
        //TODO: 也需要更新目標
        if (autoStateTime > 0.1f)
        {
            if (CheckIfRunBack())
            {
                return;
            }

            float disT = (myTarget.transform.position - transform.position).magnitude;
            if (disT < AttackRangeIn)
            {
                nextAutoState = AutoState.ATTACK;
            }
            autoStateTime = 0;
        }
    }

    protected void StopMove()
    {
        if (myAgent)
        {
            myAgent.SetDestination(transform.position);
        }
    }

    protected virtual void UpdateAttack()
    {
        if (!myTarget || !myTarget.activeInHierarchy)
        {
            if (SearchTarget())
            {
                nextAutoState = AutoState.CHASE;
            }
            else
                nextAutoState = AutoState.RUNBACK;

            return;
        }

        if (autoStateTime > 0.1f)
        {
            autoStateTime = 0;
            //float dis = (mySlot.position - transform.position).magnitude;
            //if (dis > PositionRangeOut)
            //{
            //    nextAutoState = AutoState.RUNBACK;
            //    return;
            //}
            if (CheckIfRunBack())
            {
                return;
            }

            float disT = (myTarget.transform.position - transform.position).magnitude;
            if (disT > AttackRangeOut)
            {
                nextAutoState = AutoState.CHASE;
                return;
            }
        }

        timeToAttack -= Time.deltaTime;
        if (timeToAttack <= 0)
        {
            myFace = (myTarget.transform.position - transform.position).normalized;

            DoOneAttack();
            timeToAttack = attackCD;
            attackCDLeft = attackCD;    //避免換 State 以後清零
        }

    }

    virtual protected void UpdateGoBack()
    {
        if (myAgent)
        {
            myAgent.SetDestination(mySlot.position);
            float dis = (mySlot.position - transform.position).magnitude;

            myFace = (mySlot.position - transform.position).normalized;

            //print(dis);
            if (dis < PositionRangeIn)
            {
                nextAutoState = AutoState.FOLLOW;
            }
        }
    }


    protected virtual void DoOneAttack()
    {
        Vector3 td = (myTarget.transform.position - transform.position);
        td.y = 0;
        td.Normalize();

        //GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.Euler(90, 0, 0));
        GameObject bulletObj = BattleSystem.SpawnGameObj(bulletRef, transform.position + td * bulletInitDis);

        bullet_base b = bulletObj.GetComponent<bullet_base>();
        if (b)
        {



            b.InitValue(FACTION_GROUP.PLAYER, myDamage, td, myTarget);
        }
    }


    protected override void OnDeath()
    {
        if (deathFX)
        {
#if XZ_PLAN
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rm = Quaternion.identity;
#endif
            Instantiate(deathFX, transform.position, rm, null);
        }
        //if (!canRevie)
        //{
        //    base.OnDeath();
        //}
        //else
        //{
        //    DoWaitRevive();
        //    nextAutoState = AutoState.WAIT_REVIVE;
        //}
        base.OnDeath();
    }


    //Hold 指令相關
    public virtual void StartHoldPosition(Vector3 pos) 
    {
        nextAutoState = AutoState.HOLD;
    }
    public virtual void StopHoldPosition() 
    { 
        if (currAutoState == AutoState.HOLD)
        {
            nextAutoState = AutoState.FOLLOW;
        }
    }

    public virtual void UpdateHold()
    {
        //StopMove();
    }


    public override void OnPlayerAttack(Vector3 target)
    {
        //啥也不做
    }

    public override void OnPlayerShoot(Vector3 target)
    {
        //啥也不做
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currAutoState.ToString());

    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + myFace * 2.0f);
    }
}
