using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  純防禦型的 Doll
//  目前的設計上不會有攻擊行為
//

public class Doll_Shield : DollBeta
{
    protected override void DoOneAttack()
    {
        print("空的攻擊來了!!");
    }
}
