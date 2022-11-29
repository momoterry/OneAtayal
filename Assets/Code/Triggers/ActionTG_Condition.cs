using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTG_Condition : ActionTG
{
    [System.Serializable]
    public class ConditionItem
    {
        public string eventID;
        public bool onEventStatus = true;
        public GameObject triggerTarget;
        public string SaveCondition;    //if true, this condition will pass only once
    }

    public ConditionItem[] conditionsInOrder;

    protected int currentOrder = -1;

    protected int CheckCurrentConditionOrder()
    {
        PlayerData pData = GameSystem.GetPlayerData();
        for (int i=0; i<conditionsInOrder.Length; i++)
        {
            //bool conditionResult = true;
            if (conditionsInOrder[i].SaveCondition != "" && pData.GetEvent(conditionsInOrder[i].SaveCondition))
            {
                //print("Condition Saved: " + conditionsInOrder[i].SaveCondition);
                continue;
            }

            if (conditionsInOrder[i].eventID == "" || pData.GetEvent(conditionsInOrder[i].eventID) == conditionsInOrder[i].onEventStatus)
            {
                //print("Goto " + i);
                return i;
            }
            //print("Fail: " + pData.GetEvent(conditionsInOrder[i].eventID));
        }
        return -1;
    }

    protected override void OnGameObjectIn(GameObject obj)
    {
        PlayerControllerBase pc = obj.GetComponent<PlayerControllerBase>();
        if (pc)
        {
            currentOrder = CheckCurrentConditionOrder();
            //print("ActionTG_Condition: " + currentOrder);
            if (currentOrder >= 0 || TriggerTarget != null)
            {
                base.OnGameObjectIn(obj);
            }
        }
    }

    protected override void OnAction()
    {
        if (playerToActive)
        {
            if (currentOrder >= 0)
            {
                GameObject o = conditionsInOrder[currentOrder].triggerTarget;
                if (o)
                {
                    o.SendMessage("OnTG", gameObject);
                }
                if (conditionsInOrder[currentOrder].SaveCondition != "")
                {
                    GameSystem.GetPlayerData().SaveEvent(conditionsInOrder[currentOrder].SaveCondition, true);
                }
            }
            else if (TriggerTarget)
            {
                TriggerTarget.SendMessage("OnTG", gameObject);
            }
            else
            {
                print("ERROR!! 不應該跑到這裡 !! ActionTG_Condition::OnAction");
            }
        }
    }

    //protected override void OnActionResult(bool result)
    //{
    //    if (result)
    //    {
    //        if (!deleteAfterAction)
    //        {
    //            currentOrder = CheckCurrentConditionOrder();
    //            bool isBlockContinue = (currentOrder < 0 && TriggerTarget == null);
    //            print("isBlockContinue " + isBlockContinue + " " + currentOrder);
    //            if (!continueAction || isBlockContinue)
    //            {
    //                if (hint)
    //                {
    //                    hint.gameObject.SetActive(false);
    //                }

    //                if (playerToActive)
    //                {
    //                    playerToActive.OnUnregisterActionObject(gameObject);
    //                    playerToActive = null;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}

}
