using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffReceiver : MonoBehaviour
{
    protected Dictionary<BUFF_TYPE, List<BuffBase>> buffPools = new Dictionary<BUFF_TYPE, List<BuffBase>>();

    protected GameObject groundFX;
    protected GameObject groundFXRef;   //用來比對
    protected float groundFXshift = -0.5f;

    public void AddGroundEffect(GameObject FXref)
    {
        if (groundFXRef == FXref)
            return;

        if (groundFX)
        {
            Destroy(groundFX);
        }

        groundFXRef = FXref;
        Vector3 sPos = new Vector3(0, 0, groundFXshift);
        groundFX = BattleSystem.SpawnGameObj(FXref, transform.position + sPos);
        groundFX.transform.parent = transform;
    }

    public void RemoveGroundEffect(GameObject FXref)
    {
        if (groundFXRef != FXref)
            return;
        if (groundFX)
        {
            Destroy(groundFX);
            groundFX = null;
        }
        groundFXRef = null;
    }

    public void AddBuff( BuffBase buff)
    {
        print("AddBuff: " + buff.type + " -- " + buff.value);
        if (!buffPools.ContainsKey(buff.type))
        {
            buffPools.Add(buff.type, new List<BuffBase>());
        }
        List<BuffBase> list = buffPools[buff.type];
        list.Add(buff);
        ApplyBuffEffect(buff.type, list);
    }


    public void RemoveBuff(BuffBase buff)
    {
        print("RemoveBuff: " + buff.type + " -- " + buff.value);
        if (buffPools.ContainsKey(buff.type))
        {
            List<BuffBase> list = buffPools[buff.type];
            if (list.Remove(buff))
            {
                ApplyBuffEffect(buff.type, list);
            }
        }
        else
        {
            print("ERROR: RemoveBuff, No such Type exist !!");
        }
    }

    protected void ApplyBuffEffect(BUFF_TYPE type, List<BuffBase> list)
    {
        float totalValue = 0;
        foreach (BuffBase buff in list)
        {
            totalValue += buff.value;
        }

        switch (type)
        {
            case BUFF_TYPE.ATTACK_SPEED:
                ApplyAttackSpeed(totalValue);
                break;
            case BUFF_TYPE.DAMAGE:
                ApplyDamageRate(totalValue);
                break;
            case BUFF_TYPE.HP:
                ApplayHPRate(totalValue);
                break;
        }
    }

    virtual protected void ApplyAttackSpeed(float percentAdd) { }
    virtual protected void ApplyDamageRate(float percentAdd) { }
    virtual protected void ApplayHPRate(float percentAdd) { }

}
