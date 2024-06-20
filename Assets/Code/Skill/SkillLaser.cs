using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLaser : SkillBase
{
    public GameObject theLaserRef;
    protected BulletLaser myLaser;
    protected GameObject myTarget;

    bool isLaser = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        if (myLaser == null)
        {
            GameObject o = BattleSystem.SpawnGameObj(theLaserRef, transform.position);
            myLaser = o.GetComponent<BulletLaser>();
            StopLaser();
        }
    }

    public override bool DoStart(ref SKILL_RESULT result)
    {
        //∞Ú•ª¿À¨d
        if (!base.DoStart(ref result))
            return false;

        //print("SkillShoot!!!!!! ");
        myTarget = FindBestShootTarget(10.0f);
        if (myTarget == null)
        {
            result = SKILL_RESULT.NO_TARGET;
            return false;
        }

        result = SKILL_RESULT.SUCCESS;
        base.OnSkillSucess();
        return true; ;
    }

    protected override void Update()
    {
        base.Update();
        if (isLaser)
        {
            myLaser.UpdateLaser(myTarget, transform.position);
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
}
