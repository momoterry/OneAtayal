using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
public class SaveData{
    public int Money;
    public CharacterStat mainCharacterStat;
    public string[] usingDollList;
    public SaveDataBackpckItem[] dollBackpack;
    public SaveDataEventItem[] eventData;
}


public class PlayerData : MonoBehaviour
{
    protected bool isReady = false; //是否已經載入資料或完成初始化

    public DollData theDollData;

    protected int Money = 0;

    protected CharacterStat mainCharacterStat = new CharacterStat();

    protected List<string> usingDollList = new List<string>();

    protected Dictionary<string, int> dollBackpack = new Dictionary<string, int>();

    //事件 Flag 
    protected Dictionary<string, bool> eventData = new Dictionary<string, bool>();

    //全新存檔的資料初始化，比  Start() 更早會被呼叫
    public void InitData()
    {
        print("==== PlayerData.InitData()");
        Money = 100;
        mainCharacterStat.LV = 1;
        mainCharacterStat.Exp = 0;
        usingDollList.Clear();
        dollBackpack.Clear();
        eventData.Clear();

        GameSystem.GetLevelManager().InitFirstLevel();
    }

    public void SetDataReady()
    {
        isReady = true;
    }
    public bool IsReady() { return isReady; }

    // Start is called before the first frame update
    void Start()
    {
        //print("==== PlayerData.Start()");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMainCharacterData(CharacterStat stat)
    {
        mainCharacterStat = stat;
    }

    public CharacterStat GetMainChracterData()
    {
        return mainCharacterStat;
    }

    //存檔相關
    public SaveData GetSaveData()
    {
        SaveData data = new SaveData();
        data.Money = Money;
        data.mainCharacterStat = mainCharacterStat;

        if (usingDollList.Count > 0)
        {
            data.usingDollList = new string[usingDollList.Count];
            for (int i = 0; i < usingDollList.Count; i++)
            {
                data.usingDollList[i] = usingDollList[i];
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

        return data;
    }

    public void LoadSavedData( SaveData data)
    {
        //TODO: 如果重覆載入資料的處理 ?

        Money = data.Money;
        mainCharacterStat = data.mainCharacterStat;

        if (data.usingDollList != null && data.usingDollList.Length > 0)
        {
            for (int i =0; i< data.usingDollList.Length; i++)
            {
                AddUsingDoll(data.usingDollList[i]);
            }
        }

        if (data.dollBackpack!= null && data.dollBackpack.Length > 0)
        {
            for (int i =0; i<data.dollBackpack.Length; i++)
            {
                AddDollToBackpack(data.dollBackpack[i].ID, data.dollBackpack[i].num);
            }
        }

        if (data.eventData != null &&data.eventData.Length > 0)
        {
            for ( int i=0; i<data.eventData.Length; i++)
            {
                SaveEvent(data.eventData[i].Event, data.eventData[i].status);
            }
        }

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

    public int GetCurrDollNum()
    {
        return usingDollList.Count;
    }

    public void AddUsingDoll( string dollID)
    {
        usingDollList.Add(dollID);
    }

    public string[] GetAllUsingDolls()
    {
        if (usingDollList.Count > 0)
        {
            string[] allDolls = new string[usingDollList.Count];
            int i = 0;
            foreach( string ds in usingDollList)
            {
                allDolls[i] = ds;
                i++;
            }

            return allDolls;
        }

        return null;
    }

    public void RemoveAllUsingDolls()
    {
        usingDollList.Clear();
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
            print("ERROR!!! The Doll in not in Backpack !!!! : " + dollID);
        }

        print("========= Doll BackPack =========");
        foreach (KeyValuePair<string, int> k in dollBackpack)
        {
            print(k.Key + " : " + k.Value);
        }
        print("========= ========= ========= ");
    }

    public Dictionary<string, int> GetDollBackPack()
    {
        return dollBackpack;
    }

    public GameObject GetDollRefByID(string ID)
    {
        if (theDollData)
        {
            return theDollData.GetDollRefByID(ID);
        }
        return null;
    }

    public int GetDollNumByID(string ID)
    {
        int usingNum = 0;
        foreach (string s in usingDollList)
        {
            if (s == ID)
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

}
