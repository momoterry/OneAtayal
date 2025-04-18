using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===================================================
//
// 物件化的 Skill 系統，可支援 Player ，也可支援 EnemyBeta
// 也支援 DollProgram
//
//===================================================

public enum SKILL_RESULT
{
    SUCCESS,
    ERROR,
    NO_TARGET,
    NO_MANA,
    COOL_DOWN,
    NO_BATTLE_POINT,
}


public class SkillBase : MonoBehaviour
{
    public FACTION_GROUP faction = FACTION_GROUP.PLAYER;
    public float damageRatio = 1.0f;
    public float prepareTime = 0.0f;    //技能準備時間，包含地面提示等
    public float duration = 0.2f;   //技能施放期間 (無法操作 )
    public float manaCost = 0;
    public float coolDown = 1.0f;

    public SkillGroundHint[] groundHints;   //地面提示

    //public bool isBattleLevelUpSkill = false;
    public int battlePointsCost = 0;
    public int quality = 0;     //如果有裝備，會等同裝備 ITEM_QUALITY

    public Sprite icon;

    protected GameObject theCaster;
    protected PlayerControllerBase thePC;
    protected Animator theAnimator;
    protected float casterAttack;

    protected Vector3 skillDir = Vector3.back;
    protected Vector3 skillCenter;
    protected GameObject skillTarget = null;

    //protected float cdLeft = 0;
    protected float cdSpeedRate = 1.0f;     //給 CD 加快的 Buff 使用
    protected SkillButton theButton;
    protected int skillIndex = 0;

    protected Damage myDamage;

    protected enum SKILL_PHASE
    {
        NONE,
        JUST_START,
        PREPARE,    //唱招或提示範圍期間，不一定有
        PLAY,
        COOL_DOWN,
        DONE,
    }
    protected SKILL_PHASE currPhase = SKILL_PHASE.NONE;
    protected SKILL_PHASE nextPhase = SKILL_PHASE.NONE;

    protected float stateTimeLeft = 0;

    //===================== 以下只適用於 faction == PLAYER 的場合 
    //Button 相關
    public void OnButtonClicked()
    {
        //print("SkillBase::OnButtonClicked() ...." + skillIndex);
        thePC.OnSkill(skillIndex);
    }

    public void InitButton(SkillButton skillButton)
    {
        theButton = skillButton;
        if (skillButton)
        {
            skillButton.SetIcon(icon);
            skillButton.Bind(OnButtonClicked);
            skillButton.SetCost(battlePointsCost);
            if (skillButton.bgIcon)
            {
                skillButton.bgIcon.color = GameDef.GetQaulityColor((ITEM_QUALITY)quality);
            }

            if (battlePointsCost > 0)
            {
                //skillButton.gameObject.SetActive(BattlePlayerData.GetInstance().GetBattleLVPoints() >= battlePointsCost);
                if (BattleSystem.GetInstance().IsBattleLevelUp)
                    SetupBattlePoints(BattlePlayerData.GetInstance().GetBattleLVPoints());
            }
        }
    }
    //===================== 以上只適用於 faction == PLAYER 的場合 

    public void SetCDRate(float rate)
    {
        cdSpeedRate = rate;
    }

    public void SetupBattlePoints(int points)
    {
        if (battlePointsCost > 0)
        {
            //theButton.gameObject.SetActive(points >= battlePointsCost);
            theButton.SetButtonEnable(points >= battlePointsCost);
        }
    }

    public void SetSkillIndex(int index)
    {
        skillIndex = index;
    }

    //public float GetCoolDownLeft() { return cdLeft / cdSpeedRate; }

    public virtual void InitCasterInfo(GameObject oCaster, float _casterAttack) {
        theCaster = oCaster;
        if (faction == FACTION_GROUP.PLAYER)
            thePC = oCaster.GetComponent<PlayerControllerBase>();
        theAnimator = oCaster.GetComponent<Animator>();

        casterAttack = _casterAttack;
    }

    public virtual bool Play()
    {
        SKILL_RESULT theResult = SKILL_RESULT.SUCCESS;
        return Play(ref theResult);
    }

    public virtual bool Play(ref SKILL_RESULT result)
    {
        if (currPhase != SKILL_PHASE.NONE)
        {
            One.LOG("ERROR!!!! Skill 尚未完成.." + name + " Phase: " + currPhase);
            result = SKILL_RESULT.COOL_DOWN;
            return false;
        }
        if (battlePointsCost > 0)
        {
            if (BattlePlayerData.GetInstance().GetBattleLVPoints() < battlePointsCost)
            {
                result = SKILL_RESULT.NO_BATTLE_POINT;
                return false;
            }
        }
        else
        {

            if (thePC && thePC.GetMP() < manaCost)
            {
                result = SKILL_RESULT.NO_MANA;
                return false;
            }
        }


        if (!DoPrepare(ref result))
            return false;
        
        bool retrunValue = true;
        if (prepareTime == 0)
        {
            retrunValue = DoStart(ref result);
            if (retrunValue)
            {
                currPhase = SKILL_PHASE.JUST_START;
                nextPhase = SKILL_PHASE.PLAY;
            }
        }
        else
        {
            currPhase = SKILL_PHASE.JUST_START;
            nextPhase = SKILL_PHASE.PREPARE;
        }

        if (retrunValue)
            OnSkillSucess();

        return retrunValue;
    }

    protected virtual bool DoPrepare(ref SKILL_RESULT result)
    {
        skillCenter = transform.position;
        skillDir = Vector3.back;
        skillTarget = null;
        result = SKILL_RESULT.SUCCESS;
        return true;
    }


