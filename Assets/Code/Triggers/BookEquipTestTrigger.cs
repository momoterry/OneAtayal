using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� Equip �Ϊ����

public class BookEquipTestTrigger : MonoBehaviour
{
    public int HP_Percent_Min, HP_Percent_Max;
    public int ATK_Percent_Min, ATK_Percent_Max;
    public void OnTG(GameObject whoTG)
    {
        //BookEquipManager.GetInsatance()
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��

    }
}
