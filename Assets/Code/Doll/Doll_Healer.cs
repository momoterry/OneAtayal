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
            print("Player HP  " + pc.GetHP() + " / " + pc.GetHPMax());
            bestTargetHpRatio = pc.GetHP() / pc.GetHPMax();
            myTarget = pc.gameObject;
        }
        else
            myTarget = null;

        //And Dolls (找血的比例最少的)
        List<Doll> theList = pc.GetDollManager().GetDolls();
        foreach (Doll d in theList)
        {
            if (d == this)
                continue;
            HitBody body = d.GetComponent<HitBody>();
            if (body && body.GetHP()< body.GetHPMax())
            {
                float hpRatio = body.GetHP() / body.GetHPMax();
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
