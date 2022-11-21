using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuTrigger : MonoBehaviour
{
    public LevelSelectMenu theMenu;
    public LevelItemInfo[] itemInfos;

    void OnTG(GameObject whoTG)
    {
        if (theMenu)
        {
            theMenu.OpenMenu(itemInfos);
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }
}
