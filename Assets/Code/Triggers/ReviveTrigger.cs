using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveTrigger : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {
        PC_One pc = (PC_One)BattleSystem.GetPC();

        if (pc)
        {
            print("�� PC�A�յ۴_������ !!");
            pc.ReviveAllDoll();
        }
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }
}
