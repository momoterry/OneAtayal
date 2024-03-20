using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInventoryItem : MonoBehaviour
{
    public Image skillIcon;
    public Text atkText;
    public Image bookIcon;
    //public Color[] bookColors;

    public delegate void ItemClickedCB(int _index);
    protected ItemClickedCB myCB;
    protected int myIndex;

    protected BookEquipSave myEquip;

    public void InitValue(int _index, BookEquipSave equip, ItemClickedCB _CB = null)
    {
        myIndex = _index;
        myEquip = equip;
        myCB = _CB;
        atkText.text = "§ð " + equip.ATK_Percent + "%";
        SkillDollSummonEx skillRef = BookEquipManager.GetInsatance().GetSkillByID(equip.skillID);

        if (skillRef)
        {
            skillIcon.sprite = skillRef.icon;
            DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skillRef.dollID);
            Doll d = dInfo.objRef.GetComponent<Doll>();
            atkText.text = "§ð " + Mathf.RoundToInt(equip.ATK_Percent * 0.01f * d.AttackInit);
        }

        bookIcon.color = GameDef.GetQaulityColor(equip.quality);
        //switch (equip.quality)
        //{
        //    case ITEM_QUALITY.RARE:
        //        bookIcon.color = bookColors[1];
        //        break;
        //    case ITEM_QUALITY.EPIC:
        //        bookIcon.color = bookColors[2];
        //        break;
        //}
    }

    public void OnSelected()
    {
        //print("OnSelected");
        if (myCB != null)
            myCB(myIndex);
    }
}
