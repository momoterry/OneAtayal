using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string ItemID;

    public void OnTG(GameObject whoTG)
    {

        GameSystem.GetPlayerData().AddItem(ItemID, 1);

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
