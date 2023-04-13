using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSE_FlyingRock : OSEnemy
{
    //protected Damage myDamage;
    public override void SetUpLevel(int iLv = 1)
    {
        Score *= iLv;
    }

    private void OnTriggerEnter(Collider col)
    {
        //Damage myDamage;
        myDamage.damage = Attack;
        //myDamage.ID = gameObject.name;
        //myDamage.type = Damage.OwnerType.ENEMY;
        //myDamage.Owner = gameObject;
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
