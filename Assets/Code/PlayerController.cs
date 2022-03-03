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

    //���ʩM���V
    protected float faceX = 0.0f;
    protected float faceY = -1.0f;
    protected Animator myAnimator;

    //���ʪ���
    protected GameObject actionObject = null;

    //�����ˮ`����
    protected Damage myDamage;

    //�ɯŬ���
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

    //Doll ����
    public DollManager myDollManager;
    public DollManager GetDollManager() { return myDollManager; }

    public enum PC_STATE
    {
        NONE,
        NORMAL,
        ATTACK, //For �ʧ@
        DEAD,
    }
    protected PC_STATE currState = PC_STATE.NONE;
    protected PC_STATE nextState = PC_STATE.NONE;
    // Start is called before the first frame update
    protected void Start()
    {
        //print("�ڲש�}�l�g Code !!");
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.updateRotation = false;
        myAgent.updateUpAxis = false;

        myAnimator = GetComponent<Animator>();

        InitStatus();

        //Input System Bind
        theInput.TheHero.Attack.performed += ctx => OnAttack();
        theInput.TheHero.Shoot.performed += ctx => OnShoot();
        theInput.TheHero.Action.performed += ctx => OnActionKey();
    }


    private void Awake()
    {
        theInput = new MyInputActions();
        theInput.Enable();
    }

    //��l�ƨ쵥�Ť@�����A
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

        //faceAngle = 180.0f; //TODO: ���ө�b�O���a��
        faceX = 0.0f;
        faceY = -1.0f;
}

    public virtual bool DoHpUp()
    {
        if (HP_Up == HP_UP_Max)
            return false;

        float oldValue = HP_Max;
        HP_Up++;
        HP_Max = HP_MaxInit * (1.0f + HP_Up_Ratio * (float)HP_Up);
        hp *= HP_Max / oldValue; //�{�� hp ����ҼW�[

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
        //TODO: ���J�浹�O���t��

        float minMove = 0.5f;
        Vector3 moveVec = Vector3.zero;
        bool bMove = false;

        Vector2 inputVec = theInput.TheHero.Move.ReadValue<Vector2>();

        if (inputVec.magnitude > 0.5)
        {
            bMove = true;
            moveVec = inputVec;
            moveVec = moveVec.normalized * minMove + Vector3.right * 0.001f;
        }


        //if (Input.GetKey("w"))
        //{
        //    print("W");
        //    moveVec += Vector3.up * minMove;
        //    bMove = true;

        //    moveVec += Vector3.right * 0.001f;   //�����D������A���W���U�����ʷ|�����D (�h�b�� Nav2D ����)

        //}
        //else if (Input.GetKey("s"))
        //{
        //    moveVec -= Vector3.up * minMove;
        //    bMove = true;

        //    moveVec += Vector3.right * 0.001f;   //�����D������A���W���U�����ʷ|�����D (�h�b�� Nav2D ����)
        //}

        //if (Input.GetKey("a"))
        //{
        //    moveVec -= Vector3.right * minMove;
        //    bMove = true;
        //}
        //else if (Input.GetKey("d"))
        //{
        //    moveVec += Vector3.right * minMove;
        //    bMove = true;
        //}

        //Check GamePad
        //if (Input.)

        if (bMove)
        {
            //TODO: ���n�C Frame �i��
            OnMoveToPosition(transform.position + moveVec);

            faceX = moveVec.x;
            faceY = moveVec.y;

        }

        //if (myAgent.velocity.magnitude > 0.5f)
        if (myAnimator)
        {
            myAnimator.SetBool("Run", (myAgent.velocity.magnitude > 0.1f)||bMove);
            //myAnimator.SetBool("Run", bMove);
            myAnimator.SetFloat("X", faceX);
            myAnimator.SetFloat("Y", faceY);
        }
    }

    public virtual void OnMoveToPosition(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            myAgent.SetDestination(target);
        }
    }

    //=================== ���ʪ������ ===================
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

    // =================== �������� ===================
    void OnAttack()
    {
        Vector3 faceTo = new Vector3(faceX, faceY, 0);
        OnAttackToward(transform.position + faceTo);

    }
    
    void OnShoot()
    {
        Vector3 target = new Vector3(faceX, faceY, 0) + gameObject.transform.position;
        if (currState == PC_STATE.NORMAL)
        {
            DoShootTo(target);
        }
    }

    public virtual void OnAttackToward(Vector3 target)
    {
        if (currState == PC_STATE.NORMAL)
        {
            DoMeleeTo(target);
        }
    }

    protected virtual void DoMeleeTo(Vector3 target)
    {
        Vector3 td = target - gameObject.transform.position;
        td.z = 0;
        td.Normalize();

        if (myAnimator)
        {
            myAnimator.SetFloat("AttackX", td.x);
            myAnimator.SetFloat("AttackY", td.y);
            myAnimator.SetTrigger("Attack");
        }
        faceX = td.x;
        faceY = td.y;

        if (myDollManager)
            myDollManager.OnPlayerAttack(target);
    }

    void OnMeleeDamageBox(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight < 0.5f)
            return;

        //TODO: �����d��ѼƤ�
        float centerOffset = 1.0f;
        Vector2 vCenter = Vector2.zero;
        Vector2 vSize = Vector2.one * 1.5f;

        switch (evt.intParameter)
        {
            case 0: //�W
                vCenter.y = centerOffset;
                break;
            case 1: //�k
                vCenter.x = centerOffset;
                break;
            case 2: //�U
                vCenter.y = -centerOffset;
                break;
            case 3: //��
                vCenter.x = -centerOffset;
                break;
        }

        Collider2D[] cols = Physics2D.OverlapBoxAll((Vector2)transform.position + vCenter, vSize, 0);

        //if (attackFX)
        //    Instantiate(attackFX, transform.position, Quaternion.identity, null);



        myDamage.damage = Attack;
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                if (meleeHitFX)
                {
                    Instantiate(meleeHitFX, col.ClosestPoint(transform.position), Quaternion.identity, null); ;
                }
                col.gameObject.SendMessage("DoDamage", myDamage);
            }
        }
    }

    
    protected virtual void DoShootTo(Vector3 target)
    {
        if (mp < MP_PerShoot)
        {
            print("�S Mana �r !!!!");
            Instantiate(shootFX_2, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            return;
        }

        Vector3 td = target - gameObject.transform.position;
        td.z = 0;
        td.Normalize();
        GameObject newObj = Instantiate(bulletRef, gameObject.transform.position, Quaternion.identity, null);
        if (newObj)
        {
            bullet newBullet = newObj.GetComponent<bullet>();
            if (newBullet)
            {
                newBullet.SetGroup(DAMAGE_GROUP.PLAYER);
                newBullet.targetDir = td;
                //�ˮ`�ȡA�Ѧۤv�ӵ�
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
        faceX = td.x;
        faceY = td.y;

        mp -= MP_PerShoot;

        if (myDollManager)
            myDollManager.OnPlayerShoot(target);
    }



    void DoDamage(Damage theDamage)
    {
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
