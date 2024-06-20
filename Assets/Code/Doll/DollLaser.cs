using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollLaser : DollBeta
{
    public GameObject theLaserRef;
    protected BulletLaser myLaser;

    bool isLaser = false;

    protected float checkTime = 0;

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
        //if (myAgent)
        //    myAgent.SetDestination(mySlot.position);
        FollowSlot();

        myFace = BattleSystem.GetPC().GetFaceDir();

        if (!isLaser)
        {
            checkTime += Time.deltaTime;
            if (checkTime > 0.1f)
            {
                checkTime = 0;

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


    protected override void OnDeath()
    {
        StopLaser();
        base.OnDeath();
    }
}
