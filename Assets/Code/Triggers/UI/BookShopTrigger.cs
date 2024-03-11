using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShopTrigger : MonoBehaviour
{
    public BookShopMenu theMenu;
    public BookShopMenu.BookItemInfo[] items;

    public void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu(items);
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }
}
