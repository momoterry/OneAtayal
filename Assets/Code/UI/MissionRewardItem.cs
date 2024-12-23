using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionRewardItem : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Text numText;

    public void InitValue(string itemID, int num)
    {
        if (itemID != null)
        {
            ItemInfo iInfo = ItemDef.GetInstance().GetItemInfo(itemID);
            if (iInfo != null)
            {
                nameText.text = iInfo.Name;
                icon.sprite = iInfo.Icon;
            }
            else
            {
                One.ERROR("¿ù»~ªº Item ID:" + itemID);
            }
        }

        numText.text = num.ToString();
    }
}
