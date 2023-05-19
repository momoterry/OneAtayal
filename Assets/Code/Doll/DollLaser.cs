using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollLaser : DollAuto
{
    public GameObject theLaserRef;
    protected BulletLaser myLaser;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    protected override void UpdateBattle()
    {
        base.UpdateBattle();

        if (myLaser && myLaser.gameObject.activeInHierarchy)
        {
            if (myTarget != null && myTarget.activeInHierarchy)
            {
                //print("myTarget alive!! " + myTarget);
                myLaser.UpdateLaser(myTarget, transform.position);
            }
            else
            {
                myLaser.gameObject.SetActive(false);
                //print("Target Off !!");
            }
        }
    }


    protected override void DoOneAttack()
    {
        if (myLaser == null)
        {
            GameObject o = BattleSystem.SpawnGameObj(theLaserRef, transform.position);
            myLaser = o.GetComponent<BulletLaser>();
        }
        //print("Target On !!");
        myLaser.gameObject.SetActive(true);
        Vector3 td = myTarget.transform.position - transform.position;
        myLaser.InitValue(DAMAGE_GROUP.PLAYER, myDamage, td, myTarget);
        myLaser.UpdateLaser(myTarget, transform.position);
    }
}
