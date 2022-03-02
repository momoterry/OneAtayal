using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll : MonoBehaviour
{
    protected enum DOLL_STATE
    {
        NONE,
        WAIT,
        FOLLOW,
    }
    protected DOLL_STATE currState = DOLL_STATE.NONE;
    protected DOLL_STATE nextState = DOLL_STATE.NONE;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnStateExit()
    {

    }

    void OnStateEnter()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nextState != currState)
        {
            OnStateExit();
            OnStateEnter();
            currState = nextState;
            return;
        }
        
        
        switch (currState)
        {
            case DOLL_STATE.FOLLOW:
                break;
        }
        
    }

    void OnTG(GameObject whoTG)
    {
        //print("OnTG");

        if (currState == DOLL_STATE.WAIT)
        {
            whoTG.SendMessage("OnActionResult", false);
        }
    }
}
