using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BattleConditionItem
{
    public string eventID;
    public enum COMPARE_TYPE
    {
        EQUAL,
        GREATER_EQUAL,
        LESS_EQUAL,
    }
    public COMPARE_TYPE compareType;
    public int compareValue = 0;
    public GameObject triggerTarget;
    public string SaveCondition;      //為了確保該條件只觸發一次用的，目前還沒處理
}

public class ActionTG_BattleCondition : ActionTG
{


    public BattleConditionItem[] conditionsInOrder;

    protected int currentOrder = -1;

    protected int CheckCurrentConditionOrder()
    {
        PlayerData pData = GameSystem.GetPlayerData();
        for (int i = 0; i < conditionsInOrder.Length; i++)
        {
            BattleConditionItem condition = conditionsInOrder[i];

            if (conditionsInOrder[i].SaveCondition != "" && BattlePlayerData.GetInstance().GetEventBool(condition.SaveCondition))
            {
                //print("Condition Saved: " + condition.SaveCondition);
                continue;
            }

            int value = BattlePlayerData.GetInstance().GetEventBool(condition.eventID) ? 1:0;
            if (value == 0)
                value = BattlePlayerData.GetInstance().GetEventInt(condition.eventID);

            //print("BattelEvent: " + condition.eventID + "值為: " + value);

            switch (condition.compareType)
            {
                case BattleConditionItem.COMPARE_TYPE.EQUAL:
                    if (value == condition.compareValue)
                        return i;
                    break;
                case BattleConditionItem.COMPARE_TYPE.GREATER_EQUAL:
                    if (value >= condition.compareValue)
                        return i;
                    break;
                case (BattleConditionItem.COMPARE_TYPE.LESS_EQUAL):
                    if (value <= condition.compareValue)
                        return i;
                    break;
            }

        }
        return -1;
    }

    protected override bool IsActionValid()
    {
        currentOrder = CheckCurrentConditionOrder();
        //print("ActionTG_BattleCondition: " + currentOrder);
        return (currentOrder >= 0 || TriggerTarget != null);
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
                    //GameSystem.GetPlayerData().SaveEvent(conditionsInOrder[currentOrder].SaveCondition, true);
                    BattlePlayerData.GetInstance().SetEventBool(conditionsInOrder[currentOrder].SaveCondition, true);
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
}
