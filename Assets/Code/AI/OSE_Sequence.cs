using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//射擊專用
[System.Serializable]
public class OSE_SkillData
{
    public GameObject bulletRef;  //會給予方向的 Spawn 物件
    public float bulletInitDis = 0.5f;
    public float damageRatio = 1.0f;
    public float angleShift = 0;
}

[System.Serializable]
public class OSE_PatternInfo
{
    public int skillIndex = 0;
    public float collDown = 1.0f; 
}

public class OSE_Sequence : OSEnemy
{
    public OSE_SkillData[] skillList;
    public OSE_PatternInfo[] skillPattern;

    protected int currSPIndex = 0;
    protected float coolDown = 0;

    protected override void StartBattle()
    {
        base.StartBattle();

        if (skillList.Length <=0 || skillPattern.Length <= 0)
        {
            print("ERROR!!!! OSE_Sequence Invalid Data !!!!!");
            currSPIndex = -1;
        }
    }

    protected override void UpdateBattle()
    {
        coolDown -= Time.deltaTime;
        if (coolDown <= 0)
        {
            DoOnePattern();
        }
        base.UpdateBattle();
    }

    protected void DoOnePattern()
    {
        if (currSPIndex <0)
        {
            return;
        }

        int id = skillPattern[currSPIndex].skillIndex;
        if (id < 0 || id >= skillList.Length)
        {
            print("ERROR!!!! Invalid Skill Index In Pattern");
        }

        OSE_SkillData theSkill = skillList[skillPattern[currSPIndex].skillIndex];
        DoOneSkill(theSkill);

        coolDown = skillPattern[currSPIndex].collDown;
        currSPIndex++;
        if (currSPIndex >= skillPattern.Length)
            currSPIndex = 0;
    }


    protected void DoOneSkill(OSE_SkillData skill)
    {
        Quaternion rm = Quaternion.Euler(90, 0, 0);
        Quaternion rM = Quaternion.AngleAxis(skill.angleShift, Vector3.up);
        Vector3 faceDir = rM * Vector3.back;

        //TODO: 角度修正

        if (skill.bulletRef)
        {
            Vector3 shootPoint = gameObject.transform.position + faceDir * skill.bulletInitDis;

            GameObject newObj = Instantiate(skill.bulletRef, shootPoint, rm, null);

            if (newObj)
            {
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {
                    newBullet.InitValue(DAMAGE_GROUP.ENEMY, Attack * skill.damageRatio, faceDir);
                }
            }
        }
    }
}
