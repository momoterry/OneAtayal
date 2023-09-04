using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SKILL_RESULT
{
    SUCCESS,
    ERROR,
    NO_TARGET,
    NO_MANA,
    COLL_DOWN,
    NO_BATTLE_POINT,
}

public class SkillBase : MonoBehaviour
{
    public float damageRatio = 1.0f;
    public float duration = 0.2f;   //技能施放期間 (無法操作 )
    public float manaCost = 0;
    public float coolDown = 1.0f;

    //public bool isBattleLevelUpSkill = false;
    public int battlePointsCost = 0;

    public Sprite icon;

    protected GameObject theCaster;
    protected PlayerControllerBase thePC;
    protected Animator theAnimator;

    protected float cdLeft = 0;
    protected SkillButton theButton;
    protected int skillIndex = 0;

    protected Damage myDamage;

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

            if (battlePointsCost > 0)
            {
                //skillButton.gameObject.SetActive(BattlePlayerData.GetInstance().GetBattleLVPoints() >= battlePointsCost);
                if (BattleSystem.GetInstance().IsBattleLevelUp)
                    SetupBattlePoints(BattlePlayerData.GetInstance().GetBattleLVPoints());
            }
        }
    }

    public void SetupBattlePoints( int points)
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

    public float GetCoolDownLeft() { return cdLeft; }

    public virtual void InitCasterInfo(GameObject oCaster) { 
        theCaster = oCaster;
        thePC = oCaster.GetComponent<PlayerControllerBase>();
        theAnimator = oCaster.GetComponent<Animator>();
    }
    public virtual bool DoStart() {
        SKILL_RESULT theResult = SKILL_RESULT.SUCCESS;
        return DoStart(ref theResult);
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
                print("ERROR!! No Battle Point OnSkillSucess !!!! ");
            }
        }
        else
        {
            thePC.DoUseMP(manaCost);
            cdLeft = coolDown;
        }
        if (theButton)
        {
            theButton.OnSkillRelease(coolDown);
        }
    }

    public virtual bool DoStart(ref SKILL_RESULT result)
    {
        if (thePC == null)
        {
            result = SKILL_RESULT.ERROR;
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
            if (cdLeft > 0)
            {
                result = SKILL_RESULT.COLL_DOWN;
                //print("SKILL CD " + cdLeft);
                return false;
            }

            if (thePC.GetMP() < manaCost)
            {
                result = SKILL_RESULT.NO_MANA;
                return false;
            }
        }
        return true;
    }


    protected virtual void Update()
    {
        if (cdLeft > 0)
        {
            cdLeft -= Time.deltaTime;
            if (cdLeft <= 0)
            {
                cdLeft = 0;
                if (theButton)
                {
                    theButton.OnSkillCoolDownFinish();
                }
            }
        }
    }

    protected virtual void Start()
    {
        myDamage.Init(0, Damage.OwnerType.PLAYER, gameObject.name, gameObject);
    }
}
