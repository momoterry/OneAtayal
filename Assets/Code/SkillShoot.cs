using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShoot : SkillBase
{
    public GameObject bulletRef;
    public float bulletInitDis = 0.25f;
    public float searchRange = 10.0f;
    // Start is called before the first frame update

    protected PlayerControllerBase thePC;
    protected Animator theAnimator;

    public override void InitCasterInfo(GameObject oCaster)
    {
        base.InitCasterInfo(oCaster);

        thePC = oCaster.GetComponent<PlayerControllerBase>();
        theAnimator = oCaster.GetComponent<Animator>();
    }

    protected GameObject FindBestShootTarget(float searchRange)
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

    public override bool DoStart()
    {
        //print("SkillShoot!!!!!! ");
        GameObject target = FindBestShootTarget(searchRange);
        if (target == null)
            return false;

       // PlayerControllerBase thePC = theCaster.GetComponent<PlayerControllerBase>();
        if (thePC == null)
            return false;

        if (thePC.GetMP() < manaCost)
        {
            print("Mana 不夠啦 !!");
            return false;
        }
        thePC.DoUseMP(manaCost);

        Vector3 td = target.transform.position - theCaster.transform.position;
#if XZ_PLAN
        td.y = 0;
#else
        td.z = 0;
#endif

        td.Normalize();
        thePC.SetupFaceDir(td);

        //TODO 發射點靠前量參數化?
        Vector3 shootPos = gameObject.transform.position + td * bulletInitDis;

        GameObject newObj = BattleSystem.GetInstance().SpawnGameplayObject(bulletRef, shootPos, false);
        if (newObj)
        {
            bullet newBullet = newObj.GetComponent<bullet>();
            if (newBullet)
            {
                newBullet.InitValue(DAMAGE_GROUP.PLAYER, thePC.GetATTACK() * damageRatio, td);
            }
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


        return true; ;
    }


}
