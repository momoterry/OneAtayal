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
    }

    public ConditionItem[] conditionsInOrder;

    protected int currentOrder = -1;

    protected int CheckCurrentConditionOrder()
    {
        for (int i=0; i<conditionsInOrder.Length; i++)
        {
            bool eventResult = GameSystem.GetPlayerData().GetEvent(conditionsInOrder[i].eventID);
            if (eventResult == conditionsInOrder[i].onEventStatus)
            {
                return i;
            }
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
            if (currentOrder >= 0)
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
            }
        }
    }

}
