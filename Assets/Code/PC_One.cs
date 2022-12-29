using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PC_One : PlayerControllerBase
{
    protected const string AUTO_SKILL = "AutoSkill";
    protected const string SKILL_ONE =  "SkillOne";
    protected const string SKILL_TWO = "SkillTwo";
    protected const string SKILL_THREE = "SkillThree";
    protected const string SKILL_FOUR = "SkillFour";
    protected string[] skillSaveNames = { SKILL_ONE, SKILL_TWO, SKILL_THREE, SKILL_FOUR };

    public Animator myAnimator;
    public SPAnimator mySPAnimator;

    public SkillBase autoSkillRef;
    public SkillBase[] activeSkillRefs;

    //public float autoAttackRange = 8.0f;
    public float autoAttackWait = 0.2f;
    //public float autoAttackCD = 1.0f;       //TODO: 由 Skill 決定

    public float WalkSpeed = 8.0f;


    public float HP_MaxInit = 100.0f;
    public float MP_MaxInit = 100.0f;
    public float Attack_Init = 50.0f;

    public float MP_Gen_Rate = 30.0f;

    public Talk theTalk;

    protected NavMeshAgent myAgent;
    protected float skillTime = 0.0f;
    protected float autoAttackCDLeft = 0.0f;
    protected SkillBase autoSkill;
    protected SkillBase[] activeSkillls;

    //Input
    protected MyInputActions theInput;
    protected bool inputActive = false;

    //移動和面向
    protected Vector3 faceDir;

    protected Vector3 faceFront;
    protected FaceFrontType faceFrontType;

    //protected Animator myAnimator;

    //For Touch Control
    protected bool isMovingByTouchControl = false;
    protected Vector3 moveTargetPos;

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
        ATTACK,         //自動普攻後的硬直 (不能移動, 可放技能)
        SKILL,          //攻擊或技能後硬直狀態 (不接受輸入和技能)
        STOP,           //For 外部強迫暫停
        DEAD,
    }
    protected PC_STATE currState = PC_STATE.NONE;
    protected PC_STATE nextState = PC_STATE.NONE;

    public override Vector3 GetFaceDir()
    {
        return faceDir;
    }

    public override FaceFrontType GetFaceFront()
    {
        return faceFrontType;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        if (myAgent)
        {
            myAgent.updateRotation = false;
            myAgent.updateUpAxis = false;
        }

        if (!myAnimator)
            myAnimator = GetComponent<Animator>();

        SetupFaceDirByAngle(initFaceDirAngle);

        InitStatus();

        //TODO: 應該交給 Battle System
        //Input System Bind
        theInput.TheHero.Attack.performed += ctx => OnAttack();
        theInput.TheHero.Shoot.performed += ctx => OnShoot();
        theInput.TheHero.Action.performed += ctx => OnActionKey();
        theInput.TheHero.ShootTo.performed += ctx => OnShootTo();

        inputActive = true;

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

        //先檢查存檔
        SkillBase savedAutoSkillRef = GameSystem.GetInstance().GetPlayerSkillRef(AUTO_SKILL);
        if (savedAutoSkillRef)
            print("技能存檔 !! " + savedAutoSkillRef);
        if (savedAutoSkillRef)
        {
            autoSkillRef = savedAutoSkillRef;
        }

        int activeSkillSaveMax = Mathf.Min(activeSkillRefs.Length, skillSaveNames.Length);
        //if (activeSkillRefs.Length >= 1 && activeSkillRefs.Length <= skillSaveNames.Length)
        //{
        for (int i = 0; i < activeSkillSaveMax; i++)
        {
            SkillBase savedActiveSkillRef = GameSystem.GetInstance().GetPlayerSkillRef(skillSaveNames[i]);
            if (savedActiveSkillRef)
                print("主動技能存檔 !! " + i + " : " + savedActiveSkillRef);
            if (savedActiveSkillRef)
            {
                activeSkillRefs[i] = savedActiveSkillRef;
            }
        }
        //}

        //產生各 SkillBase
        DoSetAutoSkill(autoSkillRef);

        if (activeSkillRefs.Length > 0)
        {
            activeSkillls = new SkillBase[activeSkillRefs.Length];
            for (int i = 0; i < activeSkillRefs.Length; i++)
            {
                //activeSkillls[i] = Instantiate(activeSkillRefs[i], transform);
                //activeSkillls[i].InitCasterInfo(gameObject);
                DoSetActiveSkill(activeSkillRefs[i], i);
            }
        }
    }

    protected void DoSetActiveSkill( SkillBase skillRef, int index)
    {
        if (index < 0 || index >= activeSkillls.Length)
            return;

        if (activeSkillls[index])
        {
            Destroy(activeSkillls[index]);
            activeSkillls[index] = null;
        }

        SkillButton sb = BattleSystem.GetInstance().theBattleHUD.GetSkillButton(index);
        if (skillRef)
        {
            activeSkillls[index] = Instantiate(skillRef, transform);
            activeSkillls[index].InitCasterInfo(gameObject);
            //BattleSystem.GetInstance().theBattleHUD.SetSkillIcon(activeSkillls[index].icon, index + 1);
            //SkillButton sb = BattleSystem.GetInstance().theBattleHUD.GetSkillButton(index + 1);
            activeSkillls[index].InitButton(sb);
            activeSkillls[index].SetSkillIndex(index);
        }
        else
        {
            //BattleSystem.GetInstance().theBattleHUD.SetSkillIcon(null, index + 1);
            if (sb)
            {
                sb.SetIcon(null);
            }
        }
    }

    protected void DoSetAutoSkill( SkillBase skillRef)
    {
        //print("DoSetAutoSkill!! " + skillRef);
        if (autoSkill)
        {
            Destroy(autoSkill.gameObject);
            autoSkill = null;
        }

        SkillButton sb = BattleSystem.GetInstance().theBattleHUD.GetAutoAttackButton();
        if (skillRef)
        {
            autoSkill = Instantiate(skillRef, transform);
            autoSkill.InitCasterInfo(gameObject);
            //BattleSystem.GetInstance().theBattleHUD.SetSkillIcon(autoSkill.icon, 0);
            //SkillButton sb = BattleSystem.GetInstance().theBattleHUD.GetSkillButton(0);
            autoSkill.InitButton(sb);
        }
        else
        {
            //BattleSystem.GetInstance().theBattleHUD.SetSkillIcon(null, 0);
            if (sb)
            {
                sb.SetIcon(null);
            }
        }
    }

    public void SetAutoSkill( SkillBase skillRef )
    {
        GameSystem.GetInstance().SetPlayerSkillRef(AUTO_SKILL, skillRef);
        DoSetAutoSkill(skillRef);
    }

    public void SetActiveSkill( SkillBase activeSkillRef, int index = 0 )
    {
        GameSystem.GetInstance().SetPlayerSkillRef(skillSaveNames[index], activeSkillRef);
        DoSetActiveSkill(activeSkillRef, index);
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
        inputActive = enable;
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

    public override void SetupFaceDirByAngle(float angle)
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
            case PC_STATE.ATTACK:
                UpdateAttack();
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
                if (DoAutoSkill(autoSkill))
                {
                    //autoAttackCDLeft = autoAttackCD;
                    autoAttackCDLeft = autoSkill.coolDown;
                }
                else
                {
                    autoAttackCDLeft = 0.1f;
                }
            }
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

    protected virtual void UpdateAttack()
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

        //Touch Control
        if ( !GameSystem.IsUseVpad() && isMovingByTouchControl)
        {
            Vector3 targetVec = moveTargetPos - transform.position;
            targetVec.y = 0;
            if (targetVec.sqrMagnitude > 0.25f)
            {
                bMove = true;
                moveVec = targetVec.normalized;
                //print("Move !! " + moveVec);
            }
            //else
            //{
            //    print("Too Small...........");
            //}
            isMovingByTouchControl = false;
        }

        //VPad 觸控介面
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

        bool isRun = (myAgent.velocity.magnitude > 0.1f) || bMove;
        if (myAnimator)
        {
            myAnimator.SetBool("Run", isRun);
        }
        if (mySPAnimator)
        {
            mySPAnimator.SetIsRun(isRun);
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
            myAnimator.SetFloat("X", faceDir.x);
            myAnimator.SetFloat("Y", faceDir.z);
        }
        if (mySPAnimator)
        {
            mySPAnimator.SetXY(faceDir.x, faceDir.z);
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
            myAnimator.SetFloat("X", faceDir.x);
            myAnimator.SetFloat("Y", faceDir.z);
        }
