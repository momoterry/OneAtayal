using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int moneyAdd = 10;
    //TODO: �߿��S��?

    void OnTG(GameObject whoTG)
    {
        GameSystem.GetPlayerData().AddMoney(moneyAdd);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
