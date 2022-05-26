using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//先用於 AI
[System.Serializable]
public struct SkillData
{
    public GameObject bulletRef;    //會給予方向的 Spawn 物件
    public float bulletInitDis;
    public float damageRatio;
    public string animString;

    public GameObject trackRef;     //如果要使用 Track Animation
    public string trackAnimStr;
    public GameObject trackDamageFX;      
}
[System.Serializable]
public class SkillPatternInfo
{
    public int skillIndex = 0;
    public float collDown = 1.0f;
}


public class EnemyOne : Enemy
{
    public SkillData[] skillList;
    public SkillPatternInfo[] skillPattern;

    protected int currSkillIndex = 0;

    //Track Animation 用
    protected TrackHookForAgent myHook = null;
    protected bool isWaitTrack = false;
    protected string trackAnimationStr = null;
    protected GameObject myTrackDamageFX = null;


    protected override void Start()
    {
        base.Start();
        if (skillList.Length <= 0 || skillPattern.Length <= 0)
        {
            print("ERROR!!!! OSE_Sequence Invalid Data !!!!!");
            currSkillIndex = -1;
        }
    }

    protected override void DoOneAttack()
    {
        if (currSkillIndex < 0)
            return;
        //print("EnemyOne DoOneAttack!!");

        int id = skillPattern[currSkillIndex].skillIndex;
        if (id < 0 || id >= skillList.Length)
        {
            print("ERROR!!!! Invalid Skill Index In Pattern");
        }

        SkillData theSkill = skillList[skillPattern[currSkillIndex].skillIndex];

        //coolDown = skillPattern[currSPIndex].collDown;

        DoOneSkill(theSkill);

        AttackCD = skillPattern[currSkillIndex].collDown;

        currSkillIndex++;
        if (currSkillIndex >= skillPattern.Length)
        {
            currSkillIndex = 0;
        }
    }

    protected override void UpdateAttack()
    {
        if (isWaitTrack)
        {
            if (myHook == null)
            {
                DoEndTrack();
            }
        }
        else
        {
            base.UpdateAttack();
        }
    }

    protected void DoEndTrack()
    {
        // Track 結束處理
        if (myAnimator && trackAnimationStr != "")
        {
            myAnimator.SetBool(trackAnimationStr, false);
        }
        if (myTrackDamageFX)
        {
            Destroy(myTrackDamageFX);
        }
        isWaitTrack = false;
    }


    protected void DoOneSkill(SkillData skill)
    {
#if XZ_PLAN
        Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
        Quaternion rm = Quaternion.identity;
#endif
        if (skill.bulletRef)
        {
            Vector3 shootPoint = gameObject.transform.position + faceDir * skill.bulletInitDis;

            GameObject newObj = Instantiate(skill.bulletRef, shootPoint, rm, null);

            if (newObj)
            {
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {
                    Vector3 td = targetObj.transform.position - newObj.transform.position;
#if XZ_PLAN
                    td.y = 0;
#else
                    td.z = 0;
#endif
                    newBullet.InitValue(DAMAGE_GROUP.ENEMY, Attack*skill.damageRatio, td);
                }
            }
        }

        if (myAnimator && skill.animString != "")
        {
            myAnimator.SetTrigger(skill.animString);
        }

        if (skill.trackRef)
        {
            myHook = gameObject.AddComponent<TrackHookForAgent>();
            if (myHook)
            {
                myHook.trackRef = skill.trackRef;
                myHook.StartAtBegin = true;
                myHook.hookAgent = myAgent;
                isWaitTrack = true;
                if ( myAnimator && skill.trackAnimStr != "")
                {
                    trackAnimationStr = skill.trackAnimStr;
                    myAnimator.SetBool(trackAnimationStr, true);
                }
                if (skill.trackDamageFX)
                {
                    myTrackDamageFX = BattleSystem.GetInstance().SpawnGameplayObject(skill.trackDamageFX, transform.position, false);
                    myTrackDamageFX.transform.parent = transform;
                    bullet_base newBullet = myTrackDamageFX.GetComponent<bullet_base>();
                    if (newBullet)
                    {
                        newBullet.InitValue(DAMAGE_GROUP.ENEMY, Attack * skill.damageRatio, faceDir);
                    }
                }
            }
        }
    }

}
