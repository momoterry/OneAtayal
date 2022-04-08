using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollAuto : Doll
{
    public float ChaseRangeIn = 5.0f;
    public float ChaseRangeOut = 8.0f;
    public float PositionRangeIn = 1.0f;
    public float PositionRangeOut = 9.0f;
    public float AttackRangeIn = 3.0f;
    public float AttackRangeOut = 4.0f;

    public float attackWait = 0.2f;
    public float attackCD = 0.5f;

    //== 以上其實是 public
    protected float timeToAttack = 0;
    protected float RunSpeed = 10.0f;

    protected GameObject myMaster;

    enum AutoState
    {
        NONE,
        FOLLOW,
        RUNBACK,
        CHASE,
        ATTACK
    }

    private AutoState currAutoState = AutoState.NONE;
    private AutoState nextAutoState = AutoState.NONE;

    private GameObject myTarget;
    private NavMeshAgent myAgent;

    private float autoStateTime;

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
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    protected override void OnStateEnterBattle()
    {
        myAgent.enabled = true;

        myMaster = BattleSystem.GetInstance().GetPlayer();
    }

    private void EnterAutoState(AutoState state)
    {
        switch (state)
        {
            case AutoState.FOLLOW:
                if (myAgent)
                {
                    myAgent.speed = BattleSystem.GetInstance().GetPlayerController().WalkSpeed;
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
                timeToAttack = attackWait;
                StopMove();
                break;
        }
    }
    private void ExitAutoState(AutoState state)
    {

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
            }
        }

    }

    bool SearchEnemy()
    {
        GameObject foundEnemy = null;
        float minDistance = Mathf.Infinity;

        Collider[] cols = Physics.OverlapSphere(myMaster.transform.position, ChaseRangeIn, LayerMask.GetMask("Character"));
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

    bool CheckIfRunBack()
    {
        if (myMaster)
        {
            float dis = Vector3.Distance(myMaster.transform.position, transform.position);
            if ( dis > PositionRangeOut)
            {
                nextAutoState = AutoState.RUNBACK;
                return true;
            }
        }

        return false;
    }

    void UpdateFollow()
    {
        if (myAgent)
            myAgent.SetDestination(mySlot.position);
        if (autoStateTime > 0.1f) 
        { 

            autoStateTime = 0;

            if (SearchEnemy())
            {
                nextAutoState = AutoState.CHASE;
            }
        }
    }

    void UpdateChase()
    {
        if (myTarget)
        {
            if (myAgent)
                myAgent.SetDestination(myTarget.transform.position);
        }
        else
            nextAutoState = AutoState.FOLLOW;

        //TODO: 也需要更新目標
        if (autoStateTime > 0.1f)
        {
            //float dis = (mySlot.position - transform.position).magnitude;
            //if (dis > PositionRangeOut)
            //{
            //    nextAutoState = AutoState.RUNBACK;
            //}
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

    void StopMove()
    {
        if (myAgent)
        {
            myAgent.SetDestination(transform.position);
        }
    }

    void UpdateAttack()
    {
        if (!myTarget)
        {
            if (SearchEnemy())
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
            DoOneAttack();
            timeToAttack = attackCD;
        }

    }

    void UpdateGoBack()
    {
        if (myAgent)
        {
            myAgent.SetDestination(mySlot.position);
            float dis = (mySlot.position - transform.position).magnitude;
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
        bullet b = bulletObj.GetComponent<bullet>();
        if (b)
        {
            b.phyDamage = AttackInit;
            b.SetGroup(DAMAGE_GROUP.PLAYER);
            Vector3 td = myTarget.transform.position - transform.position;
#if XZ_PLAN
            td.y = 0;
#else
            td.z = 0;
#endif
            b.targetDir = td.normalized;
        }
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

}
