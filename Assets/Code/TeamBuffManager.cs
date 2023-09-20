using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBuffManager : MonoBehaviour
{
    protected List<DollBuffBase> toAllList = new List<DollBuffBase>();
    protected List<DollBuffBase> toFrontList = new List<DollBuffBase>();
    protected List<DollBuffBase> toMiddleList = new List<DollBuffBase>();
    protected List<DollBuffBase> toBackleList = new List<DollBuffBase>();

    protected List<DollBuffBase> listToApply = new List<DollBuffBase>();
    protected List<DollBuffBase> listToDeApply = new List<DollBuffBase>();

    protected List<DollBuffBase> GetListByTargetType(DOLL_BUFF_TARGET target)
    {
        switch (target)
        {
            case DOLL_BUFF_TARGET.FRONT:
                return toFrontList;
            case DOLL_BUFF_TARGET.MIDDLE:
                return toMiddleList;
            case DOLL_BUFF_TARGET.BACK:
                return toBackleList;
            case DOLL_BUFF_TARGET.ALL:
                return toAllList;
        }
        return null;
    }

    protected List<DollBuffBase> GetListByPositionType(DOLL_POSITION_TYPE pos)
    {
        switch (pos)
        {
            case DOLL_POSITION_TYPE.FRONT:
                return toFrontList;
            case DOLL_POSITION_TYPE.MIDDLE:
                return toMiddleList;
            case DOLL_POSITION_TYPE.BACK:
                return toBackleList;
        }
        return null;
    }

    protected void DoApplyTeamBuff(DollBuffBase buff)
    {
        List<Doll> dollList = BattleSystem.GetPC().GetDollManager().GetActiveDolls();
        DOLL_POSITION_TYPE targetPositionType = GetTargetPositionType(buff.target);
        print("�W�[�@�ӹζ� Buff " + buff.type);
        foreach (Doll doll in dollList)
        {
            if (buff.target == DOLL_BUFF_TARGET.ALL || targetPositionType == doll.positionType)
            {
                BuffApplierDoll buffApplier = doll.GetComponent<BuffApplierDoll>();
                if (buffApplier == null)
                {
                    buffApplier = doll.gameObject.AddComponent<BuffApplierDoll>();
                }
                print("�ζ� Buff " + buff.type + "�I�[��: " + doll.name);
                buffApplier.ApplyBuff(buff);
            }
        }

        List<DollBuffBase> list = GetListByTargetType(buff.target);
        list.Add(buff);
    }

    protected void DoDeApplyTeamBuff(DollBuffBase buff) 
    {
        List<Doll> dollList = BattleSystem.GetPC().GetDollManager().GetActiveDolls();
        DOLL_POSITION_TYPE targetPositionType = GetTargetPositionType(buff.target);
        foreach (Doll doll in dollList)
        {
            if (buff.target == DOLL_BUFF_TARGET.ALL || targetPositionType == doll.positionType)
            {
                BuffApplierDoll buffApplier = doll.GetComponent<BuffApplierDoll>();
                if (buffApplier == null)
                {
                    print("ERROR: remove a buff form a doll without BuffApplierDoll !! ");
                    continue;
                }
                buffApplier.DeApplyBuff(buff);
            }
        }

        List<DollBuffBase> list = GetListByTargetType(buff.target);
        list.Remove(buff);
    }

    //Team Buff ���[�J�M�����A���ߤ@�� Frame �A�ާ@�A�H�K����[�J�� Doll �X���D
    public void AddTeamBuff(DollBuffBase buff)
    {
        listToApply.Add(buff);
    }

    public void RemoveTeamBuff( DollBuffBase buff) 
    {
        listToDeApply.Add(buff);
    }

    public void Update()
    {
        if (listToApply.Count > 0)
        {
            foreach (DollBuffBase buff in listToApply)
            {
                DoApplyTeamBuff(buff);
            }
            listToApply.Clear();
        }
        if (listToDeApply.Count > 0)
        {
            foreach (DollBuffBase buff in listToDeApply)
            {
                DoDeApplyTeamBuff(buff);
            }
            listToDeApply.Clear();
        }
    }

    //�w��s�[�J�� Doll�A�o��Ҧ��� Buff
    public void OnApplyBuffToOneDoll( Doll doll)
    {
        BuffApplierDoll buffApplier = doll.GetComponent<BuffApplierDoll>();
        if (buffApplier == null)
        {
            buffApplier = doll.gameObject.AddComponent<BuffApplierDoll>();
        }
        foreach (DollBuffBase buff in toAllList)
        {
            buffApplier.ApplyBuff(buff);
        }
        List<DollBuffBase> list = GetListByPositionType(doll.positionType);
        foreach (DollBuffBase buff in list)
        {
            buffApplier.ApplyBuff(buff);
        }
    }

    protected DOLL_POSITION_TYPE GetTargetPositionType( DOLL_BUFF_TARGET target)
    {
        switch (target)
        {
            case DOLL_BUFF_TARGET.FRONT:
                return DOLL_POSITION_TYPE.FRONT;
            case DOLL_BUFF_TARGET.MIDDLE:
                return DOLL_POSITION_TYPE.MIDDLE;
            case DOLL_BUFF_TARGET.BACK:
                return DOLL_POSITION_TYPE.BACK;
        }
        return DOLL_POSITION_TYPE.BACK;
    }

}
