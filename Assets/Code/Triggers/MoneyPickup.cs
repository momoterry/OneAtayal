using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int moneyAdd = 10;
    //TODO: �߿��S��?

    public void OnTG(GameObject whoTG)
    {
        //GameSystem.GetPlayerData().AddMoney(moneyAdd);
        BattleSystem.GetInstance().OnAddMoney(moneyAdd);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
