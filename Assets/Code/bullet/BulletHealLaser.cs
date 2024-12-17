using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHealLaser : BulletLaser
{
    protected override void DoOneDamage(GameObject targetO)
    {
        //base.DoOneDamage(targetO);
        if (group == FACTION_GROUP.ENEMY)
        {
            Enemy e = targetO.GetComponent<Enemy>();
            if (e)
            {
                float healed = e.DoHeal(myDamage.damage * attackPeriod, 0);
                //print("Heal Enemy: " + healed);
            }
        }
        else
        {
            print("還沒實作友方雷射補血");
        }
    }
}
