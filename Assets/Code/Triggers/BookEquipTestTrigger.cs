using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//測試 Equip 用的資料

public class BookEquipTestTrigger : MonoBehaviour
{
    public int HP_Percent_Min, HP_Percent_Max;
    public int ATK_Percent_Min, ATK_Percent_Max;
    public void OnTG(GameObject whoTG)
    {
        //BookEquipManager.GetInsatance()
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應

    }
}
