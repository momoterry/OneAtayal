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

        //And Dolls (��媺��ҳ̤֪�)
        List<Doll> theList = thePC.GetDollManager().GetDolls();
        foreach (Doll d in theList)
        {
            //if (d == this)
            //    continue;
            HitBody body = d.GetComponent<HitBody>();
            if (body && body.GetHP() < body.GetHPMax())
            {
                float hpRatio = body.GetHP() / body.GetHPMax();
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