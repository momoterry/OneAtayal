using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SkillGroundHint
{
    public Vector2 size;
    public Vector3 posShift;
    public float angleShift;
}

//先用於 AI
[System.Serializable]
public struct SkillData
{
    public GameObject bulletRef;    //會給予方向的 Spawn 物件
    public float bulletInitDis;
    public bool fixDirection;       //不瞄準目標，全場散射類型適用
    public float prepareTime;       //前置 Idle
    //public bool isGroundHint;
    public SkillGroundHint[] groundHints;
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

    //技能的發招等待
    protected float prepareSkillTime = 0;
    protected SkillData prepareingSkill;
    protected Vector3 skillDirection;

    protected enum SKILL_PHASE
    {
        NORMAL,
        PREPARE,
        TRACING,
    }
    protected SKILL_PHASE currPhase = SKILL_PHASE.NORMAL;
    protected SKILL_PHASE nextPhase = SKILL_PHASE.NORMAL;
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

        skillDirection = theSkill.fixDirection ? Vector3.forward : faceDir.normalized;

        if (theSkill.prepareTime > 0)
        {
            nextPhase = SKILL_PHASE.PREPARE;
            prepareSkillTime = theSkill.prepareTime;
            prepareingSkill = theSkill;
            //if (theSkill.isGroundHint)
            foreach (SkillGroundHint groundHint in theSkill.groundHints)
            {
                float hintLength = groundHint.size.y;
                float hintWidth = groundHint.size.x;
                Vector3 hintDirection = Quaternion.Euler(0, groundHint.angleShift, 0) * skillDirection;
                Vector3 hintCenter = transform.position + hintDirection * hintLength * 0.5f;
                Vector3 hintShift = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, hintDirection, Vector3.up), 0) * groundHint.posShift;
                //if (!theSkill.fixDirection)
                //{
                //    hintShift = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, faceDir, Vector3.up), 0) * groundHint.posShift;
                //}
                GroundHintManager.GetInstance().ShowSquareHint(hintCenter + hintShift, hintDirection, new Vector2(hintWidth, hintLength), prepareSkillTime);
            }
        }
        else
        {
            DoOneSkill(theSkill);
            DoSkillDone();
        }

    }

    protected override void UpdateAttack()
    {
        if (isWaitTrack)        //TODO: 改用 Phase
        {
            if (myHook == null)
            {
                DoEndTrack();
            }
        }
        else
        {
            if (currPhase != nextPhase)
            {
                currPhase = nextPhase;
            }
            switch (currPhase) 
            {
                case SKILL_PHASE.NORMAL:
                    base.UpdateAttack();
                    break;
                case SKILL_PHASE.PREPARE:
                    prepareSkillTime -= Time.deltaTime;
                    if (prepareSkillTime <= 0)
                    {
                        DoOneSkill(prepareingSkill);
                        DoSkillDone();
                        nextPhase = SKILL_PHASE.NORMAL;
                    }
                    break;
            }
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
        Quaternion rm = Quaternion.Euler(90, 0, 0);

        if (skill.bulletRef)
        {
            Vector3 shootPoint = gameObject.transform.position + (skillDirection * skill.bulletInitDis);

            GameObject newObj = Instantiate(skill.bulletRef, shootPoint, rm, null);

            if (newObj)
            {
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {
                    Vector3 td = skillDirection;
                    myDamage.damage = Attack * skill.damageRatio;
                    newBullet.InitValue(FACTION_GROUP.ENEMY, myDamage, td);
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
                        myDamage.damage = Attack * skill.damageRatio;
                        newBullet.InitValue(FACTION_GROUP.ENEMY, myDamage, faceDir);
                    }
                }
            }
        }
    }

    protected void DoSkillDone()
    {
        AttackCD = skillPattern[currSkillIndex].collDown;
        stateTime = AttackCD;

        currSkillIndex++;
        if (currSkillIndex >= skillPattern.Length)
        {
            currSkillIndex = 0;
        }
    }

    //private void OnGUI()
    //{
    //    Vector2 thePoint = Camera.main.WorldToScreenPoint(transform.position + Vector3.forward);
    //    thePoint.y = Camera.main.pixelHeight - thePoint.y;
    //    GUI.TextArea(new Rect(thePoint, new Vector2(100.0f, 40.0f)), stateTime.ToString("F2") + " / " + AttackCD.ToString("F2"));

    //}

}
