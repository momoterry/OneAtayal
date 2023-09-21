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
            //TODO: �ݭn�M���Ū� List ��?
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
        }
    }

    protected void ClearAllBuff()
    {
        foreach ( KeyValuePair<DOLL_BUFF_TYPE , List<DollBuffBase>> p in buffPools)
        {
            p.Value.Clear();
            ApplyBuffEffect(p.Key, p.Value);
        }
        //TODO: �ݭn�M���Ū� List ��?
    }

    virtual protected void ApplyAttackSpeed( float value ) {}

}

public class BuffApplierDoll : BuffApplierBase
{
    public Doll myDoll;

    //protected float originalAttackCD;
    //private void Start()
    //{
    //    myDoll = GetComponent<Doll>();
    //    if (!myDoll)
    //    {
    //        print("ERROR!!!! BuffApplierDoll Start without DollAuto");
    //        return;
    //    }

    //    //originalAttackCD = myDoll.attackCD;
    //}

    protected override void ApplyAttackSpeed(float value)
    {
        base.ApplyAttackSpeed(value);
        //print("�ثe���[�t " + gameObject.name + " : " + value);
        myDoll.SetAttackSpeedRate(value * 0.01f + 1.0f);
    }

    //�[�J�B���X����ɪ��ʧ@
    public void OnJoinPlayer()
    {
        //���o�ثe�� TeamBuff
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
