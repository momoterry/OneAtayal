using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHealBall : SkillShoot
{
    protected override GameObject FindBestShootTarget(float searchRange)
    {
        GameObject myTarget = null;
        float bestTargetHpRatio = 1.0f;

        //if (thePC.GetHP() < thePC.GetHPMax())
        //{
        //    bestTargetHpRatio = thePC.GetHP() / thePC.GetHPMax();
        //    myTarget = thePC.gameObject;
        //}
        //else
        //    myTarget = null;

        //And Dolls (找血的比例最少的)
        List<Doll> theList = thePC.GetDollManager().GetDolls();
        foreach (Doll d in theList)
        {
            if (!d.gameObject.activeInHierarchy)
                continue;

            HitBody body = d.GetComponent<HitBody>();
            if (body && body.GetHP() < body.GetHPMax())
            {
                float preHealValue = 0;
                PreHealInfo pi = d.GetComponent<PreHealInfo>();
                if (pi)
                {
                    preHealValue = pi.GetPreHeal();
                }
                float hpRatio = (body.GetHP()+preHealValue) / body.GetHPMax();
                if (hpRatio < bestTargetHpRatio)
                {
                    myTarget = body.gameObject;
                    bestTargetHpRatio = hpRatio;
                }
            }
        }

        //print("BestHealTarget: " + myTarget);

        return myTarget;
    }

}
