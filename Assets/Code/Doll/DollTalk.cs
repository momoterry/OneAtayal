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
        strToTalk = "啊哈哈，我加入了喲 !!";
    }
}
