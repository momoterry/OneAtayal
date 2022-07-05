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
        if (pc)
        {
            pc.DoHeal(pc.GetHPMax() * healRatio + healAbsoluteValue);
        }

        HitBody body = obj.GetComponent<HitBody>();
        if (body)
        {
            body.DoHeal(body.GetHPMax() * healRatio + healAbsoluteValue);
        }

        if (healFX)
            BattleSystem.GetInstance().SpawnGameplayObject(healFX, obj.transform.position, obj.transform);
    }
}
