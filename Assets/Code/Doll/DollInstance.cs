using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
//  巫靈鍛造機制的成果，相當於鍛造後的結果
//  DollInstance 能指定 Doll 種類，同時連結一串 DollBuff
//  直接連結到 Doll 的物件上
//============================================================



public class DollInstance : MonoBehaviour
{
    public string fullName;
    public Doll theDoll;
    public int uID;
    protected List<DollBuffBase> buffList = new List<DollBuffBase>();

    public List<DollBuffBase> GetBuffList() { return buffList; }

    //生成全新 DI 時使用，需要產生新的 ID
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

    //開始啟動所有 Buff，應該是在 Doll 加入隊伍後開始作用
    protected void ActiveAllBuff()
    {
        TeamBuffManager m = BattleSystem.GetPC().theTeamBuff;
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:ActiveAllBuff -- 還沒有實作的 target :" + buff.target);
                    break;
                default:
                    m.AddTeamBuff(buff);
                    break;
            }
        }
    }

    //取消所有的 Buff ，應該是在 Doll 死亡或離開隊伍時使用
    protected void DeActiveAllBuff()
    {
        TeamBuffManager m = BattleSystem.GetPC().theTeamBuff;
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:DeActiveAllBuff -- 還沒有實作的 target :" + buff.target);
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
