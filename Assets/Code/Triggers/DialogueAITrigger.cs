using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAITrigger : MonoBehaviour
{
    //public GameObject[] EndTriggers;

    public string speakerName;
    public Sprite speakerImage;
    public ChatGPT myChatGPT;

    protected Dialogue theDialogue;
    protected bool isWaitingGPT = false;

    // Start is called before the first frame update
    void Start()
    {
        theDialogue = BattleSystem.GetDialogue();
        if (!myChatGPT)
        {
            myChatGPT = GetComponent<ChatGPT>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTG(GameObject whoTG)
    {
        //StartDialogue();
        if (myChatGPT)
        {
            if (isWaitingGPT)
            {
                print("還在等待上一個對話回應....");
            }
            else 
            { 
                myChatGPT.StartChat(StartDialogue);
                isWaitingGPT = true;
            }
        }
        whoTG.SendMessage("OnActionResult", true);
    }

    protected void StartDialogue(string message)
    {
        isWaitingGPT = false;
        DialogueContent dc = new DialogueContent();
        dc.name = speakerName;
        dc.headImage = speakerImage;
        dc.textContents = new string[1];
        dc.textContents[0] = message.Trim();
        if (theDialogue)
        {
            theDialogue.StartDialoue(dc, gameObject);
        }
    }

    public void OnDialogueFinished()
    {
        //foreach (GameObject o in EndTriggers)
        //{
        //    o.SendMessage("OnTG", gameObject);
        //}
    }
}
