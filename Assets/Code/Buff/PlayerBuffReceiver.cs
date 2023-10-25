using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffReceiver : BuffReceiver
{
    protected PlayerControllerBase pc;

    private void Awake()
    {
        pc = GetComponent<PlayerControllerBase>();
    }

    override protected void ApplyAttackSpeed(float percentAdd)
    {
        pc.SetAttackSpeedRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplyDamageRate(float percentAdd)
    {
        pc.SetDamageRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplayHPRate(float percentAdd)
    {
        pc.SetHPRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplayMoveSpeed(float percentAdd)
    {
        pc.SetMoveSpeedRate(percentAdd * 0.01f + 1.0f);
    }
}
