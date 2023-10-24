using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffReceiver : BuffReceiver
{
    protected Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    override protected void ApplyAttackSpeed(float percentAdd) 
    {
        enemy.SetAttackSpeedRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplyDamageRate(float percentAdd) 
    {
        enemy.SetDamageRate(percentAdd * 0.01f + 1.0f);
    }

    override protected void ApplayHPRate(float percentAdd) 
    {
        enemy.SetHPRate(percentAdd * 0.01f + 1.0f);
    }
}
