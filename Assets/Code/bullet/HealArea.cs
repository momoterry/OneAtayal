using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealArea : AreaEffectBase
{
    public float healRatio = 0.02f;
    public GameObject healFX;

    protected override void ApplyEffect(GameObject obj)
    {
        float healAbsoluteValue = baseDamage;
        PlayerControllerBase pc = obj.GetComponent<PlayerControllerBase>();
        float healResult = 0;
        if (pc)
        {
            //pc.DoHeal(pc.GetHPMax() * healRatio + healAbsoluteValue);
            healResult = pc.DoHeal(healAbsoluteValue, healRatio);
        }

        HitBody body = obj.GetComponent<HitBody>();
        if (body)
        {
            //body.DoHeal(body.GetHPMax() * healRatio + healAbsoluteValue);
            healResult = body.DoHeal(healAbsoluteValue, healRatio);
        }

        //print("healResult!!! " + healResult);
        if (healFX && healResult > 0)
            BattleSystem.GetInstance().SpawnGameplayObject(healFX, obj.transform.position, obj.transform);
    }
}
