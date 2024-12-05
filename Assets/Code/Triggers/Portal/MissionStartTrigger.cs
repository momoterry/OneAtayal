using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionStartTrigger : MonoBehaviour
{
    public float fadeTime = 0.25f;
    public void OnTG(GameObject whoTG)
    {
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��

        BattleSystem.GetPC().ForceStop();
        BattleSystem.GetInstance().StartFadeOut(fadeTime, OnFadeOutFinish);
    }

    public void OnFadeOutFinish()
    {
        MissionManager.StartCurrMission();
    }
}
