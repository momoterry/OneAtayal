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
    protected BookEquip[] equipInstances = new BookEquip[MAX_BOOKEQUIP];
    protected List<BookEquipSave> inventory = new List<BookEquipSave>();

    static Dictionary<string, SkillDollSummonEx> skillMap = new Dictionary<string, SkillDollSummonEx>();

    public BookEquipManager() : base()
    {
        //print("--BookEquipManager");
        if (instance != null)
            print("ERROR !! �W�L�@�� BookEquipManager �s�b ... ");
        instance = this;
    }

    protected bool oneTimeInit = false;
    protected void Awake()
    {
        if (!oneTimeInit)
        {
            for (int i = 0; i < skillMapItems.Length; i++)
            {
                GameObject o = Instantiate(skillMapItems[i].skillRef.gameObject, transform);
                skillMapItems[i].skillRef = o.GetComponent<SkillDollSummonEx>();

                //print("SKILL MAP " + i + "" + skillMapItems[i].skillRef.name);
                skillMap.Add(skillMapItems[i].ID, skillMapItems[i].skillRef);
            }
            oneTimeInit = true;
        }

    }

    public void InitSave()
    {
        inventory.Clear();
        for (int i=0; i<MAX_BOOKEQUIP; i++)
        {
            equipped[i] = null;
            if (equipInstances[i])
            {
                Destroy(equipInstances[i].gameObject);
                equipInstances[i] = null;
            }
        }
    }

    public BookEquipSaveAll ToSaveData()
    {
        print("BookEquipManager.ToSaveData");
        BookEquipSaveAll data = new BookEquipSaveAll();
        //data.Equipped = equipped;
        data.Equipped = new BookEquipSave[MAX_BOOKEQUIP];
        for (int i=0; i<MAX_BOOKEQUIP; i++)
        {
            data.Equipped[i] = equipped[i];
        }
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
        //print("BookEquipManager.FromLoadData");
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
        //print("inventory: " + inventory.Count);
    }


    public SkillDollSummonEx GetSkillByID(string ID)
    {
        if (skillMap.ContainsKey(ID))
        {
            return skillMap[ID];
        }
        return null;
    }

    protected void DebugCheck()
    {
        print("equipped check ...........");
        for (int i = 0; i < MAX_BOOKEQUIP; i++)
        {
            BookEquipSave eq = equipped[i];
            print("=> " + i + "uID: " + (eq == null ? "null" : eq.uID.ToString()) + " ATK: " + (eq == null ? "0x0" : eq.ATK_Percent.ToString()));
        }
        print("inventory check ..........." + inventory.Count);
        for (int i = 0; i < inventory.Count; i++)
        {
            BookEquipSave eq = inventory[i];
            print("=> " + i + "uID: " + (eq == null ? "null" : eq.uID.ToString()));
        }
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

    protected void SetupEquipOnPlayer(int slotIndex)
    {
        //print("SetupEquipOnPlayer " + slotIndex + "equip ? " + (equipped[slotIndex] == null? "0x0": equipped[slotIndex].uID.ToString()));
        if (slotIndex < 0 || slotIndex >= MAX_BOOKEQUIP)
        {
            return;
        }

        SkillDollSummonEx skill = null;
        BookEquipSave equip = equipped[slotIndex];
        BookEquip inst = equipInstances[slotIndex];

        if (inst != null && equip != null && inst.GetUID() == equip.uID)
        {
            print("���P�@��  ....");
            return;
        }

        if (inst != null)
        {
            print("�M���ª� Instance  ....");
            Destroy(inst.gameObject);
        }

        if (equip != null)
        {
            //print("�إ߷s�� Instance  ....");
            GameObject o = new GameObject("BookEquip_" + slotIndex);
            BookEquip equipInstance = o.AddComponent<BookEquip>();
            equipInstance.FromSave(equip);
            skill = equipInstance.skill;
            equipInstances[slotIndex] = equipInstance;
        }
        else
        {
            //print("�Ū���  ....");
            skill = null;
            equipInstances[slotIndex] = null;
        }

        PC_One thePC = BattleSystem.GetInstance().GetPlayer().GetComponent<PC_One>();
        if (thePC == null)
        {
            print("ERROR!! BookEquipManagger only supoort PC_One!!!!");
            return;
        }
        thePC.SetActiveSkill(skill, slotIndex + 1);
    }

    //====================== Inventory �������ާ@
    public int GetInventorySize()
    {
        return inventory.Count;
    }

    public BookEquipSave GetInventoryByIndex(int i)
    {
        return inventory[i];
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
            //BookEquipSave old = equipped[slotIndex];
            //if (old != null)
            //{
            //    AddToInventory(old);
            //}
            equipped[slotIndex] = equip;
            SetupEquipOnPlayer(slotIndex);
        }
    }


    public BookEquipSave GetCurrEquip(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_BOOKEQUIP)
        {
            return equipped[slotIndex];
        }
        return null;
    }

    //�D���˳ƪ�l��
    public void InitEquipsOnPC()
    {
        //print("BookEquipManager.SetupToPC");
        //DebugCheck();
        for (int i=0; i<MAX_BOOKEQUIP; i++)
        {
            SetupEquipOnPlayer(i);
        }

        //DebugCheck();
    }
}