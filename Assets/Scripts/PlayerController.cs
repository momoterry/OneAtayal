using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float AttackCD = 1.0f;
    public float WalkSpeed = 8.0f;
    public GameObject bulletRef;
    public GameObject hitFX;

    public float HP_MaxInit = 100.0f;
    public float MP_MaxInit = 100.0f;
    public float Attack_Init = 50.0f;

    public float MP_Gen_Rate = 30.0f;
    public float MP_PerShoot = 10.0f;

    protected NavMeshAgent myAgent;
    protected float attackWait = 0.0f;

    protected float HP_Max = 100.0f;
    protected float MP_Max = 100.0f;    
    protected float hp = 100.0f;
    protected float mp = 100.0f;
    protected float Attack = 50.0f;

    //升級相關
    protected float HP_Up_Ratio = 0.6f;
    protected float ATK_Up_Ratio = 0.6f;
    protected int HP_UP_Max = 99;
    protected int ATK_UP_MAX = 99;
    protected int HP_Up = 0;
    protected int ATK_Up = 0;

    public float GetHPMax() { return HP_Max; }
    public float GetMPMax() { return MP_Max; }
    public float GetHP() { return hp; }
    public float GetMP() { return mp; }
    public float GetATTACK() { return Attack; }

    public enum PC_STATE
    {
        NONE,
        NORMAL,
        ATTACK, //For 動作
        DEAD,
    }
    protected PC_STATE currState = PC_STATE.NONE;
    protected PC_STATE nextState = PC_STATE.NONE;
    // Start is called before the first frame update
    protected void Start()
    {
        //print("我終於開始寫 Code !!");
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.updateRotation = false;
        myAgent.updateUpAxis = false;

        InitStatus();
    }

    //初始化到等級一的狀態
    public virtual void InitStatus()
    {
        HP_Max = HP_MaxInit;
        MP_Max = MP_MaxInit;
        Attack = Attack_Init;
        HP_Up = 0;
        ATK_Up = 0;

        hp = HP_Max;
        mp = MP_Max;
        if (myAgent)
            myAgent.speed = WalkSpeed;

        nextState = PC_STATE.NORMAL;
    }

    public virtual bool DoHpUp()
    {
        if (HP_Up == HP_UP_Max)
            return false;

        float oldValue = HP_Max;
        HP_Up++;
        HP_Max = HP_MaxInit * (1.0f + HP_Up_Ratio * (float)HP_Up);
        hp *= HP_Max / oldValue; //現有 hp 等比例增加

        return true;
    }

    public virtual bool DoAtkUp()
    {
        if (ATK_Up == ATK_UP_MAX)
            return false;

        ATK_Up++;
        Attack = Attack_Init * (1.0f + ATK_Up_Ratio * (float)ATK_Up);
        return true;
    }

    public bool IsKilled()
    {
        bool result = (currState == PC_STATE.DEAD);
        return result;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (currState != nextState)
        {
            OnStateExit();
            OnStateEnter();
            currState = nextState;
        }
        else
        {
            OnUpdateState();
        }
    }

    protected virtual void OnUpdateState()
    {
        switch (currState)
        {
            case PC_STATE.NORMAL:
                //UpdateAutoAttack();
                UpdateStatus();
                break;
            case PC_STATE.ATTACK:
                UpdateAttack();
                break;
            case PC_STATE.DEAD:
                break;
        }
    }

    protected virtual void OnStateEnter()
    {
        switch (nextState)
        {
            case PC_STATE.DEAD:
                break;
        }
    }

    protected virtual void OnStateExit()
    {

    }

    //private void UpdateAutoAttack()
    //{
    //    if (myAgent.velocity.magnitude < 0.1f)
    //    {
    //        attackWait -= Time.deltaTime;
    //        if (attackWait <= 0.0f)
    //        {
    //            //DoOneAutoAttack();
    //            attackWait += AttackCD;
    //        }
    //    }
    //    else
    //    {
    //        attackWait = AttackCD * 0.5f;
    //    }
    //}

    protected virtual void UpdateAttack() {}

    protected virtual void UpdateStatus()
    {
        mp += MP_Gen_Rate * Time.deltaTime;
        if (mp > MP_Max)
        {
            mp = MP_Max;
        }
    }

    public virtual void OnMoveToPosition(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            myAgent.SetDestination(target);
        }
    }

    public virtual void OnAttackToward(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            DoShootTo(target);
        }
    }

    protected virtual void DoShootTo(Vector3 target)
    {
        if (mp < MP_PerShoot)
        {
            print("沒 Mana 呀 !!!!");
            return;
        }

        GameObject newObj = Instantiate(bulletRef, gameObject.transform.position, Quaternion.identity, null);
        if (newObj)
        {
            bullet newBullet = newObj.GetComponent<bullet>();
            if (newBullet)
            {
                newBullet.SetGroup(DAMAGE_GROUP.PLAYER);
                Vector3 td = target - newObj.transform.position;
                td.z = 0;
                newBullet.targetDir = td.normalized;
                //傷害值，由自己來給
                newBullet.phyDamage = Attack;
            }
        }

        mp -= MP_PerShoot;
    }

    //virtual protected void DoOneAutoAttack()
    //{
    //    if (bulletRef == null)
    //    {
    //        print("Error!! No Bullet for PlayerController !!!!!!!!!!!");
    //        return;
    //    }

    //    Vector2 checkCenter = new Vector2(transform.position.x, transform.position.y);
    //    Collider2D[] result = Physics2D.OverlapCircleAll(checkCenter, 6.0f, LayerMask.GetMask("Character"));

    //    GameObject bestTarget = null;
    //    float bestDis = Mathf.Infinity;
    //    foreach (Collider2D col in result)
    //    {
    //        if (col.CompareTag("Enemy"))
    //        {
    //            //print(col);
    //            Vector3 disV = col.transform.position - transform.position;
    //            disV.z = 0;
    //            float dis = disV.sqrMagnitude;
    //            if (dis < bestDis)
    //            {
    //                bestTarget = col.gameObject;
    //                bestDis = dis;
    //            }
    //        }
    //    }

    //    if (bestTarget)
    //    {
    //        DoShootTo(bestTarget.transform.position);
    //    }

    //}

    void DoDamage(Damage theDamage)
    {
        Instantiate(hitFX, transform.position, Quaternion.identity, null);

        hp -= theDamage.damage;
        if (hp<=0)
        {
            hp = 0;
            nextState = PC_STATE.DEAD;
            BattleSystem.GetInstance().OnPlayerKilled();
        }
    }

    public void DoHeal( float healNum)
    {
        hp += healNum;
        if (hp > HP_Max)
            hp = HP_Max;
    }
}
