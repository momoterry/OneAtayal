using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Attacker : DollAuto
{
    //�]�L�h�S�]�^�Ӫ�����
    // Start is called before the first frame update

    protected override void DoOneAttack()
    {
        base.DoOneAttack();

        nextAutoState = AutoState.RUNBACK;
    }
}
