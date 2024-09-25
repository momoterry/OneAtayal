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
        string[] joinTalks = {"�ڨӤF","�O���F��","�I....�n��","�����o�W��!!" };
        string joinTalk = joinTalks[Random.Range(0, joinTalks.Length)];

        //strToTalk = joinTalk;
        ComicTalk.StartTalk(joinTalk, gameObject, 2.0f);
    }
}
