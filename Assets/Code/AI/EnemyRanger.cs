using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyRanger : Enemy
{
    public GameObject bulletRef;

    protected override void DoOneAttack()
    {
        // TODO: �ˬd�O�_���a�b����
        if (bulletRef)
        {
            Vector3 shootPoint = gameObject.transform.position + faceDir * 0.5f;
#if XZ_PLAN
            GameObject newObj = Instantiate(bulletRef, shootPoint, Quaternion.Euler(90, 0, 0), null);
#else
            GameObject newObj = Instantiate(bulletRef, shootPoint, Quaternion.identity, null);
#endif
            if (newObj)
            {
                bullet newBullet = newObj.GetComponent<bullet>();
                if (newBullet)
                {
                    newBullet.SetGroup(DAMAGE_GROUP.ENEMY);
                    Vector3 td = targetObj.transform.position - newObj.transform.position;
#if XZ_PLAN
                    td.y = 0;
#else
                    td.z = 0;
#endif
                    newBullet.targetDir = td.normalized;
                    newBullet.phyDamage = Attack;
                }
            }
        }

        if (myAnimator)
        {
            myAnimator.SetTrigger("Attack");
        }
    }
}
