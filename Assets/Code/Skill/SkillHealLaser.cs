using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//補血雷射，目前只能用在 敵人
//TODO: 針對 Player 和 Doll 的需求 ?

public class SkillHealLaser : SkillLaser
{
    protected override GameObject FindBestShootTarget(float searchRange)
    {
        //尋找 Enemy
        GameObject bestTarget = null;
        float bestTargetHpRatio = 1.0f;  //找血的比例最低的
        Collider[] cols = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Enemy") && col.gameObject != theCaster)
            {

                Enemy enemy = col.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    float hpRatio = enemy.GetHP() / enemy.MaxHP;
                    if (hpRatio < bestTargetHpRatio)
                    {
                        bestTargetHpRatio = hpRatio;
                        bestTarget = enemy.gameObject;
                    }
                }
            }
        }
        //print("SkillHealLaser 找尋目標結果: " + bestTarget);
        return bestTarget;
    }
}
