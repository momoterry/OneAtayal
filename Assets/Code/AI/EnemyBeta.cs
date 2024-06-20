using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyBeta : 能直接透過 SkillBase 來組合出技能的 AI 設定
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
