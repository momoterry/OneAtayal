using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSE_FlyingRock : OSEnemy
{

    private void OnTriggerEnter(Collider col)
    {
        Damage myDamage;
        myDamage.damage = Attack;
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
