using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyBeta : �ઽ���z�L SkillBase �ӲզX�X�ޯ઺ AI �]�w
//
//

public class EnemyBeta : Enemy
{
    public SkillBase normalSkillRef;

    protected SkillBase normalSkill;

    protected override void Start()
    {
        base.Start();

        if (normalSkillRef)
        {
            GameObject o = Instantiate(normalSkillRef.gameObject, transform);
            normalSkill = o.GetComponent<SkillBase>();
            normalSkill.InitCasterInfo(gameObject);
        }
    }

    protected override void DoOneAttack()
    {
        //print("EnemyBeta----DoOneAttack");
        //base.DoOneAttack();

        if (normalSkill)
        {
            //print("----normalSkill.DoStart()");
            normalSkill.DoStart();
        }
    }

    protected override void UpdateAttack()
    {
        //print("EnemyBeta----UpdateAttack");
        base.UpdateAttack();
    }
}
