using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//「巫靈召喚書」的物件
// 穿在角色身上
// 對應一個 SkillDollSummonEx

[System.Serializable]
public class BookEquipSave
{
    public string skillID;
    public int HP_Percent;
    public int ATK_Percent;
}

public class BookEquip : MonoBehaviour
{
    public SkillDollSummonEx skill;
    
    protected BookEquipSave save;

    public BookEquipSave ToSave()
    {
        return save;
    }

    public void FromSave(BookEquipSave _save)
    {
        save = _save;
        //TODO:
        //在這裡建立 skill Instance
    }
}
