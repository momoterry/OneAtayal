using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PlayerData 記錄玩家跨關卡間的進度內容
//都要加上 [System.Serializable] 才能確保被 Json 轉到

//Item: 素材等可以堆疊的物件
[System.Serializable]
public struct SaveDataItem
{
    public string ID;
    public int num;
}

[System.Serializable]
public struct SaveDataBackpckItem
{
    public string ID;
    public int num;
}

[System.Serializable]
public struct SaveDataEventItem
{
    public string Event;
    public bool status;
}

[System.Serializable]
public struct DollBuffData
{
    public int buffType;
    public int buffTarget;
    public int buffValue1;
}
[System.Serializable]
public struct FormationDollInfo // 有指定陣型位置的 Doll Info
{
    public int group;
    public int index;
    public string dollID;   //TODO: 支援 DollInstance 的情況
}
[System.Serializable]
public struct DollInstanceData
{
    public int uID;
    public string baseDollID;
    public string fullName; //TODO: 改成 TextID ?
    public DollBuffData[] buffs;
}

//地圖存檔
[System.Serializable]
public class MapSaveDataBase
{
    public string mapName;
    public string className;
    public string mapMask64;
}


[System.Serializable]
public class SaveData{
    public int Money;
    public CharacterStat mainCharacterStat;
    //public string[] usingDollList;
    public FormationDollInfo[] formationDollList;
    public SaveDataItem[] itemData;
    public SaveDataBackpckItem[] dollBackpack;
    public SaveDataEventItem[] eventData;
    public DollInstanceData[] usingDIs;
    public BookEquipSaveAll bookEquipAll;
    public WorldMapSaveData worldMap;
    public MapSavePerlinField[] savedPFields;
    public MapSaveMazeDungeon[] savedMazes;
    public MapSaveMazeOne[] saveMazeOnes;
}


public class PlayerData : MonoBehaviour
{
    //玩家資料初始值，目前數值如下 (Alpha 版)，如果值不同就是在測試
    //  INIT_MONEY = 500;
    //  INIT_LV = 1;
    //  INIT_MAX_DOLL_NUM = 4;

    //以下為 Beta 版數值
    public const int INIT_MONEY = 2000;
    public const int INIT_LV = 1;
    public const int INIT_MAX_DOLL_NUM = 24;

    protected bool isReady = false; //是否已經載入資料或完成初始化

    //public DollData theDollData;

    protected int Money;

    protected CharacterStat mainCharacterStat = new CharacterStat();

    //主角的模型 Ref
    protected GameObject playerCharacterRef = null;
    //Skill 相關
    protected Dictionary<string, SkillBase> skillMap = new Dictionary<string, SkillBase>();


    protected List<DollInstanceData> usingDIs = new List<DollInstanceData>();
    //protected List<string> usingDollList = new List<string>();
    protected List<FormationDollInfo> formationDollList = new List<FormationDollInfo>();

    protected Dictionary<string, int> itemData = new Dictionary<string, int>();
    protected Dictionary<string, int> dollBackpack = new Dictionary<string, int>();

    //地圖記錄
    protected Dictionary<string, MapSaveDataBase> savedMaps = new Dictionary<string, MapSaveDataBase>();

    //事件 Flag 
    protected Dictionary<string, bool> eventData = new Dictionary<string, bool>();

    //==== 有關唯一 ID 的生成
    static private HashSet<int> usedIds = new HashSet<int>();
    public int GenerateUniqueId()
    {
        int id;
        do
        {
            id = Random.Range(0, int.MaxValue); // 生成隨機整數
        } while (usedIds.Contains(id)); // 檢查是否已經使用過，如果是，重新生成

        usedIds.Add(id); // 將新的 ID 加入已使用的集合
        return id;
    }

    public void RegisterUsedID(int _id)
    {
        usedIds.Add(_id);
    }
    public void UnRegisterUsedID(int _id)
    {
        usedIds.Remove(_id);
    }
    //====


