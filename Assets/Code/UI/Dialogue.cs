using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueContent
{
    public string name;
    public string[] textContents;
}

public class Dialogue : MonoBehaviour
{
    public GameObject theWindow;
    public bool isRepeatable = true;
    public GameObject[] EndTriggers;

    public Text theText;
    public Text talkerName;

    [TextArea(2, 10)]
    public string[] overwriteContents;

    MyInputActions theInput;

    protected bool usingOverwite = false;
    protected int currOverwiteIndex = 0;

    //由其它元件來觸發
    protected GameObject myOwner;

    protected enum PHASE
    {
        NONE,
        WAIT,
        NORMAL,
        END,
    }
    PHASE currState = PHASE.NONE;
    PHASE nextState = PHASE.NONE;

    // Start is called before the first frame update
    void Start()
    {
        if (theWindow)
            theWindow.SetActive(false);

        //Input System Bind
        theInput.TheHero.Attack.performed += ctx => OnClick();
        theInput.TheHero.Action.performed += ctx => OnClick();

        InitContent();

        nextState = PHASE.WAIT;
    }

    protected void InitContent()
    {
        if (theText && overwriteContents.Length > 0)
        {
            currOverwiteIndex = 0;
            usingOverwite = true;
            SetupContent();
        }
    }

    private void Awake()
    {
        theInput = new MyInputActions();
    }

    private void OnDestroy()
    {
        theInput.Disable();
    }

    void OnEnterState()
    {
        switch (nextState)
        {
            case PHASE.WAIT:
                if (theWindow)
                    theWindow.SetActive(false);
                break;
            case PHASE.NORMAL:
                if (theWindow)
                    theWindow.SetActive(true);

                BattleSystem.GetInstance().GetPlayerController().SetInputActive(false);
                theInput.Enable();
                break;
            case PHASE.END:
                if (theWindow)
                    theWindow.SetActive(false);
                theInput.Disable();
                BattleSystem.GetInstance().GetPlayerController().SetInputActive(true);
                if (myOwner)
                {
                    myOwner.SendMessage("OnDialogueFinished");
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currState != nextState)
        {
            OnEnterState();
            currState = nextState;
            return;
        }

        switch (currState)
        {
            case PHASE.END:
                if (isRepeatable)
                {
                    nextState = PHASE.WAIT;
                }
                else
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void OnClick()
    {
        TryDoNext();
    }

    void OnTG(GameObject whoTG)
    {
        if (currState == PHASE.WAIT)
        {
            myOwner = null;
            nextState = PHASE.NORMAL;
            whoTG.SendMessage("OnActionResult", true);
        }
    }

    public void StartDialoue(DialogueContent content, GameObject owner)
    {
        if (currState == PHASE.WAIT)
        {
            myOwner = owner;
            overwriteContents = content.textContents;
            if (talkerName)
            {
                talkerName.text = content.name;
            }
            InitContent();

            nextState = PHASE.NORMAL;
        }
    }

    protected void OnMouseDown()
    {
        TryDoNext();
    }

    protected void SetupContent()
    {
        theText.text = overwriteContents[currOverwiteIndex];
    }

    protected void TryDoNext()
    {
        if (currState == PHASE.NORMAL)
        {
            if (usingOverwite && currOverwiteIndex < overwriteContents.Length - 1)
            {
                currOverwiteIndex++;
                SetupContent();
            }
            else
            {
                nextState = PHASE.END;
            }
        }
    }

}
