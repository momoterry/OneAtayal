using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyBeta : �ઽ���z�L SkillBase �ӲզX�X�ޯ઺ AI �]�w
//
//

public class EnemyBeta : Enemy
{
    public SkillBase normalSkillRef;
    public SkillBase bigOneSkillRef;    //�j��

    public enum SKILL_TYPE
    {
        NORMALL,
        BIG_ONE,
    }
    public SKILL_TYPE[] skillPattern;

    protected SkillBase normalSkill;
    protected SkillBase bigOneSkill;

    protected int skillIndex = 0;
    protected float currSkillCDLeft = 0;

    protected override void Start()
    {
        base.Start();

        if (normalSkillRef)
        {
            GameObject o = Instantiate(normalSkillRef.gameObject, transform);
            normalSkill = o.GetComponent<SkillBase>();
            normalSkill.InitCasterInfo(gameObject, Attack);
        }
        if (bigOneSkillRef)
        {
            GameObject o = Instantiate(bigOneSkillRef.gameObject, transform);
            bigOneSkill = o.GetComponent<SkillBase>();
            bigOneSkill.InitCasterInfo(gameObject, Attack);
        }
    }


    protected SkillBase runningSkill = null;

    protected override void DoOneAttack()
    {
        //print("EnemyBeta----DoOneAttack");
        //base.DoOneAttack();

        //if (currSkillCDLeft > 0)
        //    return;
        if (runningSkill != null)
            return;

        SkillBase currSkill = null;
        switch (skillPattern[skillIndex])
        {
            case SKILL_TYPE.NORMALL:
                currSkill = normalSkill;
                break;
            case SKILL_TYPE.BIG_ONE:
                currSkill = bigOneSkill;
                break;

        }

        if (currSkill)
        {
            //print("----normalSkill.Play()");
            if (currSkill.Play())
            {
                runningSkill = currSkill;
            }
        }
    }

    protected void OnRunningSkillDone()
    {
        runningSkill = null;
        skillIndex++;
        if (skillIndex >= skillPattern.Length)
            skillIndex = 0;
    }

    //protected override void UpdateAttack()
    //{
    //    //print("EnemyBeta----UpdateAttack");
    //    base.UpdateAttack();

    //    //TODO: �Ʊ�� CD ��� AttackCD ���覡�T�O�b���ʮɤ]�|�p�� CD
    //    if (currSkillCDLeft > 0)
    //    {
    //        currSkillCDLeft -= Time.deltaTime;
    //    }
    //}

    protected override void PostUpdate()
    {
        base.PostUpdate();
        //if (currSkillCDLeft > 0)
        //{
        //    currSkillCDLeft -= Time.deltaTime;
        //}
        if (runningSkill != null)
        {
            if (runningSkill.IsReady())
            {
                OnRunningSkillDone();
            }
        }
    }

}
