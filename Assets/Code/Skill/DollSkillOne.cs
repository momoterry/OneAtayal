using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollSkillOne : DollSkillBase
{
    public GameObject bulletRef;
    public float AttackSpeedRate = 2.0f;    //射速增加
    public float AttackRangeAdd = -1.0f;     //射程增加
    public float AttackDamageRate = 0.8f;    //傷害增加率

    //protected GameObject bulletRef;
    protected float attackCD;
    protected float attackRange;

    protected float timeToShoot;

    protected NavMeshAgent dollNav;
    protected int originalPriority;
    protected Vector3 myPosition = new();

    // Start is called before the first frame update
    void Start()
    {
        //print("DollSkillOne.Start");
        if (!bulletRef)
            bulletRef = doll.bulletRef;
        attackRange = doll.SearchRange + AttackRangeAdd;
        //attackCD = doll.attackCD / AttackSpeedRate;
        attackCD = doll.GetAttackCD() / AttackSpeedRate;

        dollNav = doll.GetComponent<NavMeshAgent>();
        if (dollNav)
            originalPriority = dollNav.avoidancePriority;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            UpdateHoldShoot();
        }
    }

    protected GameObject SearchTarget()
    {
        GameObject foundEnemy = null;

        foundEnemy = BattleUtility.SearchClosestTargetForPlayer(transform.position, attackRange);
        if (!foundEnemy)
            foundEnemy = BattleSystem.GetPC().GetHittableTarget();

        return foundEnemy;
    }

    protected void DoOneAttack(GameObject target)
    {
        GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.Euler(90, 0, 0));
        Vector3 td = target.transform.position - transform.position;
        td.y = 0;
        td.Normalize();

        myDamage.damage = doll.damage * AttackDamageRate;
        bullet_base b = bulletObj.GetComponent<bullet_base>();
        if (b)
        {
            b.InitValue(FACTION_GROUP.PLAYER, myDamage, td, target);
        }

        doll.SetFace(td);
    }

    protected void UpdateHoldShoot()
    {
        timeToShoot -= Time.deltaTime;
        if (timeToShoot <= 0)
        {
            timeToShoot += attackCD;
            GameObject target = SearchTarget();
            if (target)
            {
                DoOneAttack(target);
            }
        }
        transform.position = myPosition;
        //if (dollNav)
        //{
        //    print("SetDestination " + myPosition);
        //    dollNav.SetDestination(myPosition);
        //}
    }

    protected void StartHoldShoot()
    {
        //doll.StartDollSkill();
        timeToShoot = 0;

        myPosition = transform.position;
        if (dollNav)
        {
            dollNav.avoidancePriority = 20; //避免被輕易推動
            //dollNav.isStopped = false;
        }
    }

    protected void StopHoldShoot()
    {
        //doll.StopDollSkill();
        if (dollNav)
        {
            dollNav.avoidancePriority = originalPriority;
        }
    }

    public override void OnStartSkill(bool active = true)
    {
        base.OnStartSkill(active);

        if (active)
        {
            StartHoldShoot();
        }
        else
        {
            StopHoldShoot();
        }
        //print("DollSkillOne::OnStartSkill " + active);
    }

}
