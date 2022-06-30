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

        Collider[] cols = Physics.OverlapSphere(transform.position, searchRange, LayerMask.GetMask("Character"));

        GameObject bestEnemy = null;
        float bestSDis = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                Vector3 vDis = col.transform.position - theCaster.transform.position;
                float sDis = vDis.sqrMagnitude;
                if (sDis < bestSDis)
                {
                    bestEnemy = col.gameObject;
                    bestSDis = sDis;
                }
            }
        }

        return bestEnemy;
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
            td = thePC.GetFaceDir();

#if XZ_PLAN
        td.y = 0;
#else
        td.z = 0;
#endif

        td.Normalize();
        if (faceTarget)
            thePC.SetupFaceDir(td);

        Vector3 shootPos = theCaster.transform.position + td * bulletInitDis;

        GameObject newObj = BattleSystem.GetInstance().SpawnGameplayObject(bulletRef, shootPos, false);
        if (newObj)
        {
            bullet_base newBullet = newObj.GetComponent<bullet_base>();
            if (newBullet)
            {
                newBullet.InitValue(DAMAGE_GROUP.PLAYER, thePC.GetATTACK() * damageRatio, td, target);
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
