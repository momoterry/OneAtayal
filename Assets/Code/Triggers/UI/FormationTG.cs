using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationTG : MonoBehaviour
{
    void OnTG(GameObject whoTG)
    {
        BattleSystem.GetHUD().OpenDollLayoutUI();
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }

    // ===================== 以下是舊的常駐式陣型按鈕用

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("Formation Trigger IN");
            BattleSystem.GetHUD().OnOffDollLayoutAll(true);
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //print("Formation Trigger OUT");
            BattleSystem.GetHUD().OnOffDollLayoutAll(false);
        }
    }
}
