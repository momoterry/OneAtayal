using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ɦ�p�g�A�ثe�u��Φb �ĤH
//TODO: �w�� Player �M Doll ���ݨD ?

public class SkillHealLaser : SkillLaser
{
    protected override GameObject FindBestShootTarget(float searchRange)
    {
        //�M�� Enemy
        GameObject bestTarget = null;
        float bestTargetHpRatio = 1.0f;  //��媺��ҳ̧C��
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
        //print("SkillHealLaser ��M�ؼе��G: " + bestTarget);
        return bestTarget;
    }
}
