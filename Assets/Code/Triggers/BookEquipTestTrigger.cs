using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� Equip �Ϊ����

public class BookEquipTestTrigger : MonoBehaviour
{
    public string skillID = "DollAce";
    public int HP_Percent_Min = 100;
    public int HP_Percent_Max = 200;
    public int ATK_Percent_Min = 100;
    public int ATK_Percent_Max = 200;

    //public bool forceEquip = false;

    public void OnTG(GameObject whoTG)
    {
        BookEquipSave newEquip = BookEquipManager.GetInsatance().GenerateEmptyOne();
        //print("Default new Equip");
        //print("ATK_Percent: " + newEquip.ATK_Percent);
        //print("HP_Percent: " + newEquip.HP_Percent);

        newEquip.skillID = skillID;
        newEquip.ATK_Percent = Random.Range(ATK_Percent_Min, ATK_Percent_Max);
        newEquip.HP_Percent = Random.Range(HP_Percent_Min, HP_Percent_Max);

        BookEquipManager.GetInsatance().AddToInventory(newEquip);
        print("�s�W�˳�: ATK: " + newEquip.ATK_Percent + " HP: " + newEquip.HP_Percent);

        GameSystem.GetInstance().SaveData();
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }
}
