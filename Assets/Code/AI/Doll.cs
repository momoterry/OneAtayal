using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll : MonoBehaviour
{

    protected DollManager theDollManager;
    protected Transform mySlot;

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
        nextState = DOLL_STATE.WAIT;
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
                UpdateFollow();
                break;
        }
        
    }

    virtual protected void UpdateFollow()
    {
        gameObject.transform.position = mySlot.position;
    }

    void OnTG(GameObject whoTG)
    {
        //print("OnTG");

        if (currState == DOLL_STATE.WAIT)
        {
            //回應 ActionTrigger 是否成功
            bool actionResult = TryJoinThePlayer();
            whoTG.SendMessage("OnActionResult", actionResult);
            if (actionResult)
            {
                nextState = DOLL_STATE.FOLLOW;
            }
        }
    }

    bool TryJoinThePlayer()
    {
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if ( pc ){
            DollManager theDollManager = pc.GetDollManager();
            if (theDollManager)
            {
                mySlot = theDollManager.AddOneDoll(this);
            }
        }

        if (mySlot)
            return true;
        return false;
    }
}
