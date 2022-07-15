using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : AreaEffectBase
{
    public float defaultDamage = 20.0f;
    public GameObject damageFX;

    protected override void ApplyEffect(GameObject obj)
    {
        //float healAbsoluteValue = baseDamage;

        Damage myDamage;
        myDamage.damage = defaultDamage;

        //PlayerControllerBase pc = obj.GetComponent<PlayerControllerBase>();
        //if (pc)
        //{
        //    //pc.DoHeal(pc.GetHPMax() * healRatio + healAbsoluteValue);
        //}

        //HitBody body = obj.GetComponent<HitBody>();
        //if (body)
        //{
        //    //body.DoHeal(body.GetHPMax() * healRatio + healAbsoluteValue);
        //    obj.SendMessage("OnDamage", myDamage)
        //}

        obj.SendMessage("OnDamage", myDamage);

        if (damageFX)
            BattleSystem.GetInstance().SpawnGameplayObject(damageFX, obj.transform.position, obj.transform);
    }
}
