using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Wing : Doll
{
    public float bullet_pos = 0.5f;
    public float rapied_shoot_period = 0.125f;

    protected float timeToShoot = 0;


    protected override void UpdateBattle()
    {
        gameObject.transform.position = mySlot.position;

        timeToShoot -= Time.deltaTime;
        if (timeToShoot <=0) 
        {
            DoOneWingShoot();

            timeToShoot += rapied_shoot_period; // 只加上去以確保頻率的連續性
        }
    }


    virtual protected void DoOneWingShoot()
    {
#if XZ_PLAN
        Quaternion rm = Quaternion.Euler(90, 0, 0);
        Vector3 shootTo = Vector3.forward;
#else
        Quaternion rm = Quaternion.identity;
        Vector3 shootTo = Vector3.up;
#endif

        GameObject newObj = Instantiate(bulletRef, transform.position + bullet_pos * shootTo, rm, null);
        if (newObj)
        {
            bullet_base newBullet = newObj.GetComponent<bullet_base>();
            if (newBullet)
            {

                newBullet.InitValue(FACTION_GROUP.PLAYER, myDamage, shootTo);
            }
        }
    }
}
