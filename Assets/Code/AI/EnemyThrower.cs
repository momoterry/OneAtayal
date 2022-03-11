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
            GameObject newObj = Instantiate(damageObject, objPoint, Quaternion.identity, null);
            if (newObj)
            {
                DamageByAnimation d = newObj.GetComponent<DamageByAnimation>();
                if (d)
                {
                    d.SetGroup(DAMAGE_GROUP.ENEMY);
                    d.phyDamage = Attack;
                    print("SetUp OK");
                }
            }
        }

    }
}
