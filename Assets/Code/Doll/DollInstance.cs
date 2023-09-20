using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
//���F��y������G�A�۷����y�᪺���G
//DollInstance ����w Doll �����A�P�ɳs���@�� DollBuff
//�����s���� Doll ������W
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
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:ActiveAllBuff -- �٨S����@�� target :" + buff.target);
                    break;
                default:
                    TeamBuffManager.AddTeamBuff(buff);
                    break;
            }
        }
    }

    //�����Ҧ��� Buff �A���ӬO�b Doll ���`�����}����ɨϥ�
    protected void DeActiveAllBuff()
    {
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:DeActiveAllBuff -- �٨S����@�� target :" + buff.target);
                    break;
                default:
                    TeamBuffManager.RemoveTeamBuff(buff);
                    break;
            }
        }
    }


}
