using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStartTrigger : MonoBehaviour
{
    void OnTG(GameObject whoTG)
    {
        MissionManager.StartCurrMission();
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }
}
