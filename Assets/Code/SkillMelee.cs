using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMelee : SkillBase
{
    public GameObject meleeObject;
    public float meleeCenterDis = 1.0f;

    //protected PlayerControllerBase thePC;
    //protected Animator theAnimator;

    //public override void InitCasterInfo(GameObject oCaster)
    //{
    //    base.InitCasterInfo(oCaster);

    //    thePC = oCaster.GetComponent<PlayerControllerBase>();
    //    theAnimator = oCaster.GetComponent<Animator>();
    //}

    public override bool DoStart(ref SKILL_RESULT result)
    {
        //基本檢查
        if (!base.DoStart(ref result))
            return false;

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

        Vector3 td = thePC.GetFaceDir();

        Vector3 meleePos = theCaster.transform.position + td * meleeCenterDis;

        //角度
        //float meleeAngle = Vector3.Angle(Vector3.forward, td);
        float meleeAngle = 0;
        FaceFrontType ft = thePC.GetFaceFront();
        switch (ft)
        {
            case FaceFrontType.UP:
                meleeAngle = 0;
                break;
            case FaceFrontType.RIGHT:
                meleeAngle = -90.0f;
                break;
            case FaceFrontType.DOWN:
                meleeAngle = 180.0f;
                break;
            case FaceFrontType.LEFT:
                meleeAngle = 90.0f;
                break;
        }
        Quaternion qm = Quaternion.Euler(90, -meleeAngle, 0);


        //GameObject newObj = BattleSystem.GetInstance().SpawnGameplayObject(meleeObject, shootPos, false);
        GameObject newObj = Instantiate(meleeObject, meleePos, qm, null);
        if (newObj)
        {
            bullet_base newBullet = newObj.GetComponent<bullet_base>();
            if (newBullet)
            {
                newBullet.InitValue(DAMAGE_GROUP.PLAYER, thePC.GetATTACK() * damageRatio, td);
            }
        }

        if (theAnimator)
        {
            theAnimator.SetFloat("CastX", td.x);
            theAnimator.SetFloat("CastY", td.z);

            theAnimator.SetTrigger("Cast");
        }

        //thePC.DoUseMP(manaCost);
        //cdLeft = coolDown;
        result = SKILL_RESULT.SUCCESS;
        base.OnSkillSucess();
        return true; ;
    }
}
