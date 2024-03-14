using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�u���F�l��ѡv������
// ��b���⨭�W
// �����@�� SkillDollSummonEx

[System.Serializable]
public class BookEquipSave
{
    public int uID = 0;
    public string skillID = "";
    public int ATK_Percent = 100;
    public int HP_Percent = 100;
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
        //�b�o�̫إ� skill Instance
    }
}
