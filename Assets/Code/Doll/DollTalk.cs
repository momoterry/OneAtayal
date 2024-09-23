using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollTalk : MonoBehaviour
{
    public Talk theTalk;

    protected string strToTalk = "";

    private void Update()
    {
        if (strToTalk != "")
        {
            theTalk.AddSentence(strToTalk);
            strToTalk = "";
        }
    }

    public void OnJoinPlayer()
    {
        //strToTalk = "啊哈哈，我加入了喲 !!";
        string[] joinTalks = {"我來了","是日靈喔","呼....好累","我幫得上忙!!" };
        strToTalk = joinTalks[Random.Range(0, joinTalks.Length)];
        BattleSystem.GetPC().SaySomthing(joinTalks[Random.Range(0,joinTalks.Length)]);
    }
}
