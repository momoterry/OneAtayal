using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookShopItem : MonoBehaviour
{
    public Image skillIcon;
    public Text costText;

    protected BookShopMenu myMenu;
    protected BookShopMenu.BookItemInfo myInfo;

    public void InitValue(BookShopMenu _menu, BookShopMenu.BookItemInfo _info)
    {
        myMenu = _menu;
        myInfo = _info;
        costText.text = _info.MoneyCost.ToString();
        if (_info.SkillRef)
        {
            skillIcon.sprite = _info.SkillRef.icon;
        }
    }
}
