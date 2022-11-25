using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollShopTrigger : MonoBehaviour
{
    public DollShopMenu theMenu;

    void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu();
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
        }
    }
}