    //全新存檔的資料初始化，比  Start() 更早會被呼叫，基本上是在 GameSystem 的 Awake 中被呼叫
    public void InitData()
    {
        One.LOG("==== GameSystem 的 PlayerData.InitData()");
        Money = INIT_MONEY;
        mainCharacterStat.LV = INIT_LV;
        mainCharacterStat.Exp = 0;
        usingDIs.Clear();
        //usingDollList.Clear();
        formationDollList.Clear();
        itemData.Clear();
        dollBackpack.Clear();
        eventData.Clear();
        savedMaps.Clear();

        usedIds.Clear();

        //除了 GameSystem 以外，不能依賴其它 System 的 Instance (Awake)
        GameSystem.GetWorldMap().Init();
        GameSystem.GetLevelManager().InitFirstLevel();
        GameSystem.GetBookEquipManager().InitSave();

    }

    public void SetDataReady()
    {
        isReady = true;
    }
    public bool IsReady() { return isReady; }

    public void SetMainCharacterData(CharacterStat stat)
    {
        mainCharacterStat = stat;
    }

    public CharacterStat GetMainChracterData()
    {
        return mainCharacterStat;
    }

    //========================== 從 GameSystem 移過來 ==========================
    public void SetPlayerCharacterRef(GameObject objRef)
    {
        playerCharacterRef = objRef;
    }

    public GameObject GetPlayerCharacterRef()
    {
        return playerCharacterRef;
    }

    public void SetPlayerSkillRef(string skillStr, SkillBase skillRef)
    {
        if (skillMap.ContainsKey(skillStr))
            skillMap[skillStr] = skillRef;
        else
            skillMap.Add(skillStr, skillRef);
    }

    public SkillBase GetPlayerSkillRef(string skillStr)
    {
        if (skillMap.ContainsKey(skillStr))
            return skillMap[skillStr];
        else
            return null;
    }
    //====================================================

    //存檔相關
    public SaveData GetSaveData()
    {
        SaveData data = new SaveData();
        data.Money = Money;
        data.mainCharacterStat = mainCharacterStat;

        if (usingDIs.Count > 0)
        {
            data.usingDIs = new DollInstanceData[usingDIs.Count];
            for (int i = 0; i < usingDIs.Count; i++)
            {
                data.usingDIs[i] = usingDIs[i];
                print("ToSave " + data.usingDIs[i].fullName);
            }
        }

        //if (usingDollList.Count > 0)
        //{
        //    data.usingDollList = new string[usingDollList.Count];
        //    for (int i = 0; i < usingDollList.Count; i++)
        //    {
        //        data.usingDollList[i] = usingDollList[i];
        //    }
        //}

        if (formationDollList.Count > 0)
        {
            data.formationDollList = new FormationDollInfo[formationDollList.Count];
            for (int i = 0; i < formationDollList.Count; i++)
            {
                data.formationDollList[i] = formationDollList[i];
            }
        }

        if (itemData.Count > 0)
        {
            data.itemData = new SaveDataItem[itemData.Count];
            int i = 0;
            foreach (KeyValuePair<string, int> k in itemData)
            {
                data.itemData[i].ID = k.Key;
                data.itemData[i].num = k.Value;
                i++;
            }
        }

        if (dollBackpack.Count > 0)
        {
            data.dollBackpack = new SaveDataBackpckItem[dollBackpack.Count];
            int i = 0;
            foreach ( KeyValuePair<string, int> k in dollBackpack)
            {
                data.dollBackpack[i].ID = k.Key;
                data.dollBackpack[i].num = k.Value;
                i++;
            }
        }

        if (eventData.Count > 0)
        {
            data.eventData = new SaveDataEventItem[eventData.Count];
            int i = 0;
            foreach ( KeyValuePair<string, bool> k in eventData)
            {
                data.eventData[i].Event = k.Key;
                data.eventData[i].status = k.Value;
                i++;
            }
        }

        data.bookEquipAll = BookEquipManager.GetInstance().ToSaveData();

        if (savedMaps.Count > 0)
        {
            int nPF = 0; 
            int nMaze = 0;
            List<MapSaveMazeOne> moList = new List<MapSaveMazeOne>();

            foreach (KeyValuePair<string, MapSaveDataBase> k in savedMaps)
            {
                if (k.Value.className == "MG_PerlinField")
                {
                    nPF++;
                }
                else if (k.Value.className == "MG_MazeDungeon")
                {
                    nMaze++;
                }
                else if (k.Value.GetType() == typeof(MapSaveMazeOne))
                {
                    print("儲存一個 MapSaveMazeOne");
                    moList.Add((MapSaveMazeOne)k.Value);
                }
            }

            //int i = 0;
            int iPF = 0;
            int iMaze = 0;
            //data.savedPFields = new MapSavePerlinField[savedMaps.Count];
            data.savedPFields = new MapSavePerlinField[nPF];
            data.savedMazes = new MapSaveMazeDungeon[nMaze];
            foreach (KeyValuePair<string, MapSaveDataBase> k in savedMaps)
            {
                //data.savedPFields[i] = (MapSavePerlinField)k.Value;
                if (k.Value.className == "MG_PerlinField")
                {
                    data.savedPFields[iPF] = (MapSavePerlinField)k.Value;
                    iPF++;
                }
                else if (k.Value.className == "MG_MazeDungeon")
                {
                    data.savedMazes[iMaze] = (MapSaveMazeDungeon)k.Value;
                    iMaze++;
                }
                //i++;
            }
            data.saveMazeOnes = new MapSaveMazeOne[moList.Count];
            for (int i=0; i < moList.Count; i++)
            {
                data.saveMazeOnes[i] = moList[i];
            }
            print("----地圖存檔完成----");
        }

        data.worldMap = GameSystem.GetWorldMap().SaveData();

        return data;
    }

