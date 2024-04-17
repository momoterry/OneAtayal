using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookEquipSaveAll   // 存檔資料
{
    public BookEquipSave[] Inventory;
    public BookEquipSave[] Equipped;
}

//[System.Serializable]
//public class BookDollSummonDef
//{
//    public string dollID;
//    public int battlePointsCost = 1;
//}

//帶有隨機效果的 BookEquip 定義
[System.Serializable]
public class MagicBookEquipInfo
{
    public string ID;
    public string nameBeforeEnhance;         //在 enhance 前的名稱
    public ITEM_QUALITY quality;
    public string[] skillList;
    public BookEquipEnhancerBase enhancer;
}

public class BookEquipManager : MonoBehaviour
{
    public const int MAX_BOOKEQUIP = 3;
    public MagicBookEquipInfo[] magicBookDef;
    //public BookDollSummonDef[] dollSummonSkillRefDef;

    [System.Serializable]
    public class SkillMappingItem
    {
        public string ID;
        public SkillDollSummonEx skillRef;
    }
    public SkillMappingItem[] skillMapItems;

    static BookEquipManager instance;
    static public BookEquipManager GetInstance() { return instance; }

    protected BookEquipSave[] equipped = new BookEquipSave[MAX_BOOKEQUIP];
    protected BookEquip[] equipInstances = new BookEquip[MAX_BOOKEQUIP];
    protected List<BookEquipSave> inventory = new List<BookEquipSave>();

    static Dictionary<string, MagicBookEquipInfo> magicBookMap = new Dictionary<string, MagicBookEquipInfo>();
    static Dictionary<string, SkillDollSummonEx> skillMap = new Dictionary<string, SkillDollSummonEx>();

    public BookEquipManager() : base()
    {
        //print("--BookEquipManager");
        if (instance != null)
            print("ERROR !! 超過一份 BookEquipManager 存在 ... ");
        instance = this;
    }

    protected bool oneTimeInit = false;
    protected void Awake()
    {
        if (!oneTimeInit)
        {
            for (int i=0; i< magicBookDef.Length; i++)
            {
                magicBookMap.Add(magicBookDef[i].ID, magicBookDef[i]);
            }
            //for (int i = 0; i < skillMapItems.Length; i++)
            //{
            //    GameObject o = Instantiate(skillMapItems[i].skillRef.gameObject, transform);
            //    skillMapItems[i].skillRef = o.GetComponent<SkillDollSummonEx>();

            //    //print("SKILL MAP " + i + "" + skillMapItems[i].skillRef.name);
            //    skillMap.Add(skillMapItems[i].ID, skillMapItems[i].skillRef);
            //}

            //foreach (BookDollSummonDef dsDef in dollSummonSkillRefDef)
            //{
            //    DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(dsDef.dollID);
            //    print("Try Create Skill Ref for : " + dsDef.dollID + " dInfo: " +dInfo.dollName );

            //    GameObject o = new GameObject("SkillEX_"+ dsDef.dollID);
            //    o.SetActive(false);   //確保 SkillDollSummonEx 參數設定完才 Awake
            //    o.transform.SetParent(gameObject.transform);
            //    SkillDollSummonEx sEx = o.AddComponent<SkillDollSummonEx>();
            //    sEx.dollID = dsDef.dollID;
            //    sEx.battlePointsCost = dsDef.battlePointsCost;
            //    sEx.coolDown = 0;

            //    o.SetActive(true);   //確保 SkillDollSummonEx 參數設定完才 Awake

            //    skillMap.Add(dsDef.dollID, sEx);
            //}

            DollInfo[] dInfos = GameSystem.GetDollData().DollInfos;
            foreach (DollInfo dInfo in dInfos)
            {
                //print("Try Create Skill Ref for : " + dInfo.dollID + " dInfo: " + dInfo.dollName);

                GameObject o = new GameObject("SkillEX_" + dInfo.dollID);
                o.SetActive(false);   //確保 SkillDollSummonEx 參數設定完才 Awake
                o.transform.SetParent(gameObject.transform);
                SkillDollSummonEx sEx = o.AddComponent<SkillDollSummonEx>();
                sEx.dollID = dInfo.dollID;
                sEx.battlePointsCost = dInfo.summonCost;
                sEx.coolDown = 0;

                o.SetActive(true);   //確保 SkillDollSummonEx 參數設定完才 Awake

                skillMap.Add(dInfo.dollID, sEx);
            }

            oneTimeInit = true;
        }
    }

    public void InitSave()
    {
        string[] initEquips = { "DollOne", "DollStone", "DollFire" };
        string[] initEquipNames = { "日靈書", "石靈書", "火靈書" };
        inventory.Clear();
        for (int i=0; i<MAX_BOOKEQUIP; i++)
        {
            equipped[i] = null;
            if (equipInstances[i])
            {
                Destroy(equipInstances[i].gameObject);
                equipInstances[i] = null;
            }
            if (initEquips[i] != "")
            {
                equipped[i] = GenerateEmptyOne();
                equipped[i].skillID = initEquips[i];
                equipped[i].bookName = initEquipNames[i];
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

    //注意!! Load
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


    //====================== Magic Book 產生相關

    public MagicBookEquipInfo GetMagicBookInfo(string ID)
    {
        if (magicBookMap.ContainsKey(ID))
            return magicBookMap[ID];
        return null;
    }

    public BookEquipSave GenerateMagicBook(string ID)
    {
        if (!magicBookMap.ContainsKey(ID))
            return null;

        MagicBookEquipInfo info = magicBookMap[ID];
        BookEquipSave equip = GenerateEmptyOne();
        equip.skillID = info.skillList[Random.Range(0, info.skillList.Length)];
        equip.quality = info.quality;
        
        //真正 Enhance 的部份
        if (info.enhancer)
        {
            info.enhancer.DoEnhance(ref equip);
        }
        return equip;
    }

    //====================== 各種 BookEquip 操作
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
            print("穿到同一件  ....");
            return;
        }

        if (inst != null)
        {
            print("清除舊的 Instance  ....");
            Destroy(inst.gameObject);
        }

        if (equip != null)
        {
            //print("建立新的 Instance  ....");
            GameObject o = new GameObject("BookEquip_" + slotIndex);
            BookEquip equipInstance = o.AddComponent<BookEquip>();
            equipInstance.FromSave(equip);
            skill = equipInstance.skill;
            equipInstances[slotIndex] = equipInstance;
        }
        else
        {
            //print("空的裝  ....");
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

    //====================== Inventory 相關的操作
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

    public bool CheckIfEquipPossible(BookEquipSave equip, ref string errMsg, ref int validSlot)
    {
        errMsg = "";
        validSlot = -1;
        bool repeatErr = false;
        for (int i= equipped.Length-1; i>=0; i--)   //確保最後的 validSlot 是最小的
        {
            if (equipped[i] != null)
            {
                if (equipped[i].skillID == equip.skillID)
                {
                    repeatErr = true;   //沒有欄位的錯誤要優先，所以不先跳出
                }
            }
            else
            {
                validSlot = i;
            }
        }
        if (validSlot < 0)
        {
            errMsg = "沒有空的欄位";
            return false;
        }
        else if (repeatErr)
        {
            errMsg = "已經有重復的巫靈";
            return false;
        }
        return true;
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

    //主角裝備初始化
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
