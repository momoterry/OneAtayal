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
    //const float HITTABLE_TARGET_RANGE = 4.0f;
    // Start is called before the first frame update
    public float HP_Max = 100.0f;
    public float DamageRatio = 1.0f;
    public bool hittableObj = false;
    //protected float rangeLimit = Mathf.Infinity;    //必須在這個距離內才可以被自動攻擊
    // 
    protected float hp;

    Hp_BarHandler myHPHandler;

    // Public Get Function
    public float GetHPMax() { return HP_Max; }
    public float GetHP() { return hp; }

    //public float GetRangeLimint() { return rangeLimit; }

    //private void Awake()
    //{
    //    if (hittableObj)
    //        rangeLimit = HITTABLE_TARGET_RANGE;
    //}

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

    public void OnDamage(Damage theDamage)
    {
        hp -= theDamage.damage * DamageRatio *BattleSystem.GetAllEnemyDamageRate();
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
