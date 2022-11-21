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
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
        }
    }
}
