using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {
        //GameSystem.GetPlayerData().AddMoney(moneyAdd);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
