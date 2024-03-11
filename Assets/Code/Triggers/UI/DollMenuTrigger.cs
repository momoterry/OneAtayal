using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollMenuTrigger : MonoBehaviour
{
    public DollMenu theMenu;

    public void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu();
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }

}
