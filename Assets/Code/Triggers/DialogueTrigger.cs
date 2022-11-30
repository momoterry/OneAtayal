using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject[] EndTriggers;

    public string speakerName;
    [TextArea(2, 10)]
    public string[] dialogueContents;

    protected Dialogue theDialogue;

    // Start is called before the first frame update
    void Start()
    {
        theDialogue = BattleSystem.GetDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTG(GameObject whoTG)
    {
        StartDialogue();
        whoTG.SendMessage("OnActionResult", true);
    }

    protected void StartDialogue()
    {
        DialogueContent dc = new DialogueContent();
        dc.name = speakerName;
        dc.textContents = dialogueContents;
        if (theDialogue)
        {
            theDialogue.StartDialoue(dc, gameObject);
        }
    }

    public void OnDialogueFinished()
    {
        foreach (GameObject o in EndTriggers)
        {
            o.SendMessage("OnTG", gameObject);
        }
    }

}
