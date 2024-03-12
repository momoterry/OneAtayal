using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookCard : MonoBehaviour
{
    public Image Icon;
    public Text BookName;
    public Text DollStatText;
    public Text BuffDesc;
    
    public void SetCard(SkillDollSummonEx skill)
    {
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        if (dInfo == null)
        {
            print("ERROR!!!! BookCard ¿ù»~ªº Doll ID: " + skill.dollID);
            return;
        }
        Icon.sprite = skill.icon;
        BookName.text = dInfo.dollName + "¥l³ê®Ñ";
        Doll doll = dInfo.objRef.GetComponent<Doll>();
        HitBody hBody = dInfo.objRef.GetComponent<HitBody>();
        float attack = doll.AttackInit * skill.ATK_Percent / 100.0f;
        float hp = hBody.HP_Max * skill.HP_Percent / 100.0f;
        DollStatText.text = "";
        DollStatText.text += "¦å¶q " + Mathf.RoundToInt(hp) + "\n";
        DollStatText.text += "§ðÀ» " + Mathf.RoundToInt(attack) + "\n";
    }

}
