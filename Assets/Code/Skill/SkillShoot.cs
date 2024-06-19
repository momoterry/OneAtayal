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

    public override bool DoStart(ref SKILL_RESULT result)
    {
        //基本檢查
        if (!base.DoStart(ref result))
            return false;

        //print("SkillShoot!!!!!! ");
        GameObject target = FindBestShootTarget(searchRange);
        if (target == null && !shootEvenNoEnemy)
        {
            result = SKILL_RESULT.NO_TARGET;
            return false;
        }

        // PlayerControllerBase thePC = theCaster.GetComponent<PlayerControllerBase>();
        //if (thePC == null)
        //{
        //    result = SKILL_RESULT.ERROR;
        //    return false;
        //}

        //if (thePC.GetMP() < manaCost)
        //{
        //    //print("Mana 不夠啦 !!");
        //    result = SKILL_RESULT.NO_MANA;
        //    return false;
        //}
        //thePC.DoUseMP(manaCost);

        Vector3 td;
        if (target != null && autoAim)
            td = target.transform.position - theCaster.transform.position;
        else
        {
            if (thePC)
                td = thePC.GetFaceDir();
            else
                td = Vector3.back;  //TODO: 如果是敵人的話，抓預設面向?
        }

#if XZ_PLAN
        td.y = 0;
#else
        td.z = 0;
#endif

        td.Normalize();
        if (faceTarget && thePC)
            thePC.SetupFaceDir(td);

        Vector3 shootPos = theCaster.transform.position + td * bulletInitDis;

        GameObject newObj = BattleSystem.GetInstance().SpawnGameplayObject(bulletRef, shootPos, false);
        if (newObj)
        {
            bullet_base newBullet = newObj.GetComponent<bullet_base>();
            if (newBullet)
            {
                if (thePC)
                    myDamage.damage = thePC.GetATTACK() * damageRatio;
                else
                    myDamage.damage = 30.0f;    //!!!!!!!!!!!!!!!!!!!!!!!!!!!! Skill 的 Damage 應該由 Caster 設定
                newBullet.InitValue(faction, myDamage, td, target);
            }
        }

        //發射特效
        foreach (GameObject fx in fireFXRefs)
        {
            Instantiate(fx, transform.position, Quaternion.Euler(90, 0, 0), transform);
        }

        if (theAnimator)
        {
            theAnimator.SetFloat("CastX", td.x);
#if XZ_PLAN
            theAnimator.SetFloat("CastY", td.z);
#else
            theAnimator.SetFloat("CastY", td.y);
#endif
            theAnimator.SetTrigger("Cast");
        }

        //thePC.DoUseMP(manaCost);
        //cdLeft = coolDown;
        result = SKILL_RESULT.SUCCESS;
        base.OnSkillSucess();
        return true; ;
    }


}
