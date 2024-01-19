using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeMenu : MonoBehaviour
{
    public GameObject MenuRoot;
    public void OpenMenu()
    {
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(true);
            ClearAllItems(); //以免重復開啟造成問題
            CreateAllItems();
        }
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        ClearAllItems();
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(false);
        }
        BattleSystem.GetPC().ForceStop(false);
    }

    protected void ClearAllItems()
    {

    }

    protected void CreateAllItems()
    {

    }
}
