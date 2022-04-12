using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Healer : DollAuto
{
    // Start is called before the first frame update
    protected override bool SearchTarget()
    {
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc.GetHP() < pc.GetHPMax())
            myTarget = pc.gameObject;
        else
            myTarget = null;
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
