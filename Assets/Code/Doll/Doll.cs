using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DOLL_POSITION_TYPE
{
    FRONT,
    MIDDLE,
    BACK,
}

public class Doll : MonoBehaviour
{
    public DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT;

    public GameObject bulletRef;

    public float AttackInit = 10.0f;
    public float SearchRange = 8.0f;

    protected DollManager theDollManager;
    protected Transform mySlot;

    protected enum DOLL_STATE
    {
        NONE,
        WAIT,
        BATTLE,
    }
    protected DOLL_STATE currState = DOLL_STATE.NONE;
    protected DOLL_STATE nextState = DOLL_STATE.NONE;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        nextState = DOLL_STATE.WAIT;
    }

    void OnStateExit()
    {

    }

    protected virtual void OnStateEnterBattle()
    {
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    void OnStateEnter()
    {
        switch (nextState)
        {
            case DOLL_STATE.BATTLE:
                OnStateEnterBattle();
                break;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (nextState != currState)
        {
            OnStateExit();
            OnStateEnter();
            currState = nextState;
            return;
        }
        
        
        switch (currState)
        {
            case DOLL_STATE.BATTLE:
                UpdateBattle();
                break;
        }

        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //用 Y 值設定Z
    }

    // =====================  跟隨後相關行為 =====================
    virtual protected void UpdateBattle()
    {
        gameObject.transform.position = mySlot.position;
    }

    virtual public void OnPlayerAttack(Vector3 target)
    {
        OnPlayerShoot(target);
    }

    virtual public void OnPlayerShoot(Vector3 target)
    {
        //尋找 Enemy
        GameObject foundEnemy = null;
        float minDistance = Mathf.Infinity;
#if XZ_PLAN
        Collider[] cols = Physics.OverlapSphere(transform.position, SearchRange, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
#else
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, SearchRange, LayerMask.GetMask("Character"));
        foreach (Collider2D col in cols)
#endif
        {
            //print("I Found: "+ col.gameObject.name);
            if (col.gameObject.CompareTag("Enemy"))
            {
#if XZ_PLAN
                float dis = (col.gameObject.transform.position - gameObject.transform.position).magnitude;
#else
                float dis = ((Vector2)col.gameObject.transform.position - (Vector2)gameObject.transform.position).magnitude;
#endif
                if (dis < minDistance)
                {
                    minDistance = dis;
                    foundEnemy = col.gameObject;
                }
            }
        }

        if (foundEnemy && bulletRef)
        {
            print("Doll Shoot !!");
#if XZ_PLAN
            GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.Euler(90, 0, 0));
#else
            GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.identity);
#endif
            bullet_base b = bulletObj.GetComponent<bullet_base>();
            if (b)
            {
                //b.phyDamage = AttackInit;
                //b.SetGroup(DAMAGE_GROUP.PLAYER);
                Vector3 td = foundEnemy.transform.position - transform.position;
#if XZ_PLAN
                td.y = 0;
#else
                td.z = 0;
#endif
                //b.targetDir = td.normalized;
                b.InitValue(DAMAGE_GROUP.PLAYER, AttackInit, td);
            }
        }
    }

    // ===================== 被收集相關行為 ===================== 

    void OnTG(GameObject whoTG)
    {
        //print("OnTG");

        if (currState == DOLL_STATE.WAIT)
        {
            //回應 ActionTrigger 是否成功
            bool actionResult = TryJoinThePlayer();
            whoTG.SendMessage("OnActionResult", actionResult);
            if (actionResult)
            {
                nextState = DOLL_STATE.BATTLE;
            }
        }
    }

    bool TryJoinThePlayer()
    {
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if ( pc ){
            DollManager theDollManager = pc.GetDollManager();
            if (theDollManager)
            {
                mySlot = theDollManager.AddOneDoll(this, positionType);
            }
        }

        if (mySlot)
            return true;
        return false;
    }
}
