using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PC_One : PlayerControllerBase
{
    [System.Serializable]
    public class SkillInfo
    {
        public GameObject bulletRef;
        public float bulletInitDis;
        public float damageRatio = 1.0f;
        public float duration = 0.2f;   //技能施放期間 (無法操作 )
        public float manaCost = 0;
    }

    public SkillBase autoSkillRef;

    public SkillInfo autoAttackInfo;
    public float autoAttackRange = 8.0f;
    public float autoAttackWait = 0.2f;
    public float autoAttackCD = 1.0f;

    public float WalkSpeed = 8.0f;


    public float HP_MaxInit = 100.0f;
    public float MP_MaxInit = 100.0f;
    public float Attack_Init = 50.0f;

    public float MP_Gen_Rate = 30.0f;

    protected NavMeshAgent myAgent;
    protected float skillTime = 0.0f;
    protected float autoAttackCDLeft = 0.0f;
    protected SkillBase autoSkill;

    //Input
    protected MyInputActions theInput;

    //移動和面向
    protected Vector3 faceDir;

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

    public enum PC_STATE
    {
        NONE,
        NORMAL,
        ATTACK_AUTO,    //自動普攻狀態
        SKILL,          //攻擊或技能後硬直狀態 (不接受輸入)
        STOP,           //For 外部強迫暫停
        DEAD,
    }
    protected PC_STATE currState = PC_STATE.NONE;
    protected PC_STATE nextState = PC_STATE.NONE;
    // Start is called before the first frame update
    protected void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        if (myAgent)
        {
            myAgent.updateRotation = false;
            myAgent.updateUpAxis = false;
        }

        myAnimator = GetComponent<Animator>();

        SetupFaceDirByAngle(initFaceDirAngle);

        InitStatus();

        //TODO: 應該交給 Battle System
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

        //產生各 SkillBase
        if (autoSkillRef)
        {
            autoSkill = Instantiate(autoSkillRef, transform);
            if (autoSkill)
            {
                autoSkill.InitCasterInfo(gameObject);
            }
        }

    }

    public override void SetInputActive(bool enable)
    {
        if (enable)
        {
            theInput.Enable();
        }
        else
        {
            theInput.Disable();
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
    public override void InitStatus()
    {
        //print("PlayerController::InitStatus");
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

    protected void SetupFaceDirByAngle(float angle)
    {
        //faceDir = Vector3.RotateTowards(Vector3.forward, Vector3.right, angle * Mathf.Deg2Rad, 0);
        faceDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        SetupFrontDirection();
    }

    public override void SetupFaceDir(Vector3 dir)
    {
        faceDir = dir;
        SetupFrontDirection();
    }

    public override void DoTeleport(Vector3 position, float faceAngle)
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

    public override bool DoHpUp()
    {
        if (HP_Up == HP_UP_Max)
            return false;

        float oldValue = HP_Max;
        HP_Up++;
        HP_Max = HP_MaxInit * (1.0f + HP_Up_Ratio * (float)HP_Up);
        hp *= HP_Max / oldValue; //現有 hp 等比例增加

        return true;
    }

    public override bool DoAtkUp()
    {
        if (ATK_Up == ATK_UP_MAX)
            return false;

        ATK_Up++;
        Attack = Attack_Init * (1.0f + ATK_Up_Ratio * (float)ATK_Up);
        return true;
    }

    public override bool IsKilled()
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

        if (myDollManager && currState != PC_STATE.NONE && currState != PC_STATE.DEAD)
        {
            UpdateStatus();
            myDollManager.SetMasterPosition(transform.position);
            myDollManager.SetMasterDirection(faceDir, faceFrontType);
        }

        if (myHPHandler && currState != PC_STATE.NONE)
        {
            myHPHandler.SetHP(hp, HP_Max);
            myHPHandler.SetMP(mp, MP_Max);
        }    

        if (currState == PC_STATE.NORMAL || currState == PC_STATE.ATTACK_AUTO || currState == PC_STATE.SKILL)
        {
            if (autoAttackCDLeft > 0)
            {
                autoAttackCDLeft -= Time.deltaTime;
            }
            else
                autoAttackCDLeft = 0;
        }
    }

    protected virtual void OnUpdateState()
    {
        switch (currState)
        {
            case PC_STATE.NORMAL:
                UpdateMoveControl();
                break;
            case PC_STATE.ATTACK_AUTO:
                UpdateAttackAuto();
                UpdateMoveControl();
                break;
            case PC_STATE.SKILL:
                UpdateSkill();
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

    protected virtual void UpdateAttackAuto()
    {
        if (autoAttackCDLeft > 0)
        {
            autoAttackCDLeft -= Time.deltaTime;
        }
        
        if (autoAttackCDLeft <=0)
        {
            if (autoSkill)
            {
                if (DoStartSkill(autoSkill))
                {
                    autoAttackCDLeft = autoAttackCD;
                }
                else
                {
                    autoAttackCDLeft = 0.1f;
                }
            }

            //GameObject o = FindBestShootTarget(autoAttackRange);
            //if (o)
            //{
            //    DoStartSkill(autoAttackInfo, o);
            //    autoAttackCDLeft = autoAttackCD;
            //}
            //else
            //{
            //    autoAttackCDLeft = 0.1f;    //重找目標時間, TODO: 參數化?
            //}
        }
    }

    protected virtual void UpdateSkill()
    {
        skillTime -= Time.deltaTime;
        if (skillTime <= 0)
        {
            nextState = PC_STATE.ATTACK_AUTO;
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

        //觸控介面
        Vector2 vPadVec = Vector2.zero;
        if (BattleSystem.GetInstance().GetVPad())
        {
            vPadVec = BattleSystem.GetInstance().GetVPad().GetCurrVector();
        }

        if (vPadVec.magnitude > 0.2f)
        {
            bMove = true;
#if XZ_PLAN
            moveVec = new Vector3(vPadVec.x, 0, vPadVec.y);   //XZ Plan
            moveVec.Normalize();
#else
            moveVec = vPadVec;   //XY Plan
            moveVec.Normalize();
            moveVec.z = moveVec.y * 0.01f;
#endif
        }


        // 鍵鼠和手把
        if (inputVec.magnitude > 0.5f)
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

            if (currState == PC_STATE.ATTACK_AUTO)
            {
                nextState = PC_STATE.NORMAL;
            }
        }
        else
        {
            if (currState == PC_STATE.NORMAL)
            {
                nextState = PC_STATE.ATTACK_AUTO;
            }
        }


        if (myAnimator)
        {
            myAnimator.SetBool("Run", (myAgent.velocity.magnitude > 0.1f) || bMove);
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




    public override void OnMoveToPosition(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            myAgent.SetDestination(target);
        }
    }

    //=================== 互動物件相關 ===================
    public override void OnActionKey()
    {
        if (actionObject)
        {
            actionObject.SendMessage("OnAction");

            actionObject = null;
            if (BattleSystem.GetInstance().GetVPad())
                BattleSystem.GetInstance().GetVPad().OnActionOff();
        }
    }

    public override bool OnRegisterActionObject(GameObject obj)
    {
        if (actionObject == null)
        {
            actionObject = obj;
            if (BattleSystem.GetInstance().GetVPad())
                BattleSystem.GetInstance().GetVPad().OnActionOn();
            return true;
        }
        return false;
    }

    public override bool OnUnregisterActionObject(GameObject obj)
    {
        if (actionObject == obj)
        {
            actionObject = null;
            if (BattleSystem.GetInstance().GetVPad())
                BattleSystem.GetInstance().GetVPad().OnActionOff();
            return true;
        }
        return false;
    }

    // =================== 攻擊相關 ===================

    virtual protected GameObject FindBestShootTarget(float searchRange = 10.0f)
    {
        //float searchRange = 10.0f;

        Collider[] cols = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));

        GameObject bestEnemy = null;
        float bestSDis = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            //print("I Found: " + col.gameObject.name);
            if (col.gameObject.CompareTag("Enemy"))
            {
                Vector3 vDis = col.transform.position - transform.position;
                //float angle = Vector3.Angle(faceFront, vDis);
                //if (angle > searchAngle)
                //    continue;
                float sDis = vDis.sqrMagnitude;
                if (sDis < bestSDis)
                {
                    bestEnemy = col.gameObject;
                    bestSDis = sDis;
                }
            }
        }

        return bestEnemy;
    }

    // 以下兩種方式二擇一
    protected virtual bool DoStartSkill( SkillBase theSkill)
    {
        if (theSkill.DoStart())
        {
            if (theSkill.duration > 0)
            {
                skillTime = autoSkill.duration;
                nextState = PC_STATE.SKILL;
            }
            else
                nextState = PC_STATE.ATTACK_AUTO;

            return true;
        }

        return false;
    }

    protected virtual void DoStartSkill(SkillInfo skillInfo, GameObject target)
    {
        if (mp < skillInfo.manaCost)
        {
            print("沒 Mana 呀 !!!!");
            return;
        }

        Vector3 td = target.transform.position - gameObject.transform.position;
#if XZ_PLAN
        td.y = 0;
#else
        td.z = 0;
#endif
        td.Normalize();
        faceDir = td;
        SetupFrontDirection();

        //TODO 發射點靠前量參數化?
        Vector3 shootPos = gameObject.transform.position + td * skillInfo.bulletInitDis;

        GameObject newObj = BattleSystem.GetInstance().SpawnGameplayObject(skillInfo.bulletRef, shootPos, false);
        if (newObj)
        {
            bullet newBullet = newObj.GetComponent<bullet>();
            if (newBullet)
            {
                newBullet.InitValue(DAMAGE_GROUP.PLAYER, Attack * skillInfo.damageRatio, td);
            }
        }

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


        mp -= skillInfo.manaCost;

    }


    public override void OnAttack()
    {

    }

    public override void OnShoot()
    {

    }

    public override void OnShootTo()
    {}

    //舊的攻擊方式, 由外部呼叫往指定方向攻擊, 目前暫不使用
    public override void OnAttackTo(Vector3 target)
    {}

    public override void DoShootTo(Vector3 target)
    {}


    virtual protected void DoDeath()
    {
        hp = 0;
        nextState = PC_STATE.DEAD;
        if (myDollManager)
            myDollManager.OnPlayerDead();
        BattleSystem.GetInstance().OnPlayerKilled();
    }


    void OnDamage(Damage theDamage)
    {

        hp -= theDamage.damage;
        if (hp <= 0)
        {
            DoDeath();
        }
    }

    public override void DoHeal(float healNum)
    {
        hp += healNum;
        if (hp > HP_Max)
            hp = HP_Max;
    }

    public override void ForceStop(bool stop = true)
    {
        if (stop && currState != PC_STATE.DEAD && currState != PC_STATE.NONE)
        {
            nextState = PC_STATE.STOP;
            if (myAnimator)
                myAnimator.SetBool("Run", false);
        }
        else if (!stop && currState == PC_STATE.STOP)
        {
            nextState = PC_STATE.NORMAL;
        }
    }


    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + faceDir);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currState.ToString());
    //}
}
