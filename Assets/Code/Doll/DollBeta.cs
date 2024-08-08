using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//2023/9/7
//�s�� Doll �[�c�A���C�۰ʤơA��K�V�������D�����欰�Ӱʧ@

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
    public enum RANGE_CENTER_TYPE
    {
        SEARCH_CENTER,
        PLAYER,
    }
    public RANGE_CENTER_TYPE rangeCenter = RANGE_CENTER_TYPE.SEARCH_CENTER;

    public float AttackRangeIn = 3.0f;
    public float AttackRangeOut = 4.0f;

    //public float attackWait = 0.2f;
    public float attackCD = 0.5f;

    //Mana �t�ά���
    public bool isUsingMana = false;
    public float attackManaCost = 0;
    public float MP_Gen_Rate = 0;
    protected float MP_Max = 100.0f;   //TODO: �令�i�H�Q Buff �t�μW�[
    protected float mp = 0;

    public bool attackWhenFollow = false;

    [System.NonSerialized]
    public float RunSpeed = 14.0f;

    public GameObject deathFX;

    public SPAnimatorUD mySpAnimator;

    public bool useDefaultAgentSetting = true;  // �� true ���ܡANavMeshAgent ���ȷ|�Q�Τ@�ק�


    protected PlayerControllerBase thePC;

    protected GameObject myTarget;
    protected NavMeshAgent myAgent;

    //�u�O���F�@�}�l����
    protected HitBody myBody;
    protected Hp_BarHandler myHpHandler;
    protected Collider myCollider;

    //���V
    protected Vector3 myFace = Vector3.back;

    //���A
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

    //Buff �t�ά���
    protected float attackCDInit;
    protected float maxHPOriginal;
    protected float RunSpeedOriginal;

    public override void SetAttackSpeedRate(float ratio)
    {
        attackCD = attackCDInit / ratio;
    }

    public override void SetHPRate(float ratio)
    {
        float hpOld = myBody.HP_Max;
        float hpNew = maxHPOriginal * ratio;
        myBody.HP_Max = hpNew;
        if (hpNew > hpOld)
        {
            //��q�W�[�����p�A���q��۴���
            myBody.DoHeal(hpNew - hpOld);
        }
        else
            myBody.DoHeal(0);    //�ɤO�k�A�T�O hp <= hpMax
    }
    public override void SetMoveSpeedRate(float ratio)
    {
        RunSpeed = RunSpeedOriginal * ratio;
        if (myAgent)
        {
            myAgent.speed = RunSpeed;
        }
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
            myAgent.speed = RunSpeed;
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
        
        if (isUsingMana)
        {
            mp = MP_Max;
        }

        thePC = BattleSystem.GetPC();
    }


    protected override void UpdateBattle()
    {
        if (!thePC)
            return; //TODO: ���S���䥦�ե����覡?

        if (attackCDLeft > 0)
        {
            attackCDLeft = Mathf.Max(attackCDLeft-Time.deltaTime, 0);
        }

        if (isUsingMana)
        {
            if (mp < MP_Max)
            {
                mp += MP_Gen_Rate * Time.deltaTime;
                mp = Mathf.Min(mp, MP_Max);
                if (myHpHandler)
                    myHpHandler.SetMP(mp, MP_Max);
            }
        }

        //================ ���A�����}�l ===================
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
        //================ ���A�������� ===================

        if (mySpAnimator)
        {
            mySpAnimator.SetXY(myFace.x, myFace.z);
        }
    }

    protected void FollowSlot()
    {
        if (myAgent && mySlot)
        {
            float slotDis = Vector3.Distance(mySlot.transform.position, transform.position);
            if (slotDis < 0.5f)
            {
                // �ܾa�� Slot �ɡA����̿� myAgent ���@��
                myAgent.isStopped = true;
                transform.position = Vector3.MoveTowards(transform.position, mySlot.transform.position, RunSpeed * Time.deltaTime);
            }
            else
            {
                myAgent.isStopped = false;
                myAgent.SetDestination(mySlot.transform.position);
            }

        }
    }

    protected virtual void UpdateFollow()
    {
        FollowSlot();
        //if (myAgent && mySlot)
        //{
        //    float slotDis = Vector3.Distance(mySlot.transform.position, transform.position);
        //    if (slotDis < 0.5f)
        //    {
        //        //myAgent.SetDestination(mySlot.position + thePC.GetVelocity() * Time.deltaTime);
        //        // �ܾa�� Slot �ɡA����̿� myAgent ���@��
        //        myAgent.isStopped = true;
        //        transform.position = Vector3.MoveTowards(transform.position, mySlot.transform.position, RunSpeed * Time.deltaTime);
        //    }
        //    else
        //    {
        //        myAgent.isStopped = false;
        //        myAgent.SetDestination(mySlot.transform.position);
        //    }

        //}

        if (attackWhenFollow)
            UpdateSearchAndShoot();

        myFace = BattleSystem.GetPC().GetFaceDir();
        if (!thePC.IsMoving())
            nextPhase = PHASE.ATTACK;
    }

    protected virtual void UpdateAttack()
    {
        FollowSlot();
        //if (myAgent && mySlot)
        //{
        //    myAgent.isStopped = false;
        //    myAgent.SetDestination(mySlot.transform.position);
        //}
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
        if (isUsingMana && mp < attackManaCost)
        {
            //print("Mana ����: " + mp + " / " + MP_Max + " �ݭn: " + attackManaCost);
            return;
        }

        searchCDLeft -= Time.deltaTime;
        if (searchCDLeft > 0)
            return;
        searchCDLeft = 0.1f;    //�קK�C�� Frame ���b��

        GameObject newTarget = SearchNewTarget();
        if (newTarget)
        {
            myTarget = newTarget;
            DoOneAttack();
            attackCDLeft = attackCD;
            if (isUsingMana)
            {
                mp -= attackManaCost;
                if (myHpHandler)
                    myHpHandler.SetMP(mp, MP_Max);
            }
        }
    }

    //�O�d���ª��B�@�覡
    protected virtual bool SearchTarget()
    {
        myTarget = SearchNewTarget();
        return myTarget != null;
    }

    protected virtual GameObject SearchNewTarget()
    {
        GameObject foundEnemy = null;
        //float minDistance = Mathf.Infinity;

        Vector3 vSearchCenter = transform.position;
        switch (searchCenter)
        {
            case SEARCH_CENTER_TYPE.PLAYER:
                vSearchCenter = thePC.transform.position;
                break;
            case SEARCH_CENTER_TYPE.SLOT:
                vSearchCenter = mySlot.position;
                break;
        }
        Vector3 vRangeCenter = vSearchCenter;
        switch (rangeCenter)
        { 
            case RANGE_CENTER_TYPE.PLAYER:
                vRangeCenter = thePC.transform.position;
                break;
        }

        //foundEnemy = BattleUtility.SearchClosestTargetForPlayer(vCenter, SearchRange);        
        //foundEnemy = BattleUtility.SearchBestTargetForPlayer(vSearchCenter, vRangeCenter, SearchRange);       //��g�{�P�_�M�̪�P�_���}
        foundEnemy = BattleUtility.SearchBestTargetsForPlayer(vSearchCenter, vRangeCenter, SearchRange, 3);     //�q�h�ӥؼФ��H����@��
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
            b.InitValue(FACTION_GROUP.PLAYER, myDamage, td, myTarget);
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
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), myAgent.speed.ToString());
    //}
}
