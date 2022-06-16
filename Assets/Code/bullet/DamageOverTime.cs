using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : bullet_base
{
    public float damagePeriod = 0.5f;
    public Vector2 BoxSize;
    public GameObject hitFX;

    protected float damageTime = 0.0f;
    protected Damage myDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        damageTime += Time.deltaTime;
        if (damageTime >= damagePeriod)
        {
            DoOneDamage();
            damageTime -= damagePeriod;
        }
    }

    void DoOneDamage()
    {
        myDamage.damage = baseDamage;

        Collider[] cols = Physics.OverlapBox(transform.position, new Vector3(BoxSize.x * 0.5f, 1.0f, BoxSize.y * 0.5f));
        foreach (Collider col in cols)
        {
            bool hit = false;
            if (col.gameObject.CompareTag("Enemy") && group == DAMAGE_GROUP.PLAYER)
            {
                //print("Trigger:  Hit Enemy !! ");
                col.gameObject.SendMessage("OnDamage", myDamage);
                hit = true;
            }
            else if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Doll")) && group == DAMAGE_GROUP.ENEMY)
            {
                //print("Trigger:  Hit Player !!");
                col.gameObject.SendMessage("OnDamage", myDamage);
                hit = true;
            }

            if (hit && hitFX)
            {
                Vector3 hitPos = col.ClosestPoint(transform.position);

                BattleSystem.GetInstance().SpawnGameplayObject(hitFX, hitPos, false);   //特效不用回收
            }
        }
    }
}
