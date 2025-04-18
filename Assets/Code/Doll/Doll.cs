using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DOLL_POSITION_TYPE
{
    FRONT,
    MIDDLE,
    BACK,
}
public enum DOLL_JOIN_SAVE_TYPE
{
    FOREVER,    //永久記錄 == 存檔
    BATTLE,     //同場戰鬥中記錄
    NONE,       //不存檔，換場景就會消失
}

public class Doll : MonoBehaviour
{
    public string ID;
    public Sprite icon;
    public DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT;

    public GameObject bulletRef;
    public float bulletInitDis = 0.25f;

    public float AttackInit = 10.0f;
    public float SearchRange = 8.0f;
    public float DamageRatio = 1.0f;        //Damage = AttackInit x DamageRatio
    public float damage
    {
        get { return AttackInit * DamageRatio; }
        set { AttackInit = value/DamageRatio; }
    }

    public bool canRevie = false;

    [System.NonSerialized]
    public DOLL_JOIN_SAVE_TYPE joinSaveType = DOLL_JOIN_SAVE_TYPE.NONE;

    protected DollManager theDollManager;
    protected Transform mySlot;

    protected Damage myDamage;

    protected enum DOLL_STATE
    {
        NONE,
        WAIT,
        BATTLE,
        //SILENCE,
        TEMP_DEATH,      //可以被復活的
    }
    protected DOLL_STATE currState = DOLL_STATE.NONE;
    protected DOLL_STATE nextState = DOLL_STATE.NONE;

    public Transform GetSlot() { return mySlot; }
    public void SetSlot(Transform slot) { mySlot = slot; }

    //Buff 系統相關
    protected float attackDamageOriginal;
    virtual public void SetAttackSpeedRate(float ratio) { }
    virtual public void SetMoveSpeedRate(float ratio) { }
    virtual public void SetHPRate(float ratio) { }
    virtual public void SetDamageRate(float ratio) 
    {
        AttackInit = attackDamageOriginal * ratio;
        myDamage.Init(damage, Damage.OwnerType.DOLL, ID, gameObject);
    }

    //Doll 主動技 相關
    virtual public void StartDollSkill() { }
    virtual public void StopDollSkill() { }
    virtual public void SetFace(Vector3 face) { }
    virtual public float GetAttackCD() { return 0; }

