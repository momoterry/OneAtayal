using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRanger : Enemy
{
    public GameObject bulletRef;

    protected override void DoOneAttack()
    {
        // TODO: 檢查是否玩家在視野
        if (bulletRef)
        {
            GameObject newObj = Instantiate(bulletRef, gameObject.transform.position, Quaternion.identity, null);
            if (newObj)
            {
                bullet newBullet = newObj.GetComponent<bullet>();
                if (newBullet)
                {
                    newBullet.SetGroup(DAMAGE_GROUP.ENEMY);
                    Vector3 td = targetObj.transform.position - newObj.transform.position;
                    td.z = 0;
                    newBullet.targetDir = td.normalized;
                    newBullet.phyDamage = Attack;
                }
            }
        }

    }
}
