using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//「巫靈召喚書」的物件
// 穿在角色身上
// 對應一個 SkillDollSummonEx

[System.Serializable]
public class BookEquipSave
{
    public int uID = 0;
    public string skillID = "";
    public ITEM_QUALITY quality = ITEM_QUALITY.COMMON;
    public int ATK_Percent = 0;
    public int HP_Percent = 0;
}

public class BookEquip : MonoBehaviour
{
    public SkillDollSummonEx skill;
    
    protected BookEquipSave save;

    public int GetUID()
    {
        return save.uID;
    }

    public BookEquipSave ToSave()
    {
        return save;
    }

    public void FromSave(BookEquipSave _save)
    {
        save = _save;

        SkillDollSummonEx skillRef = BookEquipManager.GetInsatance().GetSkillByID(save.skillID);
        skill = Instantiate(skillRef, transform);
        skill.ATK_Percent = save.ATK_Percent;
        skill.HP_Percent = save.HP_Percent;
    }
}
