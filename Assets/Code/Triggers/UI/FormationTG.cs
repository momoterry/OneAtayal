using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationTG : MonoBehaviour
{
    void OnTG(GameObject whoTG)
    {
        BattleSystem.GetHUD().OpenDollLayoutUI();
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }

    // ===================== �H�U�O�ª��`�n���}�����s��

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
