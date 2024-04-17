using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDollSummonEx : SkillBase
{
    public float HP_Percent = 100;
    public float ATK_Percent = 100;

    public string dollID;       //如果定義了 DollID，就自動找尋 dollRef 和 icon
    public GameObject dollRef;
    protected GameObject summonFX;
    public float defaultSummonDistance = 2.0f;

    private void Awake()
    {
        //print("SkillDollSummonEx Awake:" + dollID);
        if (dollID != "" && GameSystem.GetDollData())
        {
            dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
            if (dollRef)
            {
                Doll doll = dollRef.GetComponent<Doll>();
                if (doll)
                {
                    icon = doll.icon;
                }
            }
        }
        summonFX = GameSystem.GetDollData().defautSpawnFX;
    }

    public override bool DoStart(ref SKILL_RESULT result)
    {
        if (!base.DoStart(ref result))
            return false;
        //================================

        DollManager dm = thePC.GetDollManager();
        if (dollRef == null || dm == null)
        {
            result = SKILL_RESULT.ERROR;
            return false;
        }

        Doll refDoll = dollRef.GetComponent<Doll>();
        if (!refDoll)
        {
            result = SKILL_RESULT.ERROR;
            return false;
        }

        if (!dm.HasEmpltySlot(refDoll.positionType))
        {
            thePC.SaySomthing("沒有空間了....");
            result = SKILL_RESULT.ERROR;
            return false;
        }


        Vector3 pos = transform.position + thePC.GetFaceDir() * defaultSummonDistance;

        if (summonFX)
            BattleSystem.GetInstance().SpawnGameplayObject(summonFX, pos, false);

        GameObject dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);
        Doll theDoll = dollObj.GetComponent<Doll>();
        if (theDoll == null)
        {
            print("Error!! There is no Doll in dollRef !!");
            Destroy(dollObj);
        }
        HitBody hitBody = dollObj.GetComponent<HitBody>();
        //開始調整參數
        theDoll.AttackInit *= ATK_Percent / 100.0f;
        if (hitBody)
        {
            hitBody.HP_Max *= HP_Percent / 100.0f;
        }


        if (!theDoll.TryJoinThePlayer(DOLL_JOIN_SAVE_TYPE.BATTLE))
        {
            print("Woooooooooops.......");
        }


        Vector3 td = (pos - transform.position).normalized;

        if (theAnimator)
        {
            theAnimator.SetFloat("CastX", td.x);
            theAnimator.SetFloat("CastY", td.z);

            theAnimator.SetTrigger("Cast");
        }

        //================================
        result = SKILL_RESULT.SUCCESS;
        base.OnSkillSucess();
        return true; ;
    }
}
