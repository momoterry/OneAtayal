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
        print("EnemyOne DoOneAttack!!");

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
            TrackHook th = gameObject.AddComponent<TrackHook>();
            if (th)
            {
                th.trackRef = skill.trackRef;
                th.StartAtBegin = true;
            }
        }
    }

}
