using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenuItem : MonoBehaviour
{
    public Text nameText;
    public Text numText;
    public Image itemIcon;

    public void InitValue(ItemInfo iInfo, int num)
    {
        nameText.text = iInfo.Name;
        itemIcon.sprite = iInfo.Icon;
        numText.text = num.ToString();
    }
}
