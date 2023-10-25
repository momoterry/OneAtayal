using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollBuffReceiver : BuffReceiver
{
    protected Doll doll;

    private void Awake()
    {
        doll = GetComponent<Doll>();
    }

    override protected void ApplyAttackSpeed(float percentAdd)
    {
        doll.SetAttackSpeedRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplyDamageRate(float percentAdd)
    {
        doll.SetDamageRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplayHPRate(float percentAdd)
    {
        doll.SetHPRate(percentAdd * 0.01f + 1.0f);
    }

    protected override void ApplayMoveSpeed(float percentAdd)
    {
        doll.SetMoveSpeedRate(percentAdd * 0.01f + 1.0f);
    }
}
