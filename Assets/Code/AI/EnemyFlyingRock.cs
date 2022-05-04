using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyingRock : Enemy
{
    protected override void UpdateIdle()
    {
        //Do Nothing
    }

    void OnDamage(Damage theDamage)
    {
        //print("Dummy OnDamage");
        if (damageFX)
            Instantiate(damageFX, transform.position, Quaternion.identity, null);

        if (myAnimator)
            myAnimator.SetTrigger("Hit");

        hp -= theDamage.damage;
        if (hp <= 0.0f)
        {
            DoDeath();
        }

    }
}
