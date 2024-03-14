using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookEquipSaveAll   // 存檔資料
{
    public BookEquipSave[] Inventory;
    public BookEquipSave[] Equipped;
}


public class BookEquipManager : MonoBehaviour
{
    public const int MAX_BOOKEQUIP = 3;
    [System.Serializable]
    public class SkillMappingItem
    {
        public string ID;
        public SkillDollSummonEx skillRef;
    }
    public SkillMappingItem[] skillMapItems;

    static BookEquipManager instance;
    static public BookEquipManager GetInsatance() { return instance; }

    protected BookEquipSave[] equipped = new BookEquipSave[MAX_BOOKEQUIP];
    protected List<BookEquipSave> inventory = new List<BookEquipSave>();

    static Dictionary<string, SkillDollSummonEx> skillMap = new Dictionary<string, SkillDollSummonEx>();

    public BookEquipManager() : base()
    {
        print("--BookEquipManager");
        if (instance != null)
            print("ERROR !! 超過一份 BookEquipManager 存在 ... ");
        instance = this;
    }

    protected void Awake()
    {
        for (int i = 0; i < skillMapItems.Length; i++)
        {
            print("SKILL MAP " + i + "" + skillMapItems[i].skillRef.name);
            skillMap.Add(skillMapItems[i].ID, skillMapItems[i].skillRef);
        }
    }

    public BookEquipSaveAll ToSaveData()
    {
        print("BookEquipManager.ToSaveData");
        BookEquipSaveAll data = new BookEquipSaveAll();
        data.Equipped = equipped;
        for (int i=0; i<data.Equipped.Length; i++)
        {
            print(i);
        }
        //print("data.Equipped :" + data.Equipped.Length);
        data.Inventory = new BookEquipSave[inventory.Count];
        for (int i=0; i<inventory.Count; i++)
        {
            data.Inventory[i] = inventory[i];
        }
        return data;
    }

    //注意!! Load
    public void FromLoadData(BookEquipSaveAll data)
    {
        print("BookEquipManager.FromLoadData");
        if (data.Equipped != null)
        {
            for (int i=0; i < data.Equipped.Length; i++)
            {
                if (i < MAX_BOOKEQUIP)
                {
                    if (data.Equipped[i] != null && data.Equipped[i].uID != 0)
                    {
                        GameSystem.GetPlayerData().RegisterUsedID(data.Equipped[i].uID);
                        equipped[i] = data.Equipped[i];
                    }
                    else
                        equipped[i] = null;
                }
            }
        }
        if (data.Inventory != null)
        {
            for (int i = 0; i < data.Inventory.Length; i++)
            {
                inventory.Add(data.Inventory[i]);
            }
        }
        print("inventory: " + inventory.Count);
    }

    //主角裝備初始化
    public void SetupToPC()
    {
        print("BookEquipManager.SetupToPC");
    }

    public SkillDollSummonEx GetSkillByID(string ID)
    {
        if (skillMap.ContainsKey(ID))
        {
            return skillMap[ID];
        }
        return null;
    }

    //====================== 各種 BookEquip 操作
    public BookEquipSave GenerateEmptyOne()
    {
        BookEquipSave o = new BookEquipSave();
        o.uID = GameSystem.GetPlayerData().GenerateUniqueId();
        return o;
    }

    //====================== Inventory 相關的操作
    public int GetInventorySize()
    {
        return inventory.Count;
    }

    public void AddToInventory(BookEquipSave equip)
    {
        inventory.Add(equip);
    }

    public BookEquipSave RemoveFromInventoryByIndex(int i)
    {
        if (i < inventory.Count)
        {
            BookEquipSave equip = inventory[i];
            inventory.RemoveAt(i);
            return equip;
        }
        return null;
    }

    public void Equip(BookEquipSave equip, int slotIndex)
    {

    }

}
