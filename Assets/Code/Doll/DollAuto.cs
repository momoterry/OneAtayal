using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollAuto : Doll
{
    protected float ChaseRangeIn = 5.0f;
    protected float ChaseRangeOut = 8.0f;
    protected float PositionRangeIn = 1.0f;
    protected float PositionRangeOut = 9.0f;
    protected float AttackRangeIn = 3.0f;
    protected float AttackRangeOut = 4.0f;

    protected float RunSpeed = 10.0f;

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
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

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

        Collider[] cols = Physics.OverlapSphere(transform.position, ChaseRangeIn, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            //print("I Found: "+ col.gameObject.name);
            if (col.gameObject.CompareTag("Enemy"))
            {
                float dis = (col.gameObject.transform.position - gameObject.transform.position).magnitude;

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

    void UpdateFollow()
    {
        if (autoStateTime > 0.1f) 
        { 
            if (myAgent)
                myAgent.SetDestination(mySlot.position);
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

        if (autoStateTime > 0.1f)
        {
            float dis = (mySlot.position - transform.position).magnitude;
            if (dis > PositionRangeOut)
            {
                nextAutoState = AutoState.RUNBACK;
            }
            autoStateTime = 0;
        }
    }

    void UpdateAttack()
    {

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
