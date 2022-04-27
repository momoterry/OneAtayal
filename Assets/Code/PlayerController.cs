using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[System.Serializable]
public class SkillDef
{
    public float baseDamage = 20.0f;
    public float duration = 0.5f;
    public float manaCost = 0;
}

public enum FaceFrontType
{
    UP,
    RIGHT,
    DOWN,
    LEFT,
}

public class PlayerController : MonoBehaviour
{
    //public float AttackCD = 1.0f;
    public float WalkSpeed = 8.0f;
    public GameObject beenHitFX;

    public GameObject bulletRef;

    public GameObject shootFX_1;
    public GameObject shootFX_2;
    public GameObject meleeFX;
    public GameObject meleeHitFX;

    public SkillDef meleeSkillDef;
    public SkillDef rangeSkillDef;

    public float HP_MaxInit = 100.0f;
    public float MP_MaxInit = 100.0f;
    public float Attack_Init = 50.0f;

    public float MP_Gen_Rate = 30.0f;
    public float MP_PerShoot = 10.0f;

    public float initFaceDirAngle = 180.0f;

    protected NavMeshAgent myAgent;
    protected float attackWait = 0.0f;

    protected float HP_Max = 100.0f;
    protected float MP_Max = 100.0f;    
    protected float hp = 100.0f;
    protected float mp = 100.0f;
    protected float Attack = 50.0f;

    //Input
    MyInputActions theInput;

    //移動和面向
    protected Vector3 faceDir;

    // 近戰用四面向
    //public enum FaceFrontType
    //{
    //    UP,
    //    RIGHT,
    //    DOWN,
    //    LEFT,
    //}
    protected Vector3 faceFront;
    protected FaceFrontType faceFrontType;

    protected Animator myAnimator;

    //互動物件
    protected GameObject actionObject = null;

    //直接傷害相關
    protected Damage myDamage;

    //升級相關
    protected float HP_Up_Ratio = 0.6f;
    protected float ATK_Up_Ratio = 0.6f;
    protected int HP_UP_Max = 99;
    protected int ATK_UP_MAX = 99;
    protected int HP_Up = 0;
    protected int ATK_Up = 0;

    protected Hp_BarHandler myHPHandler;

    public float GetHPMax() { return HP_Max; }
    public float GetMPMax() { return MP_Max; }
    public float GetHP() { return hp; }
    public float GetMP() { return mp; }
    public float GetATTACK() { return Attack; }

    //Doll 相關
    public GameObject DollManagerRef;
    public DollManager GetDollManager() { return myDollManager; }

    protected DollManager myDollManager;

    public enum PC_STATE
    {
        NONE,
        NORMAL,
        ATTACK, //For 動作
        STOP,   //For 外部強迫暫停
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

        myAnimator = GetComponent<Animator>();

        SetupFaceDirByAngle(initFaceDirAngle);

        InitStatus();

        //Input System Bind
        theInput.TheHero.Attack.performed += ctx => OnAttack();
        theInput.TheHero.Shoot.performed += ctx => OnShoot();
        theInput.TheHero.Action.performed += ctx => OnActionKey();
        theInput.TheHero.ShootTo.performed += ctx => OnShootTo();

        myHPHandler = GetComponent<Hp_BarHandler>();

        if (DollManagerRef)
        {
            GameObject dm = Instantiate(DollManagerRef, transform.position, Quaternion.identity, null);
            myDollManager = dm.GetComponent<DollManager>();
            if (myDollManager == null)
            {
                print("ERROR!! No DollManager Found !!!!!! ");
                Destroy(dm);
            }
        }

    }

    private void OnDestroy()
    {
        theInput.Disable();
    }

    private void Awake()
    {
        theInput = new MyInputActions();
        theInput.Enable();
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

        //TODO: 應該放在別的地方
//#if XZ_PLAN
//        faceDir = Vector3.back;
//        faceFront = Vector3.back;
//#else
//        faceDir = Vector3.down;
//        faceFront = Vector3.down;
//#endif
//        faceFrontType = FaceFrontType.DOWN;
    }

    protected void SetupFaceDirByAngle(float angle)
    {
        //faceDir = Vector3.RotateTowards(Vector3.forward, Vector3.right, angle * Mathf.Deg2Rad, 0);
        faceDir = Quaternion.Euler(0, angle, 0) *  Vector3.forward;
        SetupFrontDirection();
    }

