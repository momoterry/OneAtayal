using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//============================================================
//巫靈鍛造機制的成果，相當於鍛造後的結果
//DollInstance 能指定 Doll 種類，同時連結一串 DollBuff
//直接連結到 Doll 的物件上
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

    //開始啟動所有 Buff，應該是在 Doll 加入隊伍後開始作用
    protected void ActiveAllBuff()
    {
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:ActiveAllBuff -- 還沒有實作的 target :" + buff.target);
                    break;
                default:
                    TeamBuffManager.AddTeamBuff(buff);
                    break;
            }
        }
    }

    //取消所有的 Buff ，應該是在 Doll 死亡或離開隊伍時使用
    protected void DeActiveAllBuff()
    {
        foreach (DollBuffBase buff in buffList)
        {
            switch (buff.target)
            {
                case DOLL_BUFF_TARGET.MYSELF:
                case DOLL_BUFF_TARGET.MASTER:
                    print("DollInstance:DeActiveAllBuff -- 還沒有實作的 target :" + buff.target);
                    break;
                default:
                    TeamBuffManager.RemoveTeamBuff(buff);
                    break;
            }
        }
    }


}