    public void LoadSavedData( SaveData data)
    {
        //先清空以避免重覆加載的 Bug
        InitData();

        Money = data.Money;
        //One.LOG("----Money 載入完成---- ");
        mainCharacterStat = data.mainCharacterStat;

        if (data.usingDIs != null && data.usingDIs.Length > 0)
        {
            for (int i = 0; i < data.usingDIs.Length; i++)
            {
                RegisterUsedID(data.usingDIs[i].uID);
                AddUsingDI(data.usingDIs[i]);
            }
        }
        //One.LOG("----usingDIs 載入完成---- ");


        if (data.formationDollList != null && data.formationDollList.Length > 0)
        {
            for (int i = 0; i < data.formationDollList.Length; i++)
            {
                AddUsingDoll(data.formationDollList[i].dollID, data.formationDollList[i].group, data.formationDollList[i].index);
            }
        }
        //One.LOG("----formationDollList 載入完成---- ");

        if (data.itemData != null && data.itemData.Length > 0)
        {
            for (int i = 0; i < data.itemData.Length; i++)
            {
                //AddDollToBackpack(data.itemData[i].ID, data.itemData[i].num);
                //itemData.Add(data.itemData[i].ID, data.itemData[i].num);
                AddItem(data.itemData[i].ID, data.itemData[i].num);
            }
        }
        //One.LOG("----itemData 載入完成---- ");

        if (data.dollBackpack!= null && data.dollBackpack.Length > 0)
        {
            for (int i =0; i<data.dollBackpack.Length; i++)
            {
                AddDollToBackpack(data.dollBackpack[i].ID, data.dollBackpack[i].num);
            }
        }
        //One.LOG("----dollBackpack 載入完成---- ");

        if (data.eventData != null &&data.eventData.Length > 0)
        {
            for ( int i=0; i<data.eventData.Length; i++)
            {
                SaveEvent(data.eventData[i].Event, data.eventData[i].status);
            }
        }
        //One.LOG("----eventData 載入完成---- ");

        if (data.bookEquipAll != null)
        {
            GameSystem.GetInstance().theBookEquipManager.FromLoadData(data.bookEquipAll);
        }
        //One.LOG("----bookEquip 載入完成---- ");

        if (data.savedPFields !=null && data.savedPFields.Length > 0)
        {
            for (int i=0; i<data.savedPFields.Length; i++)
            {
                savedMaps.Add(data.savedPFields[i].mapName, data.savedPFields[i]);
            }
            //One.LOG("----PField 地圖載入完成---- ");
        }

        if (data.savedMazes != null && data.savedMazes.Length > 0)
        {
            for (int i=0; i<data.savedMazes.Length; i++)
            {
                savedMaps.Add(data.savedMazes[i].mapName, data.savedMazes[i]);
            }
            //One.LOG("----Maze 地圖載入完成---- ");
        }

        if (data.saveMazeOnes != null && data.saveMazeOnes.Length > 0)
        {
            for (int i = 0; i < data.saveMazeOnes.Length; i++)
            {
                savedMaps.Add(data.saveMazeOnes[i].mapName, data.saveMazeOnes[i]);
            }
            //One.LOG("----MO 地圖載入完成---- ");
        }

        if (data.worldMap != null)
        {
            GameSystem.GetWorldMap().LoadData(data.worldMap);
            //One.LOG("----世界地圖載入完成---- ");
        }
        One.LOG("----LoadSavedData 載入完成---- ");
    }

