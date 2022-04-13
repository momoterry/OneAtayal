using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHealer : BulletTrace
{
    public float healRatio = 0.1f;
    public float healAbsoluteValue = 20.0f;
    public GameObject healFX;

    // Start is called before the first frame update
    protected override void DoHitTarget()
    {

        PlayerController pc = targetObj.GetComponent<PlayerController>();
        if (pc)
        {
            pc.DoHeal(pc.GetHPMax() * healRatio + healAbsoluteValue);
        }

        HitBody body = targetObj.GetComponent<HitBody>();
        if (body)
        {
            body.DoHeal(body.GetHPMax() * healRatio + healAbsoluteValue);
        }

        if (healFX)
        {
#if XZ_PLAN
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rm = Quaternion.identity;
#endif
            Instantiate(healFX, targetObj.transform.position, rm, targetObj.transform);
        }

        Destroy(gameObject);
    }
}
