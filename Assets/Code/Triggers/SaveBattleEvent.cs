using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBattleEvent : MonoBehaviour
{
    public string eventID;
    public enum EVENT_TYPE
    {
        BOOL,
        INT_ADD,
    }
    public EVENT_TYPE eventType;
    public int eventValue = 1;
    // Start is called before the first frame update
    public void OnTG(GameObject whoTG)
    {
        if (eventID != "")
        {
            if (eventType == EVENT_TYPE.BOOL)
                BattlePlayerData.GetInstance().SetEventBool(eventID, eventValue > 0 ? true : false);
            else if (eventType == EVENT_TYPE.INT_ADD)
                BattlePlayerData.GetInstance().AddEventInt(eventID, eventValue);
        }
    }
}
