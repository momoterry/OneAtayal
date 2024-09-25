using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TALK_CONDITION
{
    COLLECTED,
}

public class DollTalk : MonoBehaviour
{

    public string[] collectTalks;

    private void Update()
    {
        //if (strToTalk != "")
        //{
        //    theTalk.AddSentence(strToTalk);
        //    strToTalk = "";
        //}
    }

    public void OnJoinPlayer()
    {
        string[] joinTalks = {"我來了","是日靈喔","呼....好累","我幫得上忙!!" };
        string joinTalk = joinTalks[Random.Range(0, joinTalks.Length)];

        //strToTalk = joinTalk;
        //ComicTalk.StartTalk(joinTalk, gameObject, 2.0f);
    }

    public void OnTalkCondition(TALK_CONDITION tCondition)
    {
        print("OnTalkCondition!! 收到");
        switch (tCondition) 
        {
            case TALK_CONDITION.COLLECTED:
                string msg = collectTalks[Random.Range(0, collectTalks.Length)];
                ComicTalk.StartTalk(msg, gameObject, 2.0f);
                break;
        }
    }

}
