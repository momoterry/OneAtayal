using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    public GameObject theWindow;
    public GameObject[] EndTriggers;

    MyInputActions theInput;

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

        nextState = PHASE.WAIT;
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
        }
    }

    void OnClick()
    {
        if (currState == PHASE.NORMAL)
        {
            nextState = PHASE.END;
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (currState == PHASE.WAIT)
        {
            nextState = PHASE.NORMAL;
            whoTG.SendMessage("OnActionResult", true);
        }
    }

    private void OnMouseDown()
    {
        if (currState == PHASE.NORMAL)
        {
            nextState = PHASE.END;
        }
    }
}
