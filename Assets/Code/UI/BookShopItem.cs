using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookShopItem : MonoBehaviour
{
    public Image skillIcon;
    public Text costText;

    public delegate void ItemClickedCB(int _index);
    protected ItemClickedCB myCB;
    protected int myIndex;

    //protected BookShopMenu myMenu;
    protected BookShopMenu.BookItemInfo myInfo;

    public void InitValue(int _index, BookShopMenu.BookItemInfo _info, ItemClickedCB _CB = null)
    {
        myIndex = _index;
        myInfo = _info;
        myCB = _CB;

        costText.text = _info.MoneyCost.ToString();
        if (_info.SkillRef)
        {
            skillIcon.sprite = _info.SkillRef.icon;
        }
    }


    public void OnSelected()
    {
        print("OnSelected");
        if (myCB != null)
            myCB(myIndex);
    }

}
