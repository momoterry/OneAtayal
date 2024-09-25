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
        string[] joinTalks = {"�ڨӤF","�O���F��","�I....�n��","�����o�W��!!" };
        string joinTalk = joinTalks[Random.Range(0, joinTalks.Length)];

        //strToTalk = joinTalk;
        //ComicTalk.StartTalk(joinTalk, gameObject, 2.0f);
    }

    public void OnTalkCondition(TALK_CONDITION tCondition)
    {
        print("OnTalkCondition!! ����");
        switch (tCondition) 
        {
            case TALK_CONDITION.COLLECTED:
                string msg = collectTalks[Random.Range(0, collectTalks.Length)];
                ComicTalk.StartTalk(msg, gameObject, 2.0f);
                break;
        }
    }

}
