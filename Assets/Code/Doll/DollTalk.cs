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
        strToTalk = "�ګ����A�ڥ[�J�F�� !!";
    }
}