    public virtual bool DoStart() {
        SKILL_RESULT theResult = SKILL_RESULT.SUCCESS;
        return DoStart(ref theResult);
    }

    public bool IsReady()
    {
        switch (currPhase)
        {
            case SKILL_PHASE.NONE:
                return true;
        }
        return false;
    }

    public virtual void OnSkillSucess()
    {
        if (battlePointsCost > 0)
        {
            if (BattlePlayerData.GetInstance().GetBattleLVPoints() >= battlePointsCost)
            {
                BattlePlayerData.GetInstance().AddBattleLVPoints(-battlePointsCost);
                //theButton.gameObject.SetActive(BattlePlayerData.GetInstance().GetBattleLVPoints() >= battlePointsCost);
                //SetupBattlePoints(BattlePlayerData.GetInstance().GetBattleLVPoints());
            }
            else
            {
                One.LOG("ERROR!! No Battle Point OnSkillSucess !!!! ");
            }
        }
        else
        {
            if (thePC)
                thePC.DoUseMP(manaCost);
            //cdLeft = coolDown;
        }

        //currPhase = SKILL_PHASE.JUST_START; //確保  IsReady() 為 false
        //if (prepareTime > 0)
        //    nextPhase = SKILL_PHASE.PREPARE;
        //else
        //    nextPhase = SKILL_PHASE.PLAY;
    }

    public virtual bool DoStart(ref SKILL_RESULT result)
    {
        //if (currPhase != SKILL_PHASE.NONE)
        //{
        //    One.LOG("ERROR!!!! Skill 尚未完成.." + name);
        //    result = SKILL_RESULT.COOL_DOWN;
        //    return false;
        //}
        //if (battlePointsCost > 0)
        //{
        //    if (BattlePlayerData.GetInstance().GetBattleLVPoints() < battlePointsCost)
        //    {
        //        result = SKILL_RESULT.NO_BATTLE_POINT;
        //        return false;
        //    }
        //}
        //else
        //{

        //    if (thePC && thePC.GetMP() < manaCost)
        //    {
        //        result = SKILL_RESULT.NO_MANA;
        //        return false;
        //    }
        //}

        if (faction == FACTION_GROUP.PLAYER)
        {
            if (thePC)
                myDamage.Init(casterAttack * damageRatio, Damage.OwnerType.PLAYER, gameObject.name, gameObject);
            else
                myDamage.Init(casterAttack * damageRatio, Damage.OwnerType.DOLL, gameObject.name, gameObject);
        }
        else if (faction == FACTION_GROUP.ENEMY)
            myDamage.Init(casterAttack * damageRatio, Damage.OwnerType.ENEMY, gameObject.name, gameObject);

        return true;
    }


    protected virtual void Update()
    {
        
        if (nextPhase != currPhase)
        {
            switch (nextPhase)
            {
                case SKILL_PHASE.PREPARE:
                    stateTimeLeft = prepareTime;
                    //print("Skill Direction: " + skillDir);
                    foreach (SkillGroundHint groundHint in groundHints)
                    {
                        float hintLength = groundHint.size.y;
                        float hintWidth = groundHint.size.x;
                        Vector3 hintDirection = Quaternion.Euler(0, groundHint.angleShift, 0) * skillDir;
                        Vector3 hintCenter = skillCenter + hintDirection * hintLength * 0.5f + hintDirection;
                        Vector3 hintShift = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, hintDirection, Vector3.up), 0) * groundHint.posShift;
                        GroundHintManager.GetInstance().ShowSquareHint(hintCenter + hintShift, hintDirection, new Vector2(hintWidth, hintLength), prepareTime);
                    }
                    break;
                case SKILL_PHASE.PLAY:
                    stateTimeLeft = duration;
                    break;
                case SKILL_PHASE.COOL_DOWN:
                    stateTimeLeft = coolDown;
                    break;
                default:
                    stateTimeLeft = 0;
                    break;
            }
            currPhase = nextPhase;
        }

        stateTimeLeft -= Time.deltaTime;

        switch (currPhase)
        {
            case SKILL_PHASE.PREPARE:
                UpdatePrepare();
                break;
            case SKILL_PHASE.PLAY:
                UpdatePlay();
                break;
            case SKILL_PHASE.COOL_DOWN:
                UpdateCoolDown();
                break;
            case SKILL_PHASE.DONE:
                nextPhase = SKILL_PHASE.NONE;
                break;
        }
    }

    
    protected virtual void UpdatePlay()
    {
        if (stateTimeLeft <= 0)
        {
            if (theButton)
            {
                theButton.OnSkillRelease(coolDown);
            }
            if (coolDown > 0)
                nextPhase = SKILL_PHASE.COOL_DOWN;
            else
                nextPhase = SKILL_PHASE.DONE;
        }
    }

    protected virtual void UpdatePrepare()
    {
        if (stateTimeLeft <= 0)
        {
            if (DoStart())
                nextPhase = SKILL_PHASE.PLAY;
            else
                nextPhase = SKILL_PHASE.DONE;
        }
    }

    protected virtual void UpdateCoolDown()
    {
        if (stateTimeLeft <= 0)
        {
            if (theButton)
            {
                theButton.OnSkillCoolDownFinish();
            }
            nextPhase = SKILL_PHASE.DONE;
        }
    }

    //private void OnGUI()
    //{
    //    if (faction == FACTION_GROUP.ENEMY)
    //    {
    //        Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //        thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //        GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), currPhase.ToString());
    //    }
    //}
}
