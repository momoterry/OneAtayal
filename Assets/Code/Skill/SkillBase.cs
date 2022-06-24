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
}

public class SkillBase : MonoBehaviour
{
    public float damageRatio = 1.0f;
    public float duration = 0.2f;   //技能施放期間 (無法操作 )
    public float manaCost = 0;
    public float coolDown = 1.0f;

    public Sprite icon;

    protected GameObject theCaster;
    protected PlayerControllerBase thePC;
    protected Animator theAnimator;

    protected float cdLeft = 0;
    protected SkillButton theButton;
    protected int skillIndex = 0;

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
        thePC.DoUseMP(manaCost);
        cdLeft = coolDown;
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

}
