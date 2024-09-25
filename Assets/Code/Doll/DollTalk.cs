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
        string[] joinTalks = {"我來了","是日靈喔","呼....好累","我幫得上忙!!" };
        string joinTalk = joinTalks[Random.Range(0, joinTalks.Length)];

        //strToTalk = joinTalk;
        ComicTalk.StartTalk(joinTalk, gameObject, 2.0f);
    }
}