#endif

    }




    public override void OnMoveToPosition(Vector3 target)
    {
        //print("OnMoveToPosition!! " + currState);
        if (inputActive && (currState == PC_STATE.NORMAL || currState == PC_STATE.ATTACK_AUTO))
        {
            //myAgent.SetDestination(target);
            isMovingByTouchControl = true;
            moveTargetPos = target;
        }
    }

    //=================== 互動物件相關 ===================
    public override void OnActionKey()
    {
        if (actionObject)
        {
            actionObject.SendMessage("OnAction");

            //Terry: 交給 actionObject 來決定是否能繼續接受 Action
            //actionObject = null;
            //if (GameSystem.IsUseVpad() && BattleSystem.GetInstance().GetVPad())
            //    BattleSystem.GetInstance().GetVPad().OnActionOff();
        }
    }

    public override bool OnRegisterActionObject(GameObject obj)
    {
        if (actionObject == null)
        {
            actionObject = obj;
            if (GameSystem.IsUseVpad() && BattleSystem.GetInstance().GetVPad())
                BattleSystem.GetInstance().GetVPad().OnActionOn();
            return true;
        }
        return false;
    }

    public override bool OnUnregisterActionObject(GameObject obj)
    {
        if (actionObject == obj || actionObject == null)
        {
            actionObject = null;
            if (GameSystem.IsUseVpad() && BattleSystem.GetInstance().GetVPad())
                BattleSystem.GetInstance().GetVPad().OnActionOff();
            return true;
        }

        return false;
    }

    public override void SaySomthing(string str)
    {
        if (theTalk)
        {
            theTalk.AddSentence(str);
        }
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
    protected virtual bool DoAutoSkill( SkillBase theSkill)
    {
        if (theSkill.DoStart())
        {
            if (theSkill.duration > 0)
            {
                skillTime = autoSkill.duration;
                nextState = PC_STATE.ATTACK;
            }
            else
                nextState = PC_STATE.ATTACK_AUTO;

            return true;
        }

        return false;
    }


    public override void OnSkill(int index)
    {
        if (activeSkillls.Length <= index)
            return;

        if (currState!= PC_STATE.NORMAL && currState != PC_STATE.ATTACK_AUTO && currState != PC_STATE.ATTACK)
            return;

        SkillBase theSkill = activeSkillls[index];

        //TEST
        //string skillTalk = "打給你死喔!!" + Random.Range(11, 99).ToString();
        //theTalk.AddSentence(skillTalk);

        SKILL_RESULT skillResult = SKILL_RESULT.SUCCESS;
        if (theSkill.DoStart(ref skillResult))
        {
            nextState = PC_STATE.SKILL;
            skillTime = theSkill.duration;
        }
        else
        {
            if (skillResult == SKILL_RESULT.NO_MANA)
            {
                if (theTalk)
                {
                    theTalk.AddSentence("Mana 不夠啦 !!");
                }
            }
        }

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

    public override void OnKillEnemy(Enemy e) 
    {
        if (theCharData)
        {
            theCharData.OnKillEnemy(e);
        }
    }

    public override void DoHeal(float healNum)
    {
        hp += healNum;
        if (hp >= HP_Max)
            hp = HP_Max;
    }

    public override float DoHeal(float healAbsoluteNum, float healRatio)
    {
        float newHp = hp + healAbsoluteNum + HP_Max * healRatio;
        if (newHp >= HP_Max)
            newHp = HP_Max;
        float healResult = newHp - hp;
        hp = newHp;
        return healResult;
    }


    public override void ForceStop(bool stop = true)
    {
        if (stop && currState != PC_STATE.DEAD && currState != PC_STATE.NONE)
        {
            nextState = PC_STATE.STOP;
            if (myAnimator)
                myAnimator.SetBool("Run", false);
            if (mySPAnimator)
            {
                mySPAnimator.SetIsRun(false);
            }
        }
        else if (!stop && currState == PC_STATE.STOP)
        {
            nextState = PC_STATE.NORMAL;
        }
    }

    // =================== Doll 指揮相關 ===================

    public void ReviveOneDoll()
    {
        List<Doll> dollList = myDollManager.GetDolls();
        foreach( Doll d in dollList)
        {
            if (d.gameObject.activeInHierarchy)
            {
                continue;
            }
            //print("Found One !!");
            //d.gameObject.SendMessage("OnRevive");
            DollAuto da = d.gameObject.GetComponent<DollAuto>();
            if (da)
            {
                //print("Do OnRevive");
                da.OnRevive();
                break;
            }

        }
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + faceDir);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currState.ToString());
    //}
}
