using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLaser : SkillBase
{
    public GameObject theLaserRef;
    public float searchRange = 10.0f;
    public float maxRange = 12.0f;
    public float laserDuration = 2.0f;


    protected BulletLaser myLaser;
    protected GameObject myTarget;
    protected float laserTime = 0;

    bool isLaser = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (myLaser == null)
        {
            GameObject o = BattleSystem.SpawnGameObj(theLaserRef, transform.position);
            o.transform.parent = transform;
            myLaser = o.GetComponent<BulletLaser>();
            StopLaser();
        }
    }

    public override bool DoStart(ref SKILL_RESULT result)
    {
        if (isLaser)
        {
            result = SKILL_RESULT.COLL_DOWN;
            return false;
        }

        //���ˬd
        if (!base.DoStart(ref result))
            return false;

        //print("SkillShoot!!!!!! ");
        myTarget = FindBestShootTarget(searchRange);
        if (myTarget == null)
        {
            result = SKILL_RESULT.NO_TARGET;
            return false;
        }

        StartLaser();

        result = SKILL_RESULT.SUCCESS;
        base.OnSkillSucess();
        return true; ;
    }

    protected override void Update()
    {
        base.Update();
        if (isLaser)
        {
            if (myTarget == null || !myTarget.activeInHierarchy)
            {
                StopLaser();
            }
            else
            {
                laserTime += Time.deltaTime;
                //myLaser.UpdateLaser(myTarget, transform.position);
                if (Vector3.Distance(myTarget.transform.position, transform.position) > maxRange || laserTime > laserDuration)
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
    protected GameObject FindBestShootTarget(float searchRange)
    {
        if (theCaster == null)
            return null;

        GameObject bestTarget = null;
        if (faction == FACTION_GROUP.PLAYER)
        {
            bestTarget = BattleUtility.SearchClosestTargetForPlayer(theCaster.transform.position, searchRange);
            if (!bestTarget)
                bestTarget = BattleSystem.GetPC().GetHittableTarget();
        }
        else
            bestTarget = BattleUtility.SearchClosestTargetForEnemy(theCaster.transform.position, searchRange);

        return bestTarget;
    }

    protected void StartLaser()
    {
        //print("StartLaser----");
        isLaser = true;
        laserTime = 0;
        myLaser.gameObject.SetActive(true);
        Vector3 td = myTarget.transform.position - transform.position;
        myLaser.InitValue(FACTION_GROUP.PLAYER, myDamage, td, myTarget);
        myLaser.UpdateLaser(myTarget, transform.position);
    }

    protected void StopLaser()
    {
        //print("StopLaser");
        isLaser = false;
        myLaser.gameObject.SetActive(false);
    }
}