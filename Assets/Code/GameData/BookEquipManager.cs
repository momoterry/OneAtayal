using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookEquipSaveAll   // �s�ɸ��
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

//�a���H���ĪG�� BookEquip �w�q (2024/0528 �s�W�A�ծѤ]�i�H�ΦP�ˤ覡�w�q�A�u�O�S�� skillList)
[System.Serializable]
public class MagicBookEquipInfo
{
    public string ID;
    public string nameBeforeEnhance;         //�b enhance �e���W��
    public ITEM_QUALITY quality;
    public string[] skillList;
    public BookEquipEnhancerBase enhancer;
    public Sprite subIcon = null;
}

public class BookEquipManager : GlobalSystemBase
{
    public const int MAX_BOOKEQUIP = 3;
    public MagicBookEquipInfo[] magicBookDef;

    public string WHITEBOOK_ID_PREFIX = "WhiteBook_";

    static BookEquipManager instance;
    static public BookEquipManager GetInstance() { return instance; }

    protected BookEquipSave[] equipped = new BookEquipSave[MAX_BOOKEQUIP];
    protected BookEquip[] equipInstances = new BookEquip[MAX_BOOKEQUIP];
    protected List<BookEquipSave> inventory = new List<BookEquipSave>();

    static Dictionary<string, MagicBookEquipInfo> magicBookMap = new Dictionary<string, MagicBookEquipInfo>();
    static Dictionary<string, SkillDollSummonEx> skillMap = new Dictionary<string, SkillDollSummonEx>();

    //public BookEquipManager() : base()
    //{
    //    //print("--BookEquipManager");
    //    if (instance != null)
    //        print("ERROR !! �W�L�@�� BookEquipManager �s�b ... ");
    //    instance = this;
    //}

    //public DollData theDollData;
    protected bool oneTimeInit = false;
    protected void Awake()
    {
        //print("BookEquipManager ..... Awake");
        if (instance != null)
            print("ERROR !! �W�L�@�� BookEquipManager �s�b ... ");
        instance = this;

        //for (int i = 0; i < magicBookDef.Length; i++)
        //{
        //    magicBookMap.Add(magicBookDef[i].ID, magicBookDef[i]);
        //}
    }

    //private void Start()
    //{
    //    if (!oneTimeInit)
    //    {
    //        //print("BookEquipManager Init Skill Refs");
    //        //DollInfo[] dInfos = GameSystem.GetDollData().DollInfos;
    //        DollInfo[] dInfos = GameSystem.GetDollData().GetAllDollInfo();
    //        foreach (DollInfo dInfo in dInfos)
    //        {
    //            //print("Try Create Skill Ref for : " + dInfo.dollID + " dInfo: " + dInfo.dollName);

    //            GameObject o = new GameObject("SkillEX_" + dInfo.dollID);
    //            o.SetActive(false);   //�T�O SkillDollSummonEx �ѼƳ]�w���~ Awake
    //            o.transform.SetParent(gameObject.transform);
    //            SkillDollSummonEx sEx = o.AddComponent<SkillDollSummonEx>();
    //            sEx.dollID = dInfo.dollID;
    //            sEx.battlePointsCost = dInfo.summonCost;
    //            //print(sEx.dollID + " cost: " + sEx.battlePointsCost);
    //            sEx.coolDown = 0;

    //            o.SetActive(true);   //�T�O SkillDollSummonEx �ѼƳ]�w���~ Awake

    //            skillMap.Add(dInfo.dollID, sEx);
    //        }

    //        //�[�J�ծ�
    //        GameObject baseEnhanceObj = new GameObject("BaseEnhance");
    //        baseEnhanceObj.transform.parent = transform;
    //        BookEquipEnhancerBase baseEnhancer = baseEnhanceObj.AddComponent<BookEquipEnhancerBase>();
    //        foreach (DollInfo dInfo in dInfos)
    //        {
    //            MagicBookEquipInfo wInfo = new MagicBookEquipInfo();
    //            wInfo.ID = WHITEBOOK_ID_PREFIX + dInfo.dollID;
    //            wInfo.quality = ITEM_QUALITY.COMMON;
    //            wInfo.skillList = new string[1];
    //            wInfo.skillList[0] = dInfo.dollID;
    //            wInfo.nameBeforeEnhance = dInfo.dollName + "��";
    //            wInfo.enhancer = baseEnhancer;
    //            wInfo.subIcon = dInfo.icon;
    //            magicBookMap.Add(wInfo.ID, wInfo);
    //        }
    //        oneTimeInit = true;
    //    }
    //}