    // 介面
    public int GetMoney() { return Money; }
    public int AddMoney(int value)
    {
        Money += value;
        if (Money < 0)
            Money = 0;

        return Money;
    }

    public int GetMaxDollNum()
    {
        return mainCharacterStat.DollMax;
    }

    //================== 關於新的 DollInstance 使用 ======================================
    public void AddUsingDI(DollInstanceData data)
    {
        usingDIs.Add(data);

        //print("目前的 DI 列表");
        //for (int i = 0; i < usingDIs.Count; i++)
        //{
        //    print("----" + usingDIs[i].fullName);
        //}
    }

    public void RemoveUsingDI(DollInstanceData data)
    {
        //TODO: 怎麼處理 ?
        bool br = usingDIs.Remove(data);  //這有辦法嗎?
        print("移除 DI 結果: " + br);
    }

    //測試用的暴力函式，清除整個世界的所有 DI
    public void RemoveAllUsingDIs()
    {
        foreach (DollInstanceData data in usingDIs)
        {
            UnRegisterUsedID(data.uID);
        }
        usingDIs.Clear();
    }

    public DollInstanceData[] GetAllUsingDIs()
    {
        if (usingDIs.Count > 0)
        {
            DollInstanceData[] data = new DollInstanceData[usingDIs.Count];
            for (int i=0; i<usingDIs.Count; i++)
            {
                data[i] = usingDIs[i];
            }
            return data;
        }
        return null;
    }

    public int GetCurrDollNum()
    {
        return formationDollList.Count;
        //return usingDollList.Count;
    }

    //public void AddUsingDoll( string dollID)
    //{
    //    usingDollList.Add(dollID);
    //}

    public void AddUsingDoll( string dollID, int group = -1, int index = -1 )
    {
        //print("加入永久巫靈: " + dollID + " Group: " + group + " Index: " + index);
        //usingDollList.Add(dollID);

        FormationDollInfo fdInfo = new FormationDollInfo();
        fdInfo.dollID = dollID;
        fdInfo.group = group;
        fdInfo.index = index;
        formationDollList.Add(fdInfo);
    }

    public FormationDollInfo[] GetAllFormationDolls()
    {
        if (formationDollList.Count > 0)
        {
            FormationDollInfo[] allDolls = new FormationDollInfo[formationDollList.Count];
            int i = 0;
            foreach (FormationDollInfo info in formationDollList)
            {
                allDolls[i] = info;
                i++;
            }

            return allDolls;
        }
        return null;
    }

    public string[] GetAllUsingDolls()
    {
        //if (usingDollList.Count > 0)
        //{
        //    string[] allDolls = new string[usingDollList.Count];
        //    int i = 0;
        //    foreach( string ds in usingDollList)
        //    {
        //        allDolls[i] = ds;
        //        i++;
        //    }

        //    return allDolls;
        //}
        if (formationDollList.Count > 0)
        {
            string[] allDolls = new string[formationDollList.Count];
            int i = 0;
            foreach (FormationDollInfo dInfo in formationDollList)
            {
                allDolls[i] = dInfo.dollID;
                i++;
            }

            return allDolls;
        }

        return null;
    }

