using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookCard : MonoBehaviour
{
    public Image Icon;
    public Image BookIcon;
    public Text BookName;
    public Text DollStatText;
    public Text EnhanceDesc;

    //public void SetCard(SkillDollSummonEx skill)
    //{
    //    DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
    //    if (dInfo == null)
    //    {
    //        print("ERROR!!!! BookCard ¿ù»~ªº Doll ID: " + skill.dollID);
    //        return;
    //    }
    //    Icon.sprite = skill.icon;
    //    BookName.text = dInfo.dollName + "¥l³ê®Ñ";
    //    Doll doll = dInfo.objRef.GetComponent<Doll>();
    //    HitBody hBody = dInfo.objRef.GetComponent<HitBody>();
    //    float attack = skill.ATK_Percent;
    //    float hp = hBody.HP_Max * skill.HP_Percent / 100.0f;
    //    DollStatText.text = "";
    //    DollStatText.text += "¦å¶q " + Mathf.RoundToInt(hp) + "\n";
    //    DollStatText.text += "§ðÀ» " + Mathf.RoundToInt(attack) + "\n";
    //}

    public void SetCard(BookEquipSave equip, bool hideValue = false)
    {
        SkillDollSummonEx skill = BookEquipManager.GetInsatance().GetSkillByID(equip.skillID);
        Icon.sprite = skill.icon;

        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        //BookName.text = dInfo.dollName + "¥l³ê®Ñ";

        BookName.color = GameDef.GetQaulityColor(equip.quality);
        BookIcon.color = GameDef.GetQaulityColor(equip.quality);
        Doll doll = dInfo.objRef.GetComponent<Doll>();
        HitBody hBody = dInfo.objRef.GetComponent<HitBody>();
        if (hideValue)
        {
            BookName.text = "????";
            DollStatText.text = "";
            DollStatText.text += "§ðÀ» ????\n";
            DollStatText.text += "¦å¶q ????\n";
            EnhanceDesc.text = "????";
        }
        else
        {
            BookName.text = equip.bookName;
            float attack = equip.ATK_Percent * doll.AttackInit * 0.01f;
            //float attack = equip.ATK_Percent;
            float hp = hBody.HP_Max * equip.HP_Percent / 100.0f;
            DollStatText.text = "";
            DollStatText.text += "§ðÀ» " + Mathf.RoundToInt(attack) + "\n";
            DollStatText.text += "¦å¶q " + Mathf.RoundToInt(hp) + "\n";

            string eStr = "";
            if (equip.ATK_Percent > 100)
            {
                eStr = eStr + "§ðÀ»  +" + (Mathf.RoundToInt(equip.ATK_Percent) - 100) + "%\n";
            }
            if (equip.HP_Percent > 100)
            {
                eStr = eStr + "¦å¶q  +" + (Mathf.RoundToInt(equip.HP_Percent) - 100) + "%\n";
            }
            EnhanceDesc.text = eStr;
        }
    }
}