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
    //        print("ERROR!!!! BookCard 錯誤的 Doll ID: " + skill.dollID);
    //        return;
    //    }
    //    Icon.sprite = skill.icon;
    //    BookName.text = dInfo.dollName + "召喚書";
    //    Doll doll = dInfo.objRef.GetComponent<Doll>();
    //    HitBody hBody = dInfo.objRef.GetComponent<HitBody>();
    //    float attack = skill.ATK_Percent;
    //    float hp = hBody.HP_Max * skill.HP_Percent / 100.0f;
    //    DollStatText.text = "";
    //    DollStatText.text += "血量 " + Mathf.RoundToInt(hp) + "\n";
    //    DollStatText.text += "攻擊 " + Mathf.RoundToInt(attack) + "\n";
    //}

    public void SetCard(BookEquipSave equip, bool hideValue = false)
    {
        SkillDollSummonEx skill = BookEquipManager.GetInsatance().GetSkillByID(equip.skillID);
        Icon.sprite = skill.icon;

        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        //BookName.text = dInfo.dollName + "召喚書";

        BookName.color = GameDef.GetQaulityColor(equip.quality);
        BookIcon.color = GameDef.GetQaulityColor(equip.quality);
        Doll doll = dInfo.objRef.GetComponent<Doll>();
        HitBody hBody = dInfo.objRef.GetComponent<HitBody>();
        if (hideValue)
        {
            BookName.text = "未知的" + dInfo.dollName + "召喚書";
            DollStatText.text = "";
            DollStatText.text += "攻擊 ????\n";
            DollStatText.text += "血量 ????\n";
            EnhanceDesc.text = "????";
        }
        else
        {
            BookName.text = equip.bookName;
            float attack = equip.ATK_Percent * doll.AttackInit * 0.01f;
            //float attack = equip.ATK_Percent;
            float hp = hBody.HP_Max * equip.HP_Percent / 100.0f;
            DollStatText.text = "";
            DollStatText.text += "攻擊 " + Mathf.RoundToInt(attack) + "\n";
            DollStatText.text += "血量 " + Mathf.RoundToInt(hp) + "\n";

            string eStr = "";
            if (equip.ATK_Percent > 100)
            {
                eStr = eStr + "攻擊  +" + (Mathf.RoundToInt(equip.ATK_Percent) - 100) + "%\n";
            }
            if (equip.HP_Percent > 100)
            {
                eStr = eStr + "血量  +" + (Mathf.RoundToInt(equip.HP_Percent) - 100) + "%\n";
            }
            EnhanceDesc.text = eStr;
        }
    }
}