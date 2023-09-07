using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//2023/9/7
//新的 Doll 架構，降低自動化，更貼向直接按主角的行為來動作

public class DollBeta : Doll
{
    [System.Serializable]
    public enum SEARCH_CENTER_TYPE
    {
        PLAYER,
        MYSELF,
        SLOT,
    }
    public SEARCH_CENTER_TYPE searchCenter = SEARCH_CENTER_TYPE.PLAYER;

    public float AttackRangeIn = 3.0f;
    public float AttackRangeOut = 4.0f;

    public float attackWait = 0.2f;
    public float attackCD = 0.5f;
    [System.NonSerialized]
    public float RunSpeed = 14.0f;

    public GameObject deathFX;

    public SPAnimatorUD mySpAnimator;

    public bool useDefaultAgentSetting = true;  // 為 true 的話，NavMeshAgent 的值會被統一修改

    protected PlayerControllerBase thePC;

    protected GameObject myTarget;
    protected NavMeshAgent myAgent;

    //只是為了一開始關掉
    protected HitBody myBody;
    protected Hp_BarHandler myHpHandler;
    protected Collider myCollider;

    //面向
    protected Vector3 myFace = Vector3.back;

    //狀態
    protected enum PHASE
    {
        NONE,
        FOLLOW,
        ATTACK,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.FOLLOW;

    protected float attackCDLeft = 0;
    protected float searchCDLeft = 0;


    private void Awake()
    {
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
    }


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
        //myMaster = BattleSystem.GetPC();
        thePC = BattleSystem.GetPC();
    }


    protected override void UpdateBattle()
    {
        if (!thePC)
            return; //TODO: 有沒有其它校正的方式?

        if (attackCDLeft > 0)
        {
            attackCDLeft = Mathf.Max(attackCDLeft-Time.deltaTime, 0);
        }

        //================ 狀態部份開始 ===================
        currPhase = nextPhase;
        switch (currPhase)
        {
            case PHASE.FOLLOW:
                UpdateFollow();
                break;
            case PHASE.ATTACK:
                UpdateAttack();
                break;
        }
        //================ 狀態部份結束 ===================

        if (mySpAnimator)
        {
            mySpAnimator.SetXY(myFace.x, myFace.z);
        }
    }


    protected virtual void UpdateFollow()
    {
        if (myAgent && mySlot)
            myAgent.SetDestination(mySlot.transform.position);
        myFace = BattleSystem.GetPC().GetFaceDir();
        if (!thePC.IsMoving())
            nextPhase = PHASE.ATTACK;
    }

    protected virtual void UpdateAttack()
    {
        if (myAgent && mySlot)
            myAgent.SetDestination(mySlot.transform.position);
        if (thePC.IsMoving())
        {
            nextPhase = PHASE.FOLLOW;
            myTarget = null;
            return;
        }

        UpdateSearchAndShoot();
        if (myTarget)
            myFace = (myTarget.transform.position - transform.position).normalized;
        else
            myFace = BattleSystem.GetPC().GetFaceDir();
    }

    protected void UpdateSearchAndShoot()
    {
        if (attackCDLeft > 0)
            return;
        searchCDLeft -= Time.deltaTime;
        if (searchCDLeft > 0)
            return;
        searchCDLeft = 0.1f;    //避免每個 Frame 都在找

        GameObject newTarget = SearchTarget();
        if (newTarget)
        {
            myTarget = newTarget;
            DoOneAttack();
            attackCDLeft = attackCD;
        }
    }

    protected virtual GameObject SearchTarget()
    {
        GameObject foundEnemy = null;
        float minDistance = Mathf.Infinity;

        Vector3 vCenter = transform.position;
        switch (searchCenter)
        {
            case SEARCH_CENTER_TYPE.PLAYER:
                vCenter = thePC.transform.position;
                break;
            case SEARCH_CENTER_TYPE.SLOT:
                vCenter = mySlot.position;
                break;
        }

        Collider[] cols = Physics.OverlapSphere(vCenter, SearchRange, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            //print("I Found: "+ col.gameObject.name);
            if (col.gameObject.CompareTag("Enemy"))
            {
                float dis = Vector3.Distance(col.gameObject.transform.position, vCenter);

                if (dis < minDistance)
                {
                    minDistance = dis;
                    foundEnemy = col.gameObject;
                }
            }
        }

        //myTarget = foundEnemy;

        //return (foundEnemy != null);
        return foundEnemy;
    }



    protected void StopMove()
    {
        if (myAgent)
        {
            myAgent.SetDestination(transform.position);
        }
    }

    protected virtual void DoOneAttack()
    {
        Vector3 td = (myTarget.transform.position - transform.position);
        td.y = 0;
        td.Normalize();

        GameObject bulletObj = BattleSystem.SpawnGameObj(bulletRef, transform.position + td * bulletInitDis);

        bullet_base b = bulletObj.GetComponent<bullet_base>();
        if (b)
        {
            b.InitValue(DAMAGE_GROUP.PLAYER, myDamage, td, myTarget);
        }
    }


    protected override void OnDeath()
    {
        if (deathFX)
        {
            Quaternion rm = Quaternion.Euler(90, 0, 0);

            Instantiate(deathFX, transform.position, rm, null);
        }

        base.OnDeath();
    }


    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currPhase.ToString());
    //}
}
