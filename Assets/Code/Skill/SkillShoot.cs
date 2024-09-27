using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillShoot : SkillBase
{
    public GameObject bulletRef;
    public float bulletInitDis = 0.25f;
    public float searchRange = 10.0f;
    public bool shootEvenNoEnemy = true;
    public bool autoAim = true;
    public bool faceTarget = true;

    public GameObject[] fireFXRefs;

    //protected PlayerControllerBase thePC;
    //protected Animator theAnimator;

    //public override void InitCasterInfo(GameObject oCaster)
    //{
    //    base.InitCasterInfo(oCaster);

    //    thePC = oCaster.GetComponent<PlayerControllerBase>();
    //    theAnimator = oCaster.GetComponent<Animator>();
    //}

    protected virtual GameObject FindBestShootTarget(float searchRange)
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


    protected override bool DoPrepare(ref SKILL_RESULT result)
    {
        if (!base.DoPrepare(ref result))
            return false;


        GameObject target = FindBestShootTarget(searchRange);
        if (target == null && !shootEvenNoEnemy)
        {
            result = SKILL_RESULT.NO_TARGET;
            return false;
        }

        Vector3 td;
        if (target != null && autoAim)
            td = target.transform.position - theCaster.transform.position;
        else
        {
            if (thePC)
                td = thePC.GetFaceDir();
            else
                td = Vector3.back;  //TODO: �p�G�O�ĤH���ܡA��w�]���V?
        }

        td.y = 0;

        td.Normalize();

        if (faceTarget && thePC)
            thePC.SetupFaceDir(td);

        skillDir = td;
        skillCenter = theCaster.transform.position + skillDir * bulletInitDis;
        skillTarget = target;
        return true;
    }

    public override bool DoStart(ref SKILL_RESULT result)
    {
        //���ˬd
        if (!base.DoStart(ref result))
            return false;

        //print("SkillShoot!!!!!! ");
//        GameObject target = FindBestShootTarget(searchRange);
//        if (target == null && !shootEvenNoEnemy)
//        {
//            result = SKILL_RESULT.NO_TARGET;
//            return false;
//        }

//        Vector3 td;
//        if (target != null && autoAim)
//            td = target.transform.position - theCaster.transform.position;
//        else
//        {
//            if (thePC)
//                td = thePC.GetFaceDir();
//            else
//                td = Vector3.back;  //TODO: �p�G�O�ĤH���ܡA��w�]���V?
//        }

//#if XZ_PLAN
//        td.y = 0;
//#else
//        td.z = 0;
//#endif

//        td.Normalize();
//        skillDir = td;

//        if (faceTarget && thePC)
//            thePC.SetupFaceDir(td);

//        Vector3 shootPos = theCaster.transform.position + td * bulletInitDis;

        GameObject newObj = BattleSystem.GetInstance().SpawnGameplayObject(bulletRef, skillCenter, false);
        if (newObj)
        {
            bullet_base newBullet = newObj.GetComponent<bullet_base>();
            if (newBullet)
            {
                //myDamage.damage = casterAttack * damageRatio;
                //if (thePC)
                //    myDamage.damage = thePC.GetATTACK() * damageRatio;  // ���F�䴩 PC �� Buff ���A�A���������ϥ� casterAttack TODO: �ݭn�ץ�
                //else
                //    myDamage.damage = casterAttack * damageRatio;
                newBullet.InitValue(faction, myDamage, skillDir, skillTarget);
            }
        }

        //�o�g�S��
        foreach (GameObject fx in fireFXRefs)
        {
            Instantiate(fx, transform.position, Quaternion.Euler(90, 0, 0), transform);
        }

        if (theAnimator)
        {
            theAnimator.SetFloat("CastX", skillDir.x);
            theAnimator.SetFloat("CastY", skillDir.z);
            theAnimator.SetTrigger("Cast");
        }

        //thePC.DoUseMP(manaCost);
        //cdLeft = coolDown;
        result = SKILL_RESULT.SUCCESS;
        base.OnSkillSucess();
        return true; ;
    }


}
