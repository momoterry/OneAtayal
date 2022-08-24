using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Attacker : DollAuto
{
    //跑過去又跑回來的類型
    // Start is called before the first frame update

    protected override void DoOneAttack()
    {
        base.DoOneAttack();

        nextAutoState = AutoState.RUNBACK;
    }
}
