using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Healer : DollAuto
{
    // Start is called before the first frame update
    protected override bool SearchTarget()
    {
        float bestTargetHpRatio = 1.0f;
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc.GetHP() < pc.GetHPMax())
        {
            //print("Player HP  " + pc.GetHP() + " / " + pc.GetHPMax());
            float preHealValue = 0;
            PreHealInfo pi = pc.GetComponent<PreHealInfo>();
            if (pi)
            {
                preHealValue = pi.GetPreHeal();
            }
            bestTargetHpRatio = ( pc.GetHP()+preHealValue ) / pc.GetHPMax();
            myTarget = pc.gameObject;
        }
        else
            myTarget = null;

        //And Dolls (找血的比例最少的)
        List<Doll> theList = pc.GetDollManager().GetDolls();
        foreach (Doll d in theList)
        {
            if (!d.gameObject.activeInHierarchy)
                continue;
            if (d == this)
                continue;
            HitBody body = d.GetComponent<HitBody>();
            if (body && body.GetHP()< body.GetHPMax())
            {
                float preHealValue = 0;
                PreHealInfo pi = d.GetComponent<PreHealInfo>();
                if (pi)
                {
                    preHealValue = pi.GetPreHeal();
                }

                float hpRatio = (body.GetHP() + preHealValue) / body.GetHPMax();
                if (hpRatio < bestTargetHpRatio)
                {
                    myTarget = body.gameObject;
                    bestTargetHpRatio = hpRatio;
                }
            }
        }

        return (myTarget!=null);
    }

    protected override void DoOneAttack()
    {
        if (!SearchTarget())
        {
            nextAutoState = AutoState.RUNBACK;
        }
        else
            base.DoOneAttack();
    }
}
