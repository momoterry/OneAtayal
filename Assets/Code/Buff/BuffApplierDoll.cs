using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffApplierBase : MonoBehaviour
{
    protected Dictionary<DOLL_BUFF_TYPE, List<DollBuffBase>> buffPools = new Dictionary<DOLL_BUFF_TYPE, List<DollBuffBase>>();


    public void ApplyBuff(DollBuffBase buff)
    {
        if (!buffPools.ContainsKey(buff.type))
        {
            buffPools.Add(buff.type, new List<DollBuffBase>());
        }
        List<DollBuffBase> list = buffPools[buff.type];
        list.Add(buff);
        ApplyBuffEffect(buff.type, list);
    }

    public void DeApplyBuff(DollBuffBase buff)
    {
        if (buffPools.ContainsKey(buff.type))
        {
            List<DollBuffBase> list = buffPools[buff.type];
            if (list.Remove(buff)) 
            {
                ApplyBuffEffect(buff.type, list);
            }
            //else
            //{
            //    print("---- DeApplyBuff, No such buff to remove !!, my state");
            //}
            //TODO: 需要清掉空的 List 嗎?
        }
        else
        {
            print("ERROR: DeApplyBuff, No such Type exist !!");
        }
    }

    protected void ApplyBuffEffect( DOLL_BUFF_TYPE type, List<DollBuffBase> list)
    {
        float totalValue = 0;
        foreach (DollBuffBase buff in list)
        {
            totalValue += buff.value1;
        }

        switch (type)
        {
            case DOLL_BUFF_TYPE.ATTACK_SPEED:
                ApplyAttackSpeed(totalValue);
                break;
            case DOLL_BUFF_TYPE.DAMAGE:
                ApplyDamageRate(totalValue);
                break;
            case DOLL_BUFF_TYPE.HP:
                ApplayHPRate(totalValue);
                break;
        }
    }

    protected void ClearAllBuff()
    {
        foreach ( KeyValuePair<DOLL_BUFF_TYPE , List<DollBuffBase>> p in buffPools)
        {
            p.Value.Clear();
            ApplyBuffEffect(p.Key, p.Value);
        }
        //TODO: 需要清掉空的 List 嗎?
    }

    virtual protected void ApplyAttackSpeed( float percentAdd ) {}
    virtual protected void ApplyDamageRate( float percentAdd) {}
    virtual protected void ApplayHPRate( float percentAdd) {}

}

public class BuffApplierDoll : BuffApplierBase
{
    protected Doll myDoll;

    private void Awake()
    {
        //print("BuffApplierDoll Awake " + gameObject.name);
        myDoll = GetComponent<Doll>();
        if (!myDoll)
        {
            print("ERROR: NO Doll when BuffApplierDoll awake!!");
        }
    }

    protected override void ApplyAttackSpeed(float percentAdd)
    {
        base.ApplyAttackSpeed(percentAdd);
        //print("目前的加速 " + gameObject.name + " : " + value);
        myDoll.SetAttackSpeedRate(percentAdd * 0.01f + 1.0f);
    }

    protected override void ApplyDamageRate(float percentAdd) 
    {
        base.ApplyDamageRate(percentAdd);
        myDoll.SetDamageRate(percentAdd * 0.01f + 1.0f);
    }

    protected override void ApplayHPRate(float percentAdd)
    {
        base.ApplayHPRate(percentAdd);
        myDoll.SetHPRate(percentAdd * 0.01f + 1.0f);
    }

    //加入、移出隊伍時的動作
    public void OnJoinPlayer()
    {
        //取得目前的 TeamBuff
        TeamBuffManager m = BattleSystem.GetPC().theTeamBuff;
        if (m)
        {
            m.OnApplyBuffToOneDoll(myDoll);
        }
    }

    public void OnLeavePlayer()
    {
        ClearAllBuff();
    }
}
