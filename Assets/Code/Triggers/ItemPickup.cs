using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string ItemID;
    public bool showMessage = false;

    public void OnTG(GameObject whoTG)
    {

        GameSystem.GetPlayerData().AddItem(ItemID, 1);

        if (showMessage)
        {
            ItemInfo info = ItemDef.GetInstance().GetItemInfo(ItemID);
            if (info!=null)
                BattleSystem.GetPC().SaySomthing("¾ß¨ì  " + info.Name);
        }

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
