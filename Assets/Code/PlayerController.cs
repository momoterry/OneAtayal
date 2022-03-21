using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    public float AttackCD = 1.0f;
    public float WalkSpeed = 8.0f;
    public GameObject beenHitFX;

    public GameObject bulletRef;

    public GameObject shootFX_1;
    public GameObject shootFX_2;
    public GameObject meleeFX;
    public GameObject meleeHitFX;

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

    //Input
    MyInputActions theInput;

    //移動和面向
    protected Vector3 faceDir;

    // 近戰用四面向
    public enum FaceFrontType
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }
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

    public float GetHPMax() { return HP_Max; }
    public float GetMPMax() { return MP_Max; }
    public float GetHP() { return hp; }
    public float GetMP() { return mp; }
    public float GetATTACK() { return Attack; }

    //Doll 相關
    public DollManager myDollManager;
    public DollManager GetDollManager() { return myDollManager; }

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

        myAnimator = GetComponent<Animator>();

        InitStatus();

        //Input System Bind
        theInput.TheHero.Attack.performed += ctx => OnAttack();
        theInput.TheHero.Shoot.performed += ctx => OnShoot();
        theInput.TheHero.Action.performed += ctx => OnActionKey();
        theInput.TheHero.ShootTo.performed += ctx => OnShootTo();
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
        faceDir = Vector3.down;
        faceFront = Vector3.down;
        faceFrontType = FaceFrontType.DOWN;
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


    protected virtual void UpdateAttack() {}

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
            //faceDir.Normalize();

            SetupFrontDirection();
        }

        //if (myAgent.velocity.magnitude > 0.5f)
        if (myAnimator)
        {
            myAnimator.SetBool("Run", (myAgent.velocity.magnitude > 0.1f)||bMove);
            //myAnimator.SetFloat("X", faceDir.x);
            //myAnimator.SetFloat("Y", faceDir.y);
            myAnimator.SetFloat("X", faceFront.x);
            myAnimator.SetFloat("Y", faceFront.y);
        }
    }

    private void SetupFrontDirection()
    {
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
        target.z = target.y;
        if (currState == PC_STATE.NORMAL)
        {
            DoShootTo(target);
        }
    }

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
        Vector3 td = target - gameObject.transform.position;
        td.z = 0;
        td.Normalize();
        faceDir = td;
        SetupFrontDirection();

        if (myAnimator)
        {
            myAnimator.SetFloat("AttackX", td.x);
            myAnimator.SetFloat("AttackY", td.y);
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
            Quaternion ro = Quaternion.Euler(0, 0, fxAngle);
            GameObject fo = Instantiate(meleeFX, fxPos, ro, transform);
            if (faceFrontType == FaceFrontType.RIGHT)
            {
                //暴力法轉向
                fo.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
        }

        if (myDollManager)
            myDollManager.OnPlayerAttack(target);
    }

    void OnMeleeDamageBox(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight < 0.5f)
            return;

        //TODO: 攻擊範圍參數化
        float centerOffset = 1.0f;
        Vector2 vCenter = Vector2.zero;
        Vector2 vSize = Vector2.one * 3.0f;

        switch (faceFrontType)
        {
            case FaceFrontType.UP: //上
                vCenter.y = centerOffset;
                break;
            case FaceFrontType.RIGHT: //右
                vCenter.x = centerOffset;
                break;
            case FaceFrontType.DOWN: //下
                vCenter.y = -centerOffset;
                break;
            case FaceFrontType.LEFT: //左
                vCenter.x = -centerOffset;
                break;
        }

        Collider2D[] cols = Physics2D.OverlapBoxAll((Vector2)transform.position + vCenter, vSize, 0);

        myDamage.damage = Attack;
        foreach (Collider2D col in cols)
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
                    hitPos.z = col.transform.position.y - 0.125f;       //角色的話用對方的 Y 來調整
                    Instantiate(meleeHitFX, hitPos, Quaternion.identity, null); ;
                }
                col.gameObject.SendMessage("OnDamage", myDamage);
            }
        }
    }


    protected virtual void DoShootTo(Vector3 target)
    {
        if (mp < MP_PerShoot)
        {
            print("沒 Mana 呀 !!!!");
            Instantiate(shootFX_2, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            return;
        }

        Vector3 td = target - gameObject.transform.position;
        td.z = 0;
        td.Normalize();
        faceDir = td;
        SetupFrontDirection();

        //TODO 發射點靠前量參數化?
        Vector3 shootPos = gameObject.transform.position + faceDir * 0.25f;

        GameObject newObj = Instantiate(bulletRef, shootPos, Quaternion.identity, null);
        if (newObj)
        {
            bullet newBullet = newObj.GetComponent<bullet>();
            if (newBullet)
            {
                newBullet.SetGroup(DAMAGE_GROUP.PLAYER);
                newBullet.targetDir = td;
                //傷害值，由自己來給
                newBullet.phyDamage = Attack;
            }
        }

        Instantiate(shootFX_1, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        Instantiate(shootFX_2, gameObject.transform.position, Quaternion.identity, gameObject.transform);

        if (myAnimator)
        {
            myAnimator.SetFloat("CastX", td.x);
            myAnimator.SetFloat("CastY", td.y);
            myAnimator.SetTrigger("Cast");
        }
        //faceX = td.x;
        //faceY = td.y;

        mp -= MP_PerShoot;

        if (myDollManager)
            myDollManager.OnPlayerShoot(target);
    }



    void OnDamage(Damage theDamage)
    {
        if (beenHitFX)
            Instantiate(beenHitFX, transform.position, Quaternion.identity, null);

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
