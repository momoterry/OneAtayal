using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollForgeTrigger : MonoBehaviour
{
    public ForgeMenu theMenu;

    public void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu();
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
        }
    }
}
