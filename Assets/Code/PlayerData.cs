using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//PlayerData �O�����a�����d�����i�פ��e
//���n�[�W [System.Serializable] �~��T�O�Q Json ���
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
public struct FormationDollInfo // �����w�}����m�� Doll Info
{
    public int group;
    public int index;
    public string dollID;   //TODO: �䴩 DollInstance �����p
}
[System.Serializable]
public struct DollInstanceData
{
    public int uID;
    public string baseDollID;
    public string fullName; //TODO: �令 TextID ?
    public DollBuffData[] buffs;
}

//�a�Ϧs��
[System.Serializable]
public class MapSaveDataBase
{
    public string mapName;
    public string className;
}

[System.Serializable]
public class SaveData{
    public int Money;
    public CharacterStat mainCharacterStat;
    public string[] usingDollList;
    public FormationDollInfo[] formationDollList;
    public SaveDataBackpckItem[] dollBackpack;
    public SaveDataEventItem[] eventData;
    public DollInstanceData[] usingDIs;
    public MapSavePerlinField[] savedPFields;
}


public class PlayerData : MonoBehaviour
{
    protected bool isReady = false; //�O�_�w�g���J��ƩΧ�����l��

    //public DollData theDollData;

    protected int Money = 0;

    protected CharacterStat mainCharacterStat = new CharacterStat();

    protected List<DollInstanceData> usingDIs = new List<DollInstanceData>();
    protected List<string> usingDollList = new List<string>();
    protected List<FormationDollInfo> formationDollList = new List<FormationDollInfo>();

    protected Dictionary<string, int> dollBackpack = new Dictionary<string, int>();

    //�a�ϰO��
    protected Dictionary<string, MapSaveDataBase> savedMaps = new Dictionary<string, MapSaveDataBase>();

    //�ƥ� Flag 
    protected Dictionary<string, bool> eventData = new Dictionary<string, bool>();

    //==== �����ߤ@ ID ���ͦ�
    static private HashSet<int> usedIds = new HashSet<int>();
    public int GenerateUniqueId()
    {
        int id;
        do
        {
            id = Random.Range(0, int.MaxValue); // �ͦ��H�����
        } while (usedIds.Contains(id)); // �ˬd�O�_�w�g�ϥιL�A�p�G�O�A���s�ͦ�

        usedIds.Add(id); // �N�s�� ID �[�J�w�ϥΪ����X
        return id;
    }

    protected void RegisterUsedID(int _id)
    {
        usedIds.Add(_id);
    }
    protected void UnRegisterUsedID(int _id)
    {
        usedIds.Remove(_id);
    }
    //====


    //���s�s�ɪ���ƪ�l�ơA��  Start() �󦭷|�Q�I�s
    public void InitData()
    {
        print("==== GameSystem �� PlayerData.InitData()");
        Money = 0;
        mainCharacterStat.LV = 1;
        mainCharacterStat.Exp = 0;
        usingDIs.Clear();
        usingDollList.Clear();
        formationDollList.Clear();
        dollBackpack.Clear();
        eventData.Clear();
        savedMaps.Clear();

        usedIds.Clear();

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

    //�s�ɬ���
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

        if (usingDollList.Count > 0)
        {
            data.usingDollList = new string[usingDollList.Count];
            for (int i = 0; i < usingDollList.Count; i++)
            {
                data.usingDollList[i] = usingDollList[i];
            }
        }

        if (formationDollList.Count > 0)
        {
            data.formationDollList = new FormationDollInfo[formationDollList.Count];
            for (int i = 0; i < formationDollList.Count; i++)
            {
                data.formationDollList[i] = formationDollList[i];
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

        if (savedMaps.Count > 0)
        {
            int i = 0;
            data.savedPFields = new MapSavePerlinField[savedMaps.Count];
            foreach (KeyValuePair<string, MapSaveDataBase> k in savedMaps)
            {
                data.savedPFields[i] = (MapSavePerlinField)k.Value;
                i++;
            }
            print("----�a�Ϧs�ɧ���----");
        }

        return data;
    }

    public void LoadSavedData( SaveData data)
    {
        //���M�ťH�קK���Х[���� Bug
        InitData();

        Money = data.Money;
        mainCharacterStat = data.mainCharacterStat;

        if (data.usingDIs != null && data.usingDIs.Length > 0)
        {
            for (int i = 0; i < data.usingDIs.Length; i++)
            {
                RegisterUsedID(data.usingDIs[i].uID);
                AddUsingDI(data.usingDIs[i]);
            }
        }


        if (data.formationDollList != null && data.formationDollList.Length > 0)
        {
            for (int i = 0; i < data.formationDollList.Length; i++)
            {
                AddUsingDoll(data.formationDollList[i].dollID, data.formationDollList[i].group, data.formationDollList[i].index);
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

        if (data.savedPFields !=null && data.savedPFields.Length > 0)
        {
            for (int i=0; i<data.savedPFields.Length; i++)
            {
                savedMaps.Add(data.savedPFields[i].mapName, data.savedPFields[i]);
            }
            print("----�a�ϸ��J����---- ");
        }

    }

    // ����
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

    //================== ����s�� DollInstance �ϥ� ======================================
    public void AddUsingDI(DollInstanceData data)
    {
        usingDIs.Add(data);

        //print("�ثe�� DI �C��");
        //for (int i = 0; i < usingDIs.Count; i++)
        //{
        //    print("----" + usingDIs[i].fullName);
        //}
    }

    public void RemoveUsingDI(DollInstanceData data)
    {
        //TODO: ���B�z ?
        bool br = usingDIs.Remove(data);  //�o����k��?
        print("���� DI ���G: " + br);
    }

    //���եΪ��ɤO�禡�A�M����ӥ@�ɪ��Ҧ� DI
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
        usingDollList.Add(dollID);
        //formationDollList.Add()
        //
        FormationDollInfo fdInfo = new FormationDollInfo();
        fdInfo.dollID = dollID;
        fdInfo.group = group;
        fdInfo.index = index;
        formationDollList.Add(fdInfo);
        //print("--All Formation Doll Lilst --");
        //foreach (FormationDollInfo info in formationDollList)
        //{
        //    print("--" + info.dollID + "(" + info.group + ", " + info.index + ")");
        //}
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
        formationDollList.Clear();
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

    //����a��
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
        if (savedMaps.ContainsKey(name))
        {
            return savedMaps[name];
        }
        return null;
    }
}
