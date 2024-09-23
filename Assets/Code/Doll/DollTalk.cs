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
        //strToTalk = "�ګ����A�ڥ[�J�F�� !!";
        string[] joinTalks = {"�ڨӤF","�O���F��","�I....�n��","�����o�W��!!" };
        strToTalk = joinTalks[Random.Range(0, joinTalks.Length)];
        BattleSystem.GetPC().SaySomthing(joinTalks[Random.Range(0,joinTalks.Length)]);
    }
}