    public override void InitSystem()
    {
        base.InitSystem();
        for (int i = 0; i < magicBookDef.Length; i++)
        {
            magicBookMap.Add(magicBookDef[i].ID, magicBookDef[i]);
        }
        if (!oneTimeInit)
        {
            One.LOG("BookEquipManager Init Skill Refs");
            //DollInfo[] dInfos = GameSystem.GetDollData().DollInfos;
            DollInfo[] dInfos = GameSystem.GetDollData().GetAllDollInfo();
            One.LOG("BookEquipManager dInfos Size: " + dInfos.Length);
            foreach (DollInfo dInfo in dInfos)
            {
                //print("Try Create Skill Ref for : " + dInfo.dollID + " dInfo: " + dInfo.dollName);

                GameObject o = new GameObject("SkillEX_" + dInfo.dollID);
                o.SetActive(false);   //�T�O SkillDollSummonEx �ѼƳ]�w���~ Awake
                o.transform.SetParent(gameObject.transform);
                SkillDollSummonEx sEx = o.AddComponent<SkillDollSummonEx>();
                sEx.dollID = dInfo.dollID;
                sEx.battlePointsCost = dInfo.summonCost;
                //print(sEx.dollID + " cost: " + sEx.battlePointsCost);
                sEx.coolDown = 0;

                o.SetActive(true);   //�T�O SkillDollSummonEx �ѼƳ]�w���~ Awake

                skillMap.Add(dInfo.dollID, sEx);
            }

            //�[�J�ծ�
            GameObject baseEnhanceObj = new GameObject("BaseEnhance");
            baseEnhanceObj.transform.parent = transform;
            BookEquipEnhancerBase baseEnhancer = baseEnhanceObj.AddComponent<BookEquipEnhancerBase>();
            foreach (DollInfo dInfo in dInfos)
            {
                MagicBookEquipInfo wInfo = new MagicBookEquipInfo();
                wInfo.ID = WHITEBOOK_ID_PREFIX + dInfo.dollID;
                wInfo.quality = ITEM_QUALITY.COMMON;
                wInfo.skillList = new string[1];
                wInfo.skillList[0] = dInfo.dollID;
                wInfo.nameBeforeEnhance = dInfo.dollName + "��";
                wInfo.enhancer = baseEnhancer;
                wInfo.subIcon = dInfo.icon;
                magicBookMap.Add(wInfo.ID, wInfo);
            }
            oneTimeInit = true;
        }
    }

    public void InitSave()
    {
        One.LOG("BookEquipManager InitSave....");
        string[] initEquips = { "DollStone", "DollWood", "DollFire" };
        string[] initEquipNames = { "���F��", "���F��", "���F��" };
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
                One.LOG("initEquips " + i + ": " + initEquipNames[i]);
            }
        }
    }

    public BookEquipSaveAll ToSaveData()
    {
        One.LOG("BookEquipManager.ToSaveData");
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
        //One.LOG("BookEquipManager.FromLoadData");
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
        //One.LOG("inventory: " + inventory.Count);
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


    //====================== Magic Book ���ͬ���

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
        
        //�u�� Enhance ������
        if (info.enhancer)
        {
            info.enhancer.DoEnhance(ref equip);
        }
        return equip;
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
           One.LOG("ERROR!! BookEquipManagger only supoort PC_One!!!!");
            return;
        }
        thePC.SetActiveSkill(skill, slotIndex + 1);
    }

    //====================== Inventory �������ާ@ ======================
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

    public int GetBookSellValue(BookEquipSave equip)       //���汼�ѥ��M�ΡA�]�w�������\��
    {
        switch (equip.quality)
        {
            case ITEM_QUALITY.COMMON:
                return 100;
            case ITEM_QUALITY.UNCOMMON:
                return 200;
            case ITEM_QUALITY.RARE:
                return 500;
            case ITEM_QUALITY.EPIC:
                return 2000;
            case ITEM_QUALITY.UNIQUE:
                return 5000;
            case ITEM_QUALITY.LEGENDARY:
                return 10000;
        }
        return 100;
    }

    public bool CheckIfEquipPossible(BookEquipSave equip, ref string errMsg, ref int validSlot)
    {
        errMsg = "";
        validSlot = -1;
        bool repeatErr = false;
        for (int i= equipped.Length-1; i>=0; i--)   //�T�O�̫᪺ validSlot �O�̤p��
        {
            if (equipped[i] != null)
            {
                if (equipped[i].skillID == equip.skillID)
                {
                    repeatErr = true;   //�S����쪺���~�n�u���A�ҥH�������X
                }
            }
            else
            {
                validSlot = i;
            }
        }
        if (validSlot < 0)
        {
            errMsg = "�S���Ū����";
            return false;
        }
        else if (repeatErr)
        {
            errMsg = "�w�g�����_�����F";
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
