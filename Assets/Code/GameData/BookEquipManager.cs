using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookEquipSaveAll   // �s�ɸ��
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
            print("ERROR !! �W�L�@�� BookEquipManager �s�b ... ");
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
        data.Inventory = new BookEquipSave[inventory.Count];
        for (int i=0; i<inventory.Count; i++)
        {
            data.Inventory[i] = inventory[i];
        }
        return data;
    }

    //�`�N!! Load
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

    //�D���˳ƪ�l��
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

    //====================== �U�� BookEquip �ާ@
    public BookEquipSave GenerateEmptyOne()
    {
        BookEquipSave o = new BookEquipSave();
        o.uID = GameSystem.GetPlayerData().GenerateUniqueId();
        o.ATK_Percent = 100;
        o.HP_Percent = 100;
        return o;
    }

    public void DestroyOne(BookEquipSave equip)
    {
        GameSystem.GetPlayerData().UnRegisterUsedID(equip.uID);
    }

    //====================== Inventory �������ާ@
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
        if (slotIndex >= 0 && slotIndex < MAX_BOOKEQUIP)
        {
            BookEquipSave old = equipped[slotIndex];
            if (old != null)
            {
                AddToInventory(old);
            }
            equipped[slotIndex] = equip;
        }
    }

    //public BookEquipSave UnEquip(int slotIndex)
    //{
    //    if (slotIndex >=0 && slotIndex < MAX_BOOKEQUIP){
    //        BookEquipSave old = equipped[slotIndex];
    //        if (old != null)
    //        {
    //            AddToInventory(old);
    //        }
    //        equipped[slotIndex] = null;
    //        return old;
    //    }
    //    return null;
    //}

    public BookEquipSave GetCurrEquip(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_BOOKEQUIP)
        {
            return equipped[slotIndex];
        }
        return null;
    }

}
