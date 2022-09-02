using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDollSummon : SkillBase
{
    public GameObject dollRef;
    public GameObject summonFX;
    public float defaultSummonDistance = 2.0f;


    public override bool DoStart(ref SKILL_RESULT result)
    {
        if (!base.DoStart(ref result))
            return false;
        //================================

        DollManager dm = thePC.GetDollManager();
        if (dollRef==null || dm == null)
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

        //Transform availableSlot = dm.GetEmptySlot(refDoll.positionType);
        //if (availableSlot == null)
        if (!dm.HasEmpltySlot(refDoll.positionType))
        {
            thePC.SaySomthing("沒有空間了....");
            result = SKILL_RESULT.ERROR;
            return false;
        }
        //print("---- " + availableSlot.position);


        Vector3 pos = transform.position + thePC.GetFaceDir() * defaultSummonDistance;
        //Vector3 pos = availableSlot.position;

        if (summonFX)
            BattleSystem.GetInstance().SpawnGameplayObject(summonFX, pos, false);

        GameObject dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);
        Doll theDoll = dollObj.GetComponent<Doll>();
        if (theDoll == null)
        {
            print("Error!! There is no Doll in dollRef !!");
            Destroy(dollObj);
        }

        if (!theDoll.TryJoinThePlayer())
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
