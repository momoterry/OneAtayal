using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrower : Enemy 
{
    public GameObject damageObject;
    public float objectRange = 1.0f;

    protected override void DoOneAttack()
    {
        if (damageObject)
        {
            Vector3 objPoint = gameObject.transform.position + faceDir * objectRange;
#if XZ_PLAN
            GameObject newObj = Instantiate(damageObject, objPoint, Quaternion.Euler(90.0f, 0, 0), null);
#else
            GameObject newObj = Instantiate(damageObject, objPoint, Quaternion.identity, null);
#endif
            if (newObj)
            {
                //DamageByAnimation d = newObj.GetComponent<DamageByAnimation>();
                //if (d)
                //{
                //    d.SetGroup(FACTION_GROUP.ENEMY);
                //    d.baseDamage = Attack;
                //}
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {

                    newBullet.InitValue(FACTION_GROUP.ENEMY, myDamage, faceDir);
                }
            }
        }

        if (myAnimator)
        {
            myAnimator.SetTrigger("Attack");
        }
    }
}