    public void DoTeleport(Vector3 position, float faceAngle)
    {
        transform.position = position;
        SetupFaceDirByAngle(faceAngle);
        if (myDollManager)
        {
            myDollManager.SetMasterPosition(transform.position);
            myDollManager.SetMasterDirection(faceDir, faceFrontType);
            myDollManager.ForceMoveAll();
        }
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

        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //用 Y 值設定Z
        if (myHPHandler && currState != PC_STATE.NONE && currState != PC_STATE.DEAD)
        {
            myHPHandler.SetHP(hp, HP_Max);
            myHPHandler.SetMP(mp, MP_Max);
        }

        if (myDollManager && currState != PC_STATE.NONE && currState != PC_STATE.DEAD)
        {
            myDollManager.SetMasterPosition(transform.position);
            myDollManager.SetMasterDirection(faceDir, faceFrontType);
        }
    }

    protected virtual void OnUpdateState()
    {
        switch (currState)
        {
            case PC_STATE.NORMAL:
                UpdateStatus();
                UpdateMoveControl();
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


    protected virtual void UpdateAttack() 
    {
        attackWait -= Time.deltaTime;
        if (attackWait <= 0)
        {
            nextState = PC_STATE.NORMAL;
        }
    }

    protected virtual void UpdateStatus()
    {
        mp += MP_Gen_Rate * Time.deltaTime;
        if (mp > MP_Max)
        {
            mp = MP_Max;
        }
    }

    protected virtual void UpdateMoveControl()
    {
        //TODO: 把輸入交給別的系統

        //float minMove = 0.5f;
        Vector3 moveVec = Vector3.zero;
        bool bMove = false;

        Vector2 inputVec = theInput.TheHero.Move.ReadValue<Vector2>();

        if (inputVec.magnitude > 0.5)
        {
            bMove = true;
#if XZ_PLAN
            moveVec = new Vector3(inputVec.x, 0, inputVec.y);   //XZ Plan
            moveVec.Normalize();
#else
            moveVec = inputVec;   //XY Plan
            moveVec.Normalize();
            moveVec.z = moveVec.y * 0.01f;
#endif
        }


        if (bMove)
        {
            //TODO: 不要每 Frame 進行
            //OnMoveToPosition(transform.position + moveVec);
            transform.position = transform.position + moveVec * WalkSpeed * Time.deltaTime;

            faceDir = moveVec;

#if XZ_PLAN
            faceDir.y = 0;      //XZ Plan
#else
            faceDir.z = 0;      //XY Plan

#endif

            SetupFrontDirection();
        }


        if (myAnimator)
        {
            myAnimator.SetBool("Run", (myAgent.velocity.magnitude > 0.1f)||bMove);
            myAnimator.SetFloat("X", faceFront.x);
#if XZ_PLAN
            myAnimator.SetFloat("Y", faceFront.z);
#else
            myAnimator.SetFloat("Y", faceFront.y);
#endif
        }
    }

    private void SetupFrontDirection()
    {
#if XZ_PLAN
        if (faceDir.z > faceDir.x)
        {
            if (faceDir.z > -faceDir.x)
            {
                faceFront = Vector3.forward;
                faceFrontType = FaceFrontType.UP;
            }
            else
            {
                faceFront = Vector3.left;
                faceFrontType = FaceFrontType.LEFT;
            }
        }
        else
        {
            if (faceDir.z > -faceDir.x)
            {
                faceFront = Vector3.right;
                faceFrontType = FaceFrontType.RIGHT;
            }
            else
            {
                faceFront = Vector3.back;
                faceFrontType = FaceFrontType.DOWN;
            }
        }
        if (myAnimator)
        {
            myAnimator.SetFloat("X", faceFront.x);
            myAnimator.SetFloat("Y", faceFront.z);
        }

#else
        if (faceDir.y > faceDir.x)
        {
            if ( faceDir.y > -faceDir.x)
            {
                faceFront = Vector3.up;
                faceFrontType = FaceFrontType.UP;
            }
            else
            {
                faceFront = Vector3.left;
                faceFrontType = FaceFrontType.LEFT;
            }
        }
        else
        {
            if (faceDir.y > -faceDir.x)
            {
                faceFront = Vector3.right;
                faceFrontType = FaceFrontType.RIGHT;
            }
            else
            {
                faceFront = Vector3.down;
                faceFrontType = FaceFrontType.DOWN;
            }
        }

        if (myAnimator)
        {
            myAnimator.SetFloat("X", faceFront.x);
            myAnimator.SetFloat("Y", faceFront.z);
        }
#endif

    }

//private void OnGUI()
//{
//    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position+faceDir);
//    thePoint.y = Camera.main.pixelHeight - thePoint.y;
//    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), "FACE");
//    thePoint = Camera.main.WorldToScreenPoint(transform.position + faceFront);
//    thePoint.y = Camera.main.pixelHeight - thePoint.y;
//    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), "X");
//}


public virtual void OnMoveToPosition(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            myAgent.SetDestination(target);
        }
    }

    //=================== 互動物件相關 ===================
    void OnActionKey()
    {
        if (actionObject)
        {
            actionObject.SendMessage("OnAction");
        }
    }

    public void OnRegisterActionObject( GameObject obj )
    {
        if (actionObject == null)
        {
            actionObject = obj;
        }
    }

    public void OnUnregisterActionObject (GameObject obj )
    {
        if (actionObject == obj)
        {
            actionObject = null;
        }
    }

    // =================== 攻擊相關 ===================
    void OnAttack()
    {
        //OnAttackToward(transform.position + faceDir);
        if (currState == PC_STATE.NORMAL)
        {
            DoMeleeTo(transform.position + faceDir);
        }
    }

    void OnShoot()
    {
        //TODO: 用滑鼠或右類比決定方向
        Vector3 target = faceDir + gameObject.transform.position;
        if (currState == PC_STATE.NORMAL)
        {
            DoShootTo(target);
        }


    }

    void OnShootTo()
    {
        //print("OnShootTo");
        Vector2 mousePos = theInput.TheHero.MousePos.ReadValue<Vector2>();
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePos);
#if XZ_PLAN
        target.y = transform.position.y;
#else
    target.z = transform.position.z;
#endif
        //target.z = target.y;
        if (currState == PC_STATE.NORMAL)
        {
            DoShootTo(target);
        }
    }

