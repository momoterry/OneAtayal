using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplore : bullet
{
    public float expRadius = 2.0f;
    public GameObject expFX;

    private void OnTriggerEnter(Collider col)
    {
        //print("BulletExplore::OnTriggerEnter : " + col);
        bool hit = false;
        bool destroy = false;
        if (col.gameObject.CompareTag("Enemy") && group == DAMAGE_GROUP.PLAYER)
        {
            //print("Trigger:  Hit Enemy !!");
            hit = true;
        }
        else if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Doll")) && group == DAMAGE_GROUP.ENEMY)
        {
            //print("Trigger:  Hit Player or Doll !!");
            hit = true;
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //print("Trigger:  HitWall !!");
            hit = true;
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("DeadZone"))
        {
            destroy = true;
        }


        if (hit)
        {
            Vector3 hitPos = col.ClosestPoint(transform.position);

            BattleSystem.GetInstance().SpawnGameplayObject(expFX, hitPos, false);

            destroy = true;

            //Ãz¬µ³B²z
            Collider[] cols = Physics.OverlapSphere(hitPos, expRadius);
            foreach(Collider co in cols)
            {
                if ((co.gameObject.CompareTag("Enemy") && group == DAMAGE_GROUP.PLAYER)
                    || ((co.gameObject.CompareTag("Player") || co.gameObject.CompareTag("Doll")) && group == DAMAGE_GROUP.ENEMY))
                {
                    co.gameObject.SendMessage("OnDamage", myDamage);
                    BattleSystem.GetInstance().SpawnGameplayObject(hitFX, co.transform.position, false);
                }
            }

        }

        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
