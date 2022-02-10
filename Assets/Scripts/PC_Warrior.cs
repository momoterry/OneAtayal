using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Warrior : PlayerController
{
    public float AttackMinTime = 1.0f;
    public float AttackMoveSpeed = 3.0f;
    public float DamagePeriod = 0.2f;
    public float DamageRadius = 1.5f;
    //public float AttackDamage = 10.0f;
    public float HPLeechPerHit = 3.0f;
    public float MPLeechPerHit = 3.0f;

    protected Animator myAnimator;       //TODO: 應該交由父類別宣告

    private Vector3 attackTo;
    private Damage myDamage;    //TODO: 應該由父類別宣告
    private float attackTime;
    private float damageTime;

    private int apOriginal;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        apOriginal = myAgent.avoidancePriority;
    }

    public override void InitStatus()
    {
        myAnimator = GetComponent<Animator>();

        base.InitStatus();

        //myDamage.damage = AttackDamage;
        myDamage.damage = Attack;
    }

    //// Update is called once per frame
    //new void Update()
    //{
    //    base.Update();
    //}

    public override void OnAttackToward(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            if (mp >= MP_PerShoot)
            {
                mp -= MP_PerShoot;
                attackTo = target;
                nextState = PC_STATE.ATTACK; 
            }
            else 
            {
                print("Mana 不夠啦 !!!!"); //TODO 遊戲中訊息
            }
        }
    }

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        if (nextState == PC_STATE.ATTACK)
        {
            myAgent.speed = AttackMoveSpeed;
            myAgent.SetDestination(attackTo);
            attackTime = AttackMinTime;
            damageTime = 0;

            myAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
            myAgent.avoidancePriority = 100;

            if (myAnimator)
                myAnimator.SetBool("AttackWW", true);
        }
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
        if (currState == PC_STATE.ATTACK)
        {
            myAgent.speed = WalkSpeed;
            myAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            myAgent.avoidancePriority = apOriginal;
            if (myAnimator)
                myAnimator.SetBool("AttackWW", false);
        }
    }

    protected override void UpdateAttack()
    {
        base.UpdateAttack();

        if ((attackTo - transform.position).sqrMagnitude < 0.1f)
        {
            nextState = PC_STATE.NORMAL;
            return;
        }

        attackTime -= Time.deltaTime;
        if (attackTime <= 0)
        {
            nextState = PC_STATE.NORMAL;
            return;
        }

        damageTime -= Time.deltaTime;
        if (damageTime <=0)
        {
            DoOneDamage();
            damageTime = DamagePeriod;
        }
    }

    private void DoOneDamage()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, DamageRadius);
        //if (attackFX)
        //    Instantiate(attackFX, transform.position, Quaternion.identity, null);

        myDamage.damage = Attack;
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                col.gameObject.SendMessage("DoDamage", myDamage);
                // 吸血
                hp += HPLeechPerHit;
                if (hp > HP_Max)
                    hp = HP_Max;
                // 吸 Mana
                mp += MPLeechPerHit;
                if (mp > MP_Max)
                    mp = MP_Max;
            }
        }
    }

}