    //舊的攻擊方式, 由外部呼叫往指定方向攻擊, 目前暫不使用
    public virtual void OnAttackToward(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            //TODO: 由滑鼠決定攻擊方向
            DoMeleeTo(target);
            DoShootTo(target);
        }
    }

    protected virtual void DoMeleeTo(Vector3 target)
    {
        //if (mp < MP_PerShoot)
        if (mp < meleeSkillDef.manaCost)
        {
            print("沒 Mana 呀 !!!!");
            return;
        }

        Vector3 td = target - gameObject.transform.position;
#if XZ_PLAN
        td.y = 0;
#else
        td.z = 0;
#endif
        td.Normalize();
        faceDir = td;
        SetupFrontDirection();
        if (myAnimator)
        {
            myAnimator.SetFloat("AttackX", td.x);
#if XZ_PLAN
            myAnimator.SetFloat("AttackY", td.z);
#else
            myAnimator.SetFloat("AttackY", td.y);
#endif
            myAnimator.SetTrigger("Attack");
        }

        if (meleeFX)
        {
            Vector3 fxPos = transform.position + faceFront;
            float fxAngle = 0;
            switch (faceFrontType)
            {
                case FaceFrontType.UP:
                    fxAngle = 0;
                    break;
                case FaceFrontType.RIGHT:
                    fxAngle = -90.0f;
                    break;
                case FaceFrontType.DOWN:
                    fxAngle = 180.0f;
                    break;
                case FaceFrontType.LEFT:
                    fxAngle = 90.0f;
                    break;
            }
#if XZ_PLAN
            Quaternion ro = Quaternion.Euler(90, -fxAngle, 0);
#else
            Quaternion ro = Quaternion.Euler(0, 0, fxAngle);
#endif
            GameObject fo = Instantiate(meleeFX, fxPos, ro, transform);
            if (faceFrontType == FaceFrontType.RIGHT)
            {
                //暴力法轉向
                fo.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }

        if (myDollManager)
            myDollManager.OnPlayerAttack(target);

        mp -= meleeSkillDef.manaCost;

        nextState = PC_STATE.ATTACK;
        attackWait = meleeSkillDef.duration;
    }

    void OnMeleeDamageBox(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight < 0.5f)
            return;

        //TODO: 攻擊範圍參數化
        float centerOffset = 1.0f;

        Vector2 vCenter = Vector2.zero;
        Vector2 vSize = Vector2.one * 3.5f;


        switch (faceFrontType)
        {
            case FaceFrontType.UP: //上
                vCenter.y = centerOffset;
                break;
            case FaceFrontType.DOWN: //下
                vCenter.y = -centerOffset;
                break;
            case FaceFrontType.RIGHT: //右
                vCenter.x = centerOffset;
                break;
            case FaceFrontType.LEFT: //左
                vCenter.x = -centerOffset;
                break;
        }

        //myDamage.damage = Attack;
        myDamage.damage = meleeSkillDef.baseDamage;     //TODO: 升級?
#if XZ_PLAN
        Collider[] cols = Physics.OverlapBox(transform.position + new Vector3(vCenter.x, 0, vCenter.y), new Vector3(vSize.x * 0.5f, 1.0f, vSize.y * 0.5f));
        foreach (Collider col in cols)
#else
        Collider2D[] cols = Physics2D.OverlapBoxAll((Vector2)transform.position + vCenter, vSize, 0);
        foreach (Collider2D col in cols)
#endif
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                HitStopper hs = GetComponent<HitStopper>();
                if (hs)
                {
                    hs.DoHitStop(0.05f);
                }
                if (meleeHitFX)
                {
                    Vector3 hitPos = col.ClosestPoint(transform.position);
                    hitPos = (hitPos + col.transform.position) * 0.5f;  //往受擊方的位置 Shift //暴力法
                                                                        //hitPos.z = col.transform.position.y - 0.125f;       //角色的話用對方的 Y 來調整
#if XZ_PLAN
                    Instantiate(meleeHitFX, hitPos, Quaternion.Euler(90, 0, 0), null);
#else
                    Instantiate(meleeHitFX, hitPos, Quaternion.identity, null);
#endif
                }
                col.gameObject.SendMessage("OnDamage", myDamage);
            }
        }
    }


    protected virtual void DoShootTo(Vector3 target)
    {
        //if (mp < MP_PerShoot)
        if (mp < rangeSkillDef.manaCost)  
        {
            print("沒 Mana 呀 !!!!");
            Instantiate(shootFX_2, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            return;
        }

        Vector3 td = target - gameObject.transform.position;
#if XZ_PLAN
        td.y = 0;
#else
        td.z = 0;
#endif
        td.Normalize();
        faceDir = td;
        SetupFrontDirection();

        //TODO 發射點靠前量參數化?
        Vector3 shootPos = gameObject.transform.position + faceDir * 0.25f;

        Quaternion ro;
#if XZ_PLAN
        ro = Quaternion.Euler(90, 0, 0);
#else
        ro = Quaternion.identity;
#endif

        GameObject newObj = Instantiate(bulletRef, shootPos, ro, null);
        if (newObj)
        {
            bullet newBullet = newObj.GetComponent<bullet>();
            if (newBullet)
            {
                //newBullet.SetGroup(DAMAGE_GROUP.PLAYER);
                //newBullet.targetDir = td;
                //傷害值，由自己來給
                //newBullet.phyDamage = rangeSkillDef.baseDamage; //TODO:  升級?
                newBullet.InitValue(DAMAGE_GROUP.PLAYER, rangeSkillDef.baseDamage, td);
            }
        }

        Instantiate(shootFX_1, gameObject.transform.position, ro, gameObject.transform);
        Instantiate(shootFX_2, gameObject.transform.position, ro, gameObject.transform);

        if (myAnimator)
        {
            myAnimator.SetFloat("CastX", td.x);
#if XZ_PLAN
            myAnimator.SetFloat("CastY", td.z);
#else
            myAnimator.SetFloat("CastY", td.y);
#endif
            myAnimator.SetTrigger("Cast");
        }

        //mp -= MP_PerShoot;
        mp -= rangeSkillDef.manaCost;

        if (myDollManager)
            myDollManager.OnPlayerShoot(target);

        attackWait = rangeSkillDef.duration;
        nextState = PC_STATE.ATTACK;
    }



    void OnDamage(Damage theDamage)
    {
#if XZ_PLAN
        if (beenHitFX)
            Instantiate(beenHitFX, transform.position, Quaternion.Euler(90, 0, 0), null);
#else
        if (beenHitFX)
            Instantiate(beenHitFX, transform.position, Quaternion.identity, null);
#endif

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

    public void ForceStop( bool stop = true)
    {
        if ( stop && currState != PC_STATE.DEAD && currState!= PC_STATE.NONE ) 
        {
            nextState = PC_STATE.STOP;
            if (myAnimator)
                myAnimator.SetBool("Run", false);
        }
        else if ( !stop && currState == PC_STATE.STOP )
        {
            nextState = PC_STATE.NORMAL;
        }
    }
}
