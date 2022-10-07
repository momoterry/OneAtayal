using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveEvent : MonoBehaviour
{
    public string EventName;
    public bool EventStatus = true;
    // Start is called before the first frame update

    void OnTG(GameObject whoTG)
    {
        if (EventName != "")
        {
            GameSystem.GetPlayerData().SaveEvent(EventName, EventStatus);
        }
    }
}
