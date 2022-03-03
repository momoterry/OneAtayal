using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll : MonoBehaviour
{
    public GameObject bulletRef;

    public float AttackInit = 10.0f;
    public float SearchRange = 8.0f;

    protected DollManager theDollManager;
    protected Transform mySlot;

    protected enum DOLL_STATE
    {
        NONE,
        WAIT,
        FOLLOW,
    }
    protected DOLL_STATE currState = DOLL_STATE.NONE;
    protected DOLL_STATE nextState = DOLL_STATE.NONE;
    // Start is called before the first frame update
    void Start()
    {
        nextState = DOLL_STATE.WAIT;
    }

    void OnStateExit()
    {

    }

    void OnStateEnter()
    {

    }

    // Update is called once per frame
    void Update()
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
            case DOLL_STATE.FOLLOW:
                UpdateFollow();
                break;
        }
        
    }

    // =====================  跟隨後相關行為 =====================
    virtual protected void UpdateFollow()
    {
        gameObject.transform.position = mySlot.position;
    }

    virtual public void OnPlayerAttack(Vector3 target)
    {
        print("XX");
    }

    virtual public void OnPlayerShoot(Vector3 target)
    {
        //尋找 Enemy
        GameObject foundEnemy = null;
        float minDistance = Mathf.Infinity;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, SearchRange, LayerMask.GetMask("Character"));
        foreach (Collider2D col in cols)
        {
            //print("I Found: "+ col.gameObject.name);
            if (col.gameObject.CompareTag("Enemy"))
            {
                float dis = ((Vector2)col.gameObject.transform.position - (Vector2)gameObject.transform.position).magnitude;
                if (dis < minDistance)
                {
                    minDistance = dis;
                    foundEnemy = col.gameObject;
                }
            }
        }

        if (foundEnemy && bulletRef)
        {
            GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.identity);
            bullet b = bulletObj.GetComponent<bullet>();
            if (b)
            {
                b.phyDamage = AttackInit;
                b.SetGroup(DAMAGE_GROUP.PLAYER);
                Vector3 td = foundEnemy.transform.position - transform.position;
                td.z = 0;
                b.targetDir = td.normalized;
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
                nextState = DOLL_STATE.FOLLOW;
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
                mySlot = theDollManager.AddOneDoll(this);
            }
        }

        if (mySlot)
            return true;
        return false;
    }
}
