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
    public int uID;
    protected List<DollBuffBase> buffList = new List<DollBuffBase>();

    public List<DollBuffBase> GetBuffList() { return buffList; }

    //�ͦ����s DI �ɨϥΡA�ݭn���ͷs�� ID
    public void InitBySpawn( string _name, Doll _doll, int _uID)
    {
        uID = _uID;
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
        data.uID = uID;
        data.baseDollID = theDoll.ID;
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

    public void InitFromData(DollInstanceData data, Doll _doll)
    {
        uID = data.uID;
        fullName = data.fullName;
        theDoll = _doll;
        
        for (int i=0; i<data.buffs.Length; i++)
        {
            //print("Add Buff " + i + " - " + data.buffs[i].buffType);
            buffList.Add(DollBuffBase.GenerateFromData(data.buffs[i]));
        }
    }

    static public GameObject SpawnDollFromData( DollInstanceData data, Vector3 pos)
    {
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(data.baseDollID);
        if (!dollRef)
        {
            print("ERROR!!!! No Doll Ref called: " + data.baseDollID);
            return null;
        }
        GameObject o = BattleSystem.SpawnGameObj(dollRef, pos);
        Doll d = o.GetComponent<Doll>();
        DollInstance di = o.AddComponent<DollInstance>();
        di.InitFromData(data, d);

        return o;
    }

}
