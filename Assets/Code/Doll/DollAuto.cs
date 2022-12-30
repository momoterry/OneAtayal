using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollAuto : Doll
{
    //public float ChaseRangeIn = 5.0f;
    //public float ChaseRangeOut = 8.0f;
    public float PositionRangeIn = 1.0f;
    public float PositionRangeOut = 9.0f;
    public float AttackRangeIn = 3.0f;
    public float AttackRangeOut = 4.0f;

    public float attackWait = 0.2f;
    public float attackCD = 0.5f;
    public float RunSpeed = 10.0f;

    public GameObject deathFX;

    public Animator myAnimator;
    public SPAnimatorUD mySpAnimator;
    
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

    //// Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        nextAutoState = AutoState.FOLLOW;
        myAgent = GetComponent<NavMeshAgent>();
        if (myAgent)
        {
            myAgent.updateRotation = false;
            myAgent.updateUpAxis = false;
            myAgent.enabled = false;
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
                //case AutoState.WAIT_REVIVE:
                //    transform.position = mySlot.position;
                //    break;
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
        float minDistance = Mathf.Infinity;

        Collider[] cols = Physics.OverlapSphere(myMaster.transform.position, SearchRange, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            //print("I Found: "+ col.gameObject.name);
            if (col.gameObject.CompareTag("Enemy"))
            {
                //float dis = (col.gameObject.transform.position - gameObject.transform.position).magnitude;
                float dis = Vector3.Distance(col.gameObject.transform.position, myMaster.transform.position);

                if (dis < minDistance)
                {
                    minDistance = dis;
                    foundEnemy = col.gameObject;
                }
            }
        }

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
        if (myAgent)
            myAgent.SetDestination(mySlot.position);

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
#if XZ_PLAN
        GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.Euler(90, 0, 0));
#else
        GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.identity);
#endif
        bullet_base b = bulletObj.GetComponent<bullet_base>();
        if (b)
        {
            //b.baseDamage = AttackInit;
            //b.SetGroup(DAMAGE_GROUP.PLAYER);
            Vector3 td = myTarget.transform.position - transform.position;
#if XZ_PLAN
            td.y = 0;
#else
            td.z = 0;
#endif
            //b.targetDir = td.normalized;
            b.InitValue(DAMAGE_GROUP.PLAYER, AttackInit, td, myTarget);
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


    //protected void DoWaitRevive()
    //{
    //    //暴力法清除連結的特效
    //    FlashFX[] fxLinked = GetComponentsInChildren<FlashFX>();

    //    foreach( FlashFX fx in fxLinked)
    //    {
    //        //print("FlashFX Found !! " + fx.gameObject + " ... " + fx.transform.position);
    //        Destroy(fx.gameObject);
    //    }
        
    //}

    //public virtual void OnRevive()
    //{
    //    if (currAutoState != AutoState.WAIT_REVIVE)
    //    {
    //        return;
    //    }
    //    nextAutoState = AutoState.FOLLOW;

    //    gameObject.SetActive(true);
    //    transform.position = mySlot.position;
    //    HitBody hb = GetComponent<HitBody>();
    //    if (hb)
    //    {
    //        hb.DoHeal(Mathf.Infinity);
    //    }
    //}


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