    protected virtual void Awake()
    {
        gameObject.AddComponent<BuffApplierDoll>();
        attackDamageOriginal = AttackInit;
        if (gameObject.GetComponent<DollBuffReceiver>() == null)
        {
            DollBuffReceiver dr = gameObject.AddComponent<DollBuffReceiver>();
            dr.groundFXshift = -0.625f;
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (nextState == DOLL_STATE.NONE)   //以確保一開始就被加入玩家的情況能正確
            nextState = DOLL_STATE.WAIT;

        myDamage.Init(damage, Damage.OwnerType.DOLL, ID, gameObject);
    }

    void OnStateExit()
    {

    }

    protected virtual void OnStateEnterBattle()
    {
    }


    protected virtual void OnDeath()
    {
        gameObject.SendMessage("OnLeavePlayer", SendMessageOptions.DontRequireReceiver);

        if (!canRevie)
        {
            theDollManager.OnDollDestroy(this);
            Destroy(gameObject);
        }
        else
        {
            nextState = DOLL_STATE.TEMP_DEATH;
            PrepareTempDeath();
        }
    }

    public virtual void BacktoPack()
    {
        //print("BackToBack");
        gameObject.SendMessage("OnLeavePlayer", SendMessageOptions.DontRequireReceiver);
        theDollManager.OnDollDestroy(this);
        Destroy(gameObject);
    }

    virtual protected void PrepareTempDeath()
    {
        //暴力法清除連結的特效
        FlashFX[] fxLinked = GetComponentsInChildren<FlashFX>();

        foreach (FlashFX fx in fxLinked)
        {
            Destroy(fx.gameObject);
        }
    }

    public virtual void OnRevive()
    {
        if (currState != DOLL_STATE.TEMP_DEATH)
        {
            return;
        }
        nextState = DOLL_STATE.BATTLE;

        gameObject.SetActive(true);
        transform.position = mySlot.position;
        HitBody hb = GetComponent<HitBody>();
        if (hb)
        {
            hb.DoHeal(Mathf.Infinity);
        }

        theDollManager.OnDollRevive(this);
        gameObject.SendMessage("OnJoinPlayer", SendMessageOptions.DontRequireReceiver);
    }

    void OnStateEnter()
    {
        switch (nextState)
        {
            case DOLL_STATE.BATTLE:
                OnStateEnterBattle();
                break;
            case DOLL_STATE.TEMP_DEATH:
                gameObject.SetActive(false);
                theDollManager.OnDollTempDeath(this);
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
        //OnPlayerShoot(target);
    }

    virtual public void OnPlayerShoot(Vector3 target)
    {
//        //尋找 Enemy
//        GameObject foundEnemy = null;
//        float minDistance = Mathf.Infinity;
//#if XZ_PLAN
//        Collider[] cols = Physics.OverlapSphere(transform.position, SearchRange, LayerMask.GetMask("Character"));
//        foreach (Collider col in cols)
//#else
//        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, SearchRange, LayerMask.GetMask("Character"));
//        foreach (Collider2D col in cols)
//#endif
//        {
//            //print("I Found: "+ col.gameObject.name);
//            if (col.gameObject.CompareTag("Enemy"))
//            {
//#if XZ_PLAN
//                float dis = (col.gameObject.transform.position - gameObject.transform.position).magnitude;
//#else
//                float dis = ((Vector2)col.gameObject.transform.position - (Vector2)gameObject.transform.position).magnitude;
//#endif
//                if (dis < minDistance)
//                {
//                    minDistance = dis;
//                    foundEnemy = col.gameObject;
//                }
//            }
//        }

//        if (foundEnemy && bulletRef)
//        {
//            print("Doll Shoot !!");
//#if XZ_PLAN
//            GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.Euler(90, 0, 0));
//#else
//            GameObject bulletObj = Instantiate(bulletRef, transform.position, Quaternion.identity);
//#endif
//            bullet_base b = bulletObj.GetComponent<bullet_base>();
//            if (b)
//            {
//                //b.baseDamage = AttackInit;
//                //b.SetGroup(FACTION_GROUP.PLAYER);
//                Vector3 td = foundEnemy.transform.position - transform.position;
//#if XZ_PLAN
//                td.y = 0;
//#else
//                td.z = 0;
//#endif
//                //b.targetDir = td.normalized;
//                b.InitValue(FACTION_GROUP.PLAYER, myDamage, td);
//            }
//        }
    }

    virtual public void OnPlayerDead()
    {
        //nextState = DOLL_STATE.SILENCE;
        OnDeath();
    }

    // ===================== 被收集相關行為 ===================== 

    public bool TryJoinThePlayer( DOLL_JOIN_SAVE_TYPE saveType = DOLL_JOIN_SAVE_TYPE.NONE, int group = -1, int index = -1)
    {
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        bool isOK = false;
        if ( pc ){
            theDollManager = pc.GetDollManager();
            if (theDollManager)
            {
                //isOK = theDollManager.AddOneDoll(this/*, positionType*/);
                isOK = theDollManager.AddOneDollWithGivenPosition(this, group, index);
            }
        }

        if (isOK)
        {
            nextState = DOLL_STATE.BATTLE;
            gameObject.SendMessage("OnJoinPlayer", SendMessageOptions.DontRequireReceiver);
            //print("....Doll Join " + ID + " save type: " + saveType);
            int _group = -1, _index = -1;
            theDollManager.GetDollGroupAndIndex(this, ref _group, ref _index);

            joinSaveType = saveType;
            switch (saveType)
            {
                case DOLL_JOIN_SAVE_TYPE.FOREVER:
                    GameSystem.GetPlayerData().AddUsingDoll(ID, _group, _index);
                    break;
                case DOLL_JOIN_SAVE_TYPE.BATTLE:
                    //ContinuousBattleManager.AddCollectedDoll(ID);
                    ContinuousBattleManager.AddCollectedDoll(ID, _group, _index);
                    break;
            }
            return true;
        }
        return false;
    }
}
