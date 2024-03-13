using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookEquipSaveAll   // 存檔資料
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
            print("ERROR !! 超過一份 BookEquipManager 存在 ");
        instance = this;
    }

    public BookEquipSaveAll ToSaveData()
    {
        print("BookEquipManager.ToSaveData");
        return new BookEquipSaveAll();
    }

    //注意!! Load
    public void FromLoadData(BookEquipSaveAll data)
    {
        print("BookEquipManager.FromLoadData");
    }

    //主角裝備初始化
    public void SetupToPC()
    {
        print("BookEquipManager.SetupToPC");
    }
}
