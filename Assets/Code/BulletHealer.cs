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
        //TODO: 改成也可以治療 Doll
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        pc.DoHeal(pc.GetHPMax() * healRatio + healAbsoluteValue);
        if (healFX)
        {
#if XZ_PLAN
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rm = Quaternion.identity;
#endif
            Instantiate(healFX, pc.transform.position, rm, pc.transform);
        }

        Destroy(gameObject);
    }
}
