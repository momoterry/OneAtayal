using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeMaterialItem : MonoBehaviour
{
    public Image matIcon;
    public Text matName;
    public Text matRequireNum;
    public Text matHasNum;


    public void InitValue(ForgeMaterialInfo info)
    {
        print("ForgeMaterialItem.IntValue: " + info.matID);
        ItemInfo matInfo = ItemDef.GetInstance().GetItemInfo(info.matID);
        if (matInfo == null)
        {
            print("ERROR!! ¿ù»~ªº matID: " + info.matID);
            return;
        }
        matIcon.sprite = matInfo.Icon;
        matRequireNum.text = info.num.ToString();
        matName.text = matInfo.Name;
        int hasNum = GameSystem.GetInstance().thePlayerData.GetItemNum(info.matID);
        matHasNum.text = "(«ù¦³: "+ hasNum.ToString()+")";
        if (hasNum < info.num)
        {
            matHasNum.color = Color.red;
        }
    }

}