    public void RemoveAllUsingDolls()
    {
        //usingDollList.Clear();
        formationDollList.Clear();
    }


    public Dictionary<string, int> GetItemInventory()
    {
        return itemData;
    }

    public int GetItemNum(string ID)
    {
        if (itemData.ContainsKey(ID))
        {
            return itemData[ID];
        }
        else
        {
            return 0;
        }
    }

    public void AddItem(string ID, int add = 1)
    {
        if (itemData.ContainsKey(ID))
        {
            itemData[ID] += add;
        }
        else
        {
            itemData.Add(ID, add);
        }
    }

    public void AddDollToBackpack( string dollID, int add = 1 )
    {
        if (dollBackpack.ContainsKey(dollID))
        {
            dollBackpack[dollID] = dollBackpack[dollID] + add;
        }
        else
        {
            dollBackpack.Add(dollID, add);
        }

        //print("========= Doll BackPack =========");
        //foreach ( KeyValuePair<string, int> k in dollBackpack)
        //{
        //    print(k.Key + " : " + k.Value);
        //}
        //print("========= ========= ========= ");
    }

    public void RemoveDollFromBackpack( string dollID)
    {
        if (dollBackpack.ContainsKey(dollID))
        {
            dollBackpack[dollID] = dollBackpack[dollID] - 1;
            if (dollBackpack[dollID] <= 0)
            {
                dollBackpack.Remove(dollID);
            } 
        }
        else
        {
            //dollBackpack.Add(dollID, 1);
            One.LOG("ERROR!!! The Doll in not in Backpack !!!! : " + dollID);
        }

    //    print("========= Doll BackPack =========");
    //    foreach (KeyValuePair<string, int> k in dollBackpack)
    //    {
    //        print(k.Key + " : " + k.Value);
    //    }
    //    print("========= ========= ========= ");
    }

    public Dictionary<string, int> GetDollBackPack()
    {
        return dollBackpack;
    }

    //public GameObject GetDollRefByID(string ID)
    //{
    //    if (theDollData)
    //    {
    //        return theDollData.GetDollRefByID(ID);
    //    }
    //    return null;
    //}

    public int GetDollNumByID(string ID)
    {
        int usingNum = 0;
        //foreach (string s in usingDollList)
        //{
        //    if (s == ID)
        //    {
        //        usingNum++;
        //    }
        //}
        foreach (FormationDollInfo dInfo in formationDollList)
        {
            if (dInfo.dollID == ID)
            {
                usingNum++;
            }
        }
        int backNum = 0;
        if (dollBackpack.ContainsKey(ID))
        {
            backNum = dollBackpack[ID];
        }


        return usingNum + backNum;
    }

    public void SaveEvent(string eventName, bool status = true)
    {
        if (eventData.ContainsKey(eventName))
        {
            eventData[eventName] = status;
        }
        else
        {
            eventData.Add(eventName, status);
        }
    }

    public bool GetEvent(string eventName)
    {
        if (eventData.ContainsKey(eventName))
        {
            return eventData[eventName];
        }

        return false;
    }

    // ========================================= 關於地圖 ============================= 
    public void SaveMap(string name , MapSaveDataBase data)
    {
        if (savedMaps.ContainsKey(name))
        {
            if (data != null)
            {
                savedMaps[name] = data;
            }
            else
            {
                savedMaps.Remove(name);
            }
        }
        else
        {
            if (data != null) 
            {
                savedMaps.Add(name, data);
            }
        }
    }

    public MapSaveDataBase GetSavedMap(string name)
    {
        if (name == null || name == "")
            return null;

        if (savedMaps.ContainsKey(name))
        {
            return savedMaps[name];
        }
        return null;
    }

    public void ResetWorldMap()
    {
        savedMaps.Clear();      //清除所有地圖存檔
        GameSystem.GetWorldMap().Init();    //重設世界地圖
    }
}
