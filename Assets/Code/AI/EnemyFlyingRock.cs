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


    private void OnTriggerEnter(Collider col)
    {
        bool hit = false;
        if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Doll")) )
        {
            //print("Trigger:  Hit Player or Doll !!");
            hit = true;
            col.gameObject.SendMessage("OnDamage", myDamage);
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //print("Trigger:  HitWall !!");
            hit = true;
        }


        if (hit)
        {
            DoDeath();
        }
    }
}
