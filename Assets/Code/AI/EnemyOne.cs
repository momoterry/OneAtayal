using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//先用於 AI
[System.Serializable]
public struct SkillData
{
    public GameObject bulletRef;  //會給予方向的 Spawn 物件
    public float bulletInitDis;
    public float damageRatio;
    public string animString;
}



public class EnemyOne : Enemy
{
    public SkillData[] skillList;

    protected int currSkillIndex = 0;


    protected override void DoOneAttack()
    {
        print("EnemyOne DoOneAttack!!");
        if (skillList.Length > 0)
        {
            DoOneSkill(skillList[currSkillIndex]);
            currSkillIndex++;
            if (currSkillIndex >= skillList.Length)
            {
                currSkillIndex = 0;
            }
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

        if (myAnimator && skill.animString != null)
        {
            myAnimator.SetTrigger(skill.animString);
        }
    }

}
