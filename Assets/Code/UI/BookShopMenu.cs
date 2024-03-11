using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//巫靈書商店介面
//Book 表示 Doll Summon Skill ，就算改用別的方式包裝

public class BookShopMenu : MonoBehaviour
{
    [System.Serializable]
    public class BookItemInfo
    {
        public SkillDollSummonEx SkillRef;
        public int MoneyCost = 1000;
    }

    public Transform MenuRoot;

    public void OpenMenu(BookItemInfo[] items)
    {
        MenuRoot.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        MenuRoot.gameObject.SetActive(false);
    }
}
