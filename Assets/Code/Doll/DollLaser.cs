using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollLaser : DollAuto
{
    public GameObject theLaserRef;
    protected BulletLaser myLaser;

    bool isLaser = false;

    protected override void Start()
    {
        base.Start();
        if (myLaser == null)
        {
            GameObject o = BattleSystem.SpawnGameObj(theLaserRef, transform.position);
            myLaser = o.GetComponent<BulletLaser>();
            StopLaser();
        }
    }

    protected override void UpdateFollow()
    {
        if (myAgent)
            myAgent.SetDestination(mySlot.position);

        myFace = BattleSystem.GetPC().GetFaceDir();

        if (!isLaser)
        {
            if (autoStateTime > 0.1f)
            {
                autoStateTime = 0;

                if (SearchTarget())
                {
                    StartLaser();
                }
            }
        }
        else
        {
            if (myTarget == null || !myTarget.activeInHierarchy)
            {
                StopLaser();
            }
            else
            {
                if (Vector3.Distance(myTarget.transform.position, transform.position) > AttackRangeOut)
                {
                    StopLaser();
                }
                else
                {
                    myLaser.UpdateLaser(myTarget, transform.position);
                }
            }
        }
    }

    protected void StartLaser()
    {
        isLaser = true;
        myLaser.gameObject.SetActive(true);
        Vector3 td = myTarget.transform.position - transform.position;
        myLaser.InitValue(FACTION_GROUP.PLAYER, myDamage, td, myTarget);
        myLaser.UpdateLaser(myTarget, transform.position);
    }

    protected void StopLaser()
    {
        isLaser = false;
        myLaser.gameObject.SetActive(false);
    }

    //protected override void UpdateBattle()
    //{
    //    base.UpdateBattle();

    //    if (myLaser && myLaser.gameObject.activeInHierarchy)
    //    {
    //        if (myTarget != null && myTarget.activeInHierarchy)
    //        {
    //            //print("myTarget alive!! " + myTarget);
    //            myLaser.UpdateLaser(myTarget, transform.position);
    //        }
    //        else
    //        {
    //            myLaser.gameObject.SetActive(false);
    //            //print("Target Off !!");
    //        }
    //    }
    //}


    //protected override void DoOneAttack()
    //{
    //    if (myLaser == null)
    //    {
    //        GameObject o = BattleSystem.SpawnGameObj(theLaserRef, transform.position);
    //        myLaser = o.GetComponent<BulletLaser>();
    //    }
    //    //print("Target On !!");
    //    myLaser.gameObject.SetActive(true);
    //    Vector3 td = myTarget.transform.position - transform.position;
    //    myLaser.InitValue(FACTION_GROUP.PLAYER, myDamage, td, myTarget);
    //    myLaser.UpdateLaser(myTarget, transform.position);
    //}

    protected override void OnDeath()
    {
        //print("被打死了 !!");
        //if (myLaser)
        //{
        //    //print("雷射清除");
        //    Destroy(myLaser.gameObject);
        //    myLaser = null;
        //}

        StopLaser();
        base.OnDeath();
    }
}
