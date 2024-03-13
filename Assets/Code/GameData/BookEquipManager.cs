using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookEquipSaveAll   // �s�ɸ��
{
    public BookEquipSave[] Inventory;
    public BookEquipSave[] equipped;
}



public class BookEquipManager : MonoBehaviour
{
    static BookEquipManager instance;
    static public BookEquipManager GetInsatance() { return instance; }
    protected void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� BookEquipManager �s�b ");
        instance = this;
    }

    public BookEquipSaveAll ToSaveData()
    {
        print("BookEquipManager.ToSaveData");
        return new BookEquipSaveAll();
    }

    //�`�N!! Load
    public void FromLoadData(BookEquipSaveAll data)
    {
        print("BookEquipManager.FromLoadData");
    }

    //�D���˳ƪ�l��
    public void SetupToPC()
    {
        print("BookEquipManager.SetupToPC");
    }
}
