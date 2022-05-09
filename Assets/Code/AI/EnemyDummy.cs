using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : Enemy
{
    // Start is called before the first frame update
    protected override void UpdateIdle()
    {
        //Do Nothing
        if (hp<MaxHP)
        {
            hp += Time.deltaTime * 50.0f;
            if (hp > MaxHP)
                hp = MaxHP;
        }
    }

    void OnDamage(Damage theDamage)
    {
        //print("Dummy OnDamage");
        //if (damageFX)
        //    Instantiate(damageFX, transform.position, Quaternion.identity, null);

        if (myAnimator)
            myAnimator.SetTrigger("Hit");

        hp -= theDamage.damage;
        if (hp < 1.0f)
        {
            hp = 1.0f;
            //Âê¦å
        }

    }
}
