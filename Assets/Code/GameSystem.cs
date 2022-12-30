using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //為了透過 Scene 自動加載 GameSystem
using System.IO;    //存讀檔
using System.Text;  //存讀檔

public class GameSystem : MonoBehaviour
{
    public PlayerData thePlayerData;
    public LevelManager theLevelManager;

    //TODO: 這部份應該改到 PlayerData 中
    private GameObject playerCharacterRef = null;

    //Skill 相關 //TODO: 這部份應該改到 PlayerData 中
    private Dictionary<string, SkillBase> skillMap = new Dictionary<string, SkillBase>();

    //Option 相關 //TODO: 應該移到真正的 PlayerPref 當中
    protected bool useVpadControl = true;

    protected string strSaveFile = "mySave.txt";

    static private GameSystem instance;

    public GameSystem() : base()
    {
        print("GameSystem : 我被創建了!!!");
        if (instance != null)
            print("ERROR !! 超過一份 Game System 存在 ");
        instance = this;
        print("GameSystem 創建完成");
    }

    private void Awake()
    {
        //print("我被喚醒了");
        if (!LoadData())
        {
            thePlayerData.InitData();
            SaveData(); //建立存檔 !!
        }
        thePlayerData.SetDataReady();

#if TOUCH_MOVE
        useVpadControl = false;
#endif
    }

    static public GameSystem GetInstance()
    {
        //print("GetInstance() 結果 = " + instance);
        return instance;
    }

    static public void Ensure()
    {
        if (instance == null)
        {
            print("還沒有創建 GameSystem，需要加載 Scene: GameSystem!!");
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
            //print("加載完, instace =" + instance);
        }
    }

    static public void SetUseVPad( bool useVPad ) { instance.useVpadControl = useVPad; }
    static public bool IsUseVpad() { return instance.useVpadControl; }

    static public PlayerData GetPlayerData()
    {
        if (!instance || !instance.thePlayerData)
        {
            print("ERROR !!! GameSystem 找不到 PlayerData !!" + instance);
            return null;
        }
        return instance.thePlayerData;
    }

    static public LevelManager GetLevelManager()
    {
        if (!instance || !instance.theLevelManager)
        {
            print("ERROR !!! GameSystem 找不到 Level Manager !!" + instance);
            return null;
        }
        return instance.theLevelManager;
    }

    public void SetPlayerCharacterRef( GameObject objRef)
    {
        playerCharacterRef = objRef;
    }

    public GameObject GetPlayerCharacterRef()
    {
        return playerCharacterRef;
    }

    public void SetPlayerSkillRef( string skillStr, SkillBase skillRef)
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

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;  //強迫 Android 開放 !!

        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if SAVE_TO_PLAYERPREFS
    public void SaveData()
    {
        PlayerPrefs.DeleteAll();

        string playerName = "DefTerry_";
        SaveData theSaveData = thePlayerData.GetSaveData();

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt(playerName+"Money", theSaveData.Money);
        //print("Money: " + theSaveData.Money);
        PlayerPrefs.SetInt(playerName+"LV", theSaveData.mainCharacterStat.LV);
        //print("LV: " + theSaveData.mainCharacterStat.LV);
        PlayerPrefs.SetInt(playerName + "EXP", theSaveData.mainCharacterStat.LV);

        //使用中的 Doll
        if (theSaveData.usingDollList != null)
        {
            int usingDollSize = theSaveData.usingDollList.Length;
            PlayerPrefs.SetInt(playerName + "DollListSize", usingDollSize);
            for (int i = 0; i < usingDollSize; i++)
            {
                PlayerPrefs.SetString(playerName + "DollList_" + i, theSaveData.usingDollList[i]);
            }
        }
        // Doll 背包
        if (theSaveData.dollBackpack != null)
        {
            int backPackSize = theSaveData.dollBackpack.Length;
            PlayerPrefs.SetInt(playerName + "BackPackSize", backPackSize);
            for (int i=0; i< backPackSize; i++)
            {
                PlayerPrefs.SetString(playerName + "BackPack_ID_" + i, theSaveData.dollBackpack[i].ID);
                PlayerPrefs.SetInt(playerName + "BackPack_num_" + i, theSaveData.dollBackpack[i].num);
            }
        }
        //Event
        if (theSaveData.eventData != null)
        {
            int eventDataSize = theSaveData.eventData.Length;
            PlayerPrefs.SetInt(playerName + "EventDataSize", eventDataSize);
            //print("...... Save EventDataSize in PlayerPrefs: " + eventDataSize);
            for (int i=0; i<eventDataSize; i++)
            {
                PlayerPrefs.SetString(playerName + "Event_ID_" + i, theSaveData.eventData[i].Event);
                PlayerPrefs.SetInt(playerName + "Event_Status_" + i, theSaveData.eventData[i].status ? 1:0);
                //print("......Save Event Data in PlayerPrefs: " + theSaveData.eventData[i].Event + " status = " + theSaveData.eventData[i].status);
            }
        }

        PlayerPrefs.Save();
        print("......PlayerPrefs Save Done !!");
    }

