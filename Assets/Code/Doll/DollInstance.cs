using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
//  ���F��y������G�A�۷����y�᪺���G
//  DollInstance ����w Doll �����A�P�ɳs���@�� DollBuff
//  �����s���� Doll ������W
//============================================================



public class DollInstance : MonoBehaviour
{
    public string fullName;
    public Doll theDoll;
    protected List<DollBuffBase> buffList = new List<DollBuffBase>();

    public List<DollBuffBase> GetBuffList() { return buffList; }

    public void Init( string _name, Doll _doll)
    {
        fullName = _name;
        theDoll = _doll;
    }

    public void AddBuff( DollBuffBase buff)
    {
        buffList.Add(buff);
    }

    public void OnJoinPlayer()
    {
        ActiveAllBuff();
    }

    public void OnLeavePlayer()
    {
        DeActiveAllBuff();
    }

    //�}�l�ҰʩҦ� Buff�A���ӬO�b Doll �[�J�����}�l�@��
    protected void ActiveAllBuff()
    {
        TeamBuffManager m = BattleSystem.GetPC().theTeamBuff;
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:ActiveAllBuff -- �٨S����@�� target :" + buff.target);
                    break;
                default:
                    m.AddTeamBuff(buff);
                    break;
            }
        }
    }

    //�����Ҧ��� Buff �A���ӬO�b Doll ���`�����}����ɨϥ�
    protected void DeActiveAllBuff()
    {
        TeamBuffManager m = BattleSystem.GetPC().theTeamBuff;
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:DeActiveAllBuff -- �٨S����@�� target :" + buff.target);
                    break;
                default:
                    m.RemoveTeamBuff(buff);
                    break;
            }
        }
    }

    public DollInstanceData ToData()
    {
        DollInstanceData data = new DollInstanceData();
        data.fullName = fullName;
        data.buffs = new DollBuffData[buffList.Count];
        for (int i=0; i<buffList.Count; i++)
        {
            data.buffs[i].buffType = (int)buffList[i].type;
            data.buffs[i].buffTarget = (int)buffList[i].target;
            data.buffs[i].buffValue1 = (int)buffList[i].value1;
        }
        return data;
    }

}
