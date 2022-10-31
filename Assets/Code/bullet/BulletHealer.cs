using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHealer : BulletTrace
{
    public float healRatio = 0.1f;
    //public float healAbsoluteValue = 20.0f;
    //public float heal
    public GameObject healFX;

    protected float predictedHealValue = 0;
    protected PreHealInfo myPreHeal;


    public override void InitValue(DAMAGE_GROUP g, float damage, Vector3 targetVec, GameObject targetObject = null)
    {
        base.InitValue(g, damage, targetVec, targetObject);

        predictedHealValue = 0;
        if (targetObj && targetObj.activeInHierarchy)
        {
            PlayerControllerBase pc = targetObj.GetComponent<PlayerControllerBase>();
            if (pc)
            {
                predictedHealValue = (pc.GetHPMax() * healRatio) + baseDamage;
            }

            HitBody body = targetObj.GetComponent<HitBody>();
            if (body)
            {
                predictedHealValue = (body.GetHPMax() * healRatio) + baseDamage;
            }
        }

        if (predictedHealValue >= 0)
        {
            myPreHeal = targetObj.GetComponent<PreHealInfo>();
            if (!myPreHeal)
            {
                myPreHeal = targetObj.AddComponent<PreHealInfo>();
            }
            myPreHeal.AddPreHeal(predictedHealValue);
        }
    }

    protected override void DoHitTarget()
    {
        if (targetObj.activeInHierarchy)
        {
            //float healAbsoluteValue = baseDamage;
            PlayerControllerBase pc = targetObj.GetComponent<PlayerControllerBase>();
            if (pc)
            {
                //pc.DoHeal(pc.GetHPMax() * healRatio + healAbsoluteValue);
                pc.DoHeal(predictedHealValue);
            }

            HitBody body = targetObj.GetComponent<HitBody>();
            if (body)
            {
                //body.DoHeal(body.GetHPMax() * healRatio + healAbsoluteValue);
                body.DoHeal(predictedHealValue);
            }

            if (healFX)
            {
                BattleSystem.GetInstance().SpawnGameplayObject(healFX, targetObj.transform.position, false);
            }
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (myPreHeal)
        {
            myPreHeal.AddPreHeal(-predictedHealValue);
        }
    }

}