    public void DeleteData()
    {
        print("......PlayerPrefs Deleted !!");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        thePlayerData.InitData();
    }

    public bool LoadData()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "");
        if (playerName == "")
        {
            print("........ No PlayerPrefs Data !!");
            return false;
        }

        print("...... Found Saved PlayerPrefs, PlayerName = " + playerName);

        SaveData loadData = new SaveData();
        loadData.mainCharacterStat = new CharacterStat();
        loadData.Money = PlayerPrefs.GetInt(playerName+"Money", 0);
        loadData.mainCharacterStat.LV = PlayerPrefs.GetInt(playerName+"LV", 0);
        loadData.mainCharacterStat.Exp = PlayerPrefs.GetInt(playerName+"EXP", 0);

        //使用中的 Doll
        int usingDollSize = PlayerPrefs.GetInt(playerName + "DollListSize", 0);
        if (usingDollSize > 0)
        {
            loadData.usingDollList = new string[usingDollSize];
            for (int i=0; i<usingDollSize; i++)
            {
                loadData.usingDollList[i] = PlayerPrefs.GetString(playerName + "DollList_" + i, "");
            }
        }
        // Doll 背包
        int backPackSize = PlayerPrefs.GetInt(playerName + "BackPackSize", 0);
        if (backPackSize > 0)
        {
            loadData.dollBackpack = new SaveDataBackpckItem[backPackSize];
            for (int i=0; i<backPackSize; i++)
            {
                loadData.dollBackpack[i].ID = PlayerPrefs.GetString(playerName + "BackPack_ID_" + i, "");
                loadData.dollBackpack[i].num = PlayerPrefs.GetInt(playerName + "BackPack_num_" + i, 0);
            }
        }
        //Event
        int eventDataSize = PlayerPrefs.GetInt(playerName + "EventDataSize", 0);
        //print(".....LoadData, eventDataSize = " + eventDataSize);
        if (eventDataSize > 0)
        {
            loadData.eventData = new SaveDataEventItem[eventDataSize];
            for (int i=0; i< eventDataSize; i++)
            {
                loadData.eventData[i].Event = PlayerPrefs.GetString(playerName + "Event_ID_" + i, "");
                loadData.eventData[i].status = (PlayerPrefs.GetInt(playerName + "Event_Status_" + i, 0) == 1);
                //print("......Found Event Data in PlayerPrefs: " + loadData.eventData[i].Event + " status = " + loadData.eventData[i].status);
            }
        }

        thePlayerData.LoadSavedData(loadData);

        return true;
    }

#else
    public void DeleteData()
    {
        //print("ERROR!! TODO.... DeleteData");
        string filePath = Application.persistentDataPath + "/" + strSaveFile;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        thePlayerData.InitData();
    }


    public void SaveData()
    {
        // 測試存檔
        //print("GameSystem :: SaveData !!.......");
        //測試存檔
        //const string strSaveFile = "mySave.txt";
        string filePath = Application.persistentDataPath + "/" + strSaveFile;


        SaveData theSaveData = thePlayerData.GetSaveData();
        string saveDataStr = JsonUtility.ToJson(theSaveData);
        //print(saveDataStr);
        //print("====================");


        //print("即將存檔到: " + filePath);
        byte[] rawData = Encoding.UTF8.GetBytes(saveDataStr);
        //File.WriteAllBytes(filePath, rawData);
        File.WriteAllBytesAsync(filePath, rawData);
    }


    public bool LoadData()
    {
        string filePath = Application.persistentDataPath + "/" + strSaveFile;
        print("GameSystem :: Try LoadData !! " + filePath);

        if ( !File.Exists(filePath)) 
        { 
            return false;
        }

        //print("有找到存檔，開始讀取 .......");
        byte[] rawData = File.ReadAllBytes(filePath);
        string strSave = Encoding.UTF8.GetString(rawData);

        //print(strSave);
        SaveData loadData = JsonUtility.FromJson<SaveData>(strSave);

        thePlayerData.LoadSavedData(loadData);

        return true;
    }
#endif
}
