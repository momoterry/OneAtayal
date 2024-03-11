using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollShopTrigger : MonoBehaviour
{
    public DollShopMenu theMenu;
    public DollShopItemInfo[] shopItemInfo;

    void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu(shopItemInfo);
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
        }
    }
}
