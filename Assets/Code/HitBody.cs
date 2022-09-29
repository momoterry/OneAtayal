using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealData
{
    public float healAbsoluteValue;
    public float healRatio;
    public float healResult;
}
public class HitBody : MonoBehaviour
{
    // Start is called before the first frame update
    public float HP_Max = 100.0f;
    public float DamageRatio = 1.0f;
    // 
    protected float hp;

    Hp_BarHandler myHPHandler;

    // Public Get Function
    public float GetHPMax() { return HP_Max; }
    public float GetHP() { return hp; }

    void Start()
    {
        hp = HP_Max;
        myHPHandler = GetComponent<Hp_BarHandler>();
    }

    private void Update()
    {
        if (myHPHandler )
        {
            myHPHandler.SetHP(hp, HP_Max);
        }
    }

    void OnDamage(Damage theDamage)
    {
        hp -= theDamage.damage * DamageRatio;
        if (hp <= 0)
        {
            gameObject.SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void DoHeal(float healNum)
    {
        hp += healNum;
        if (hp > HP_Max)
            hp = HP_Max;
    }

    public float DoHeal(float healAbsoluteNum, float healRatio)
    {
        float newHp = hp + healAbsoluteNum + HP_Max * healRatio;
        if (newHp >= HP_Max)
            newHp = HP_Max;
        float healResult = newHp - hp;
        hp = newHp;
        return healResult;
    }
}
