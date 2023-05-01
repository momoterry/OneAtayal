using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //為了透過 Scene 自動加載 GameSystem
using UnityEngine.Networking;
using System.IO;    //存讀檔
using System.Text;  //存讀檔
using System.Data;
using UnityEngine.InputSystem;
using System;

public class GameSystem : MonoBehaviour
{
    //網路存檔路徑相關
    //public const string urlRoot = "http://localhost/one/oaserver/";
    public const string urlRoot = "http://yeshouse.tplinkdns.com/one/oaserver/";

    public PlayerData thePlayerData;
    public LevelManager theLevelManager;

    public bool isOnlineSave = false;
    public OnlineSaveLoad theOnlineSaveLoad;

    protected bool hasSaveGame = false;

    protected string onlineID = "";
    public const string INVALID_ID = "INVALID_ID";
    protected string nickName = "";

    //TODO: 這部份應該改到 PlayerData 中
    protected GameObject playerCharacterRef = null;

    //Skill 相關 //TODO: 這部份應該改到 PlayerData 中
    protected Dictionary<string, SkillBase> skillMap = new Dictionary<string, SkillBase>();

    //Option 相關 //TODO: 應該移到真正的 PlayerPref 當中
    protected bool useVpadControl = true;


    public bool IsHasSaveGame() { return hasSaveGame; }

    //網路存檔相關
    public string GetID() { return onlineID; }
    public string GetNickName() { return nickName; }

    protected const string PREF_ONLINE_ID = "ONLINE_ID";
    protected const string PREF_NICK_NAME = "NICK_NAME";

    //迷宮小遊戲相關
    protected int MazeUserSize = -1;        // -1 表示玩家沒有設定
    public int GetMazeUserSize() { return MazeUserSize; }
    public void SetMazeUserSize(int _size) { MazeUserSize = _size; }

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
        if (isOnlineSave)
        {
            OnlineInitAsync();
        }
        else
        {
            OfflineInit();
        }
        //OnlineInit();

        //if (!LoadData())
        //{
        //    thePlayerData.InitData();
        //    //SaveData(); //建立存檔 !!     //如果沒有存檔，先保持 New Game 狀態，直到第一次存檔出現 (換關等)
        //    hasSaveGame = false;
        //}
        //else
        //    hasSaveGame = true;
        //thePlayerData.SetDataReady();

#if TOUCH_MOVE
        useVpadControl = false;
#endif
    }

    static public GameSystem GetInstance()
    {
        return instance;
    }

    static public void Ensure()
    {
        if (instance == null)
        {
            print("還沒有創建 GameSystem，需要加載 Scene: GameSystem!!");
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
        }
    }



    protected void OnlineInit()
    {
        ChatGPT.GetKeyStaticAsync();
        if (isOnlineSave)
        {
            onlineID = PlayerPrefs.GetString(PREF_ONLINE_ID, "");
            nickName = PlayerPrefs.GetString(PREF_NICK_NAME, "");
            print("Online ID = " + onlineID + "  Nick Name = " + nickName);

            //檢查 ID 正確性
            if (onlineID != "")
            {
                if (theOnlineSaveLoad.CheckID(onlineID, nickName))
                {
                    print("帳號暱稱檢查通過 .....");
                }
                else
                {
                    print("帳號暱稱檢查失敗，設為錯誤帳號錯誤狀態 .....");
                    onlineID = INVALID_ID;
                    nickName = "";
                }
            }
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

    public void SaveData()
    {
        if (isOnlineSave)
            SaveDataOnlineAsync();
        else
            SaveDataLocal();

        hasSaveGame = true;
    }

    bool LoadData()
    {
        if (isOnlineSave)
            return LoadDataOnline();
        else
            return LoadDataLocal();
    }

    public void DeleteData()
    {
        if (isOnlineSave)
            DeleteDataOnline();
        else
            DeleteDataLocal();
    }

    public bool SetNickName(string _nickName)
    {
        if (!isOnlineSave)
            return false;

        if ( theOnlineSaveLoad.SetNickName(onlineID, _nickName))
        {
            SetAndSaveNickName(_nickName);
            return true;
        }
        return false;
    }

    public bool RetriveAccountByNickname(string _nickName)
    {
        string newID = theOnlineSaveLoad.GetIDByNickName(_nickName);
        if (!newID.StartsWith("ERROR")) {
            print("用暱稱找到有效 ID: " + newID);

            SetAndSaveOnlineID(newID);
            SetAndSaveNickName(_nickName);

            return LoadData();
        }
        return false;
    }

    // ======================================================================

    //Debug 測試用
    public void LoadOnlineAgain()
    {
        onlineID = PlayerPrefs.GetString(PREF_ONLINE_ID, "");
        nickName = PlayerPrefs.GetString(PREF_NICK_NAME, "");
        print("LoadOnlineAgain ID = " + onlineID + ", NickName = " + nickName);
        LoadData();
    }

    protected void SaveDataOnline()
    {
        if (onlineID == "")
        {
            string newID = theOnlineSaveLoad.GetNewID();
            if (newID == "")
            {
                //print("ERROR !! 無法取得 Online ID，改用 Local Save");
                //isOnlineSave = false;
                //SaveDataLocal();
                print("ERROR !! 無法取得 Online ID，顯示錯誤訊息....");
                SystemUI.ShowMessageBox(OnlineFailMessageCB, "線上存檔失敗，請檢查網路狀態....");
                return;
            }
            else
            {
                //PlayerPrefs.SetString("ONLINE_ID", onlineID);
                //PlayerPrefs.Save();
                SetAndSaveOnlineID(newID);
                SetAndSaveNickName("");
                print("取得新的 Online ID 並存到 PlayerPrefs: " + newID);
            }
        }
        else if (onlineID == INVALID_ID)
        {
            print("ERROR !! 在錯誤狀態下進行線上存檔...." + INVALID_ID);
            SystemUI.ShowMessageBox(OnlineFailMessageCB, "無效的帳號，請檢查網路並重啟遊戲....");
            return;
        }

        SaveData theSaveData = thePlayerData.GetSaveData();
        string saveDataStr = JsonUtility.ToJson(theSaveData);

        theOnlineSaveLoad.SaveGameData(onlineID, saveDataStr);
    }

    protected bool LoadDataOnline()
    {
        if (onlineID != "" && onlineID != INVALID_ID)
        {
            string strSave = theOnlineSaveLoad.LoadGameData(onlineID);
            if (strSave == "")
            {
                //ID 有問題，保持錯誤狀態
                print("LoadDataOnline 無資料......設定為錯誤 Online ID" + onlineID);
                onlineID = INVALID_ID;
                nickName = "";
                return false;
            }
            SaveData loadData = JsonUtility.FromJson<SaveData>(strSave);

            thePlayerData.LoadSavedData(loadData);
            return true;
        }
        return false;
    }


    protected void DeleteDataOnline()
    {
        SetAndSaveOnlineID("");
        SetAndSaveNickName("");
        thePlayerData.InitData();
    }

    protected void SetAndSaveOnlineID(string _id)
    {
        onlineID = _id;
        PlayerPrefs.SetString(PREF_ONLINE_ID, onlineID);
        PlayerPrefs.Save();
    }

    protected void SetAndSaveNickName(string _nickname)
    {
        nickName = _nickname;
        PlayerPrefs.SetString(PREF_NICK_NAME, _nickname);
        PlayerPrefs.Save();
    }

    public void OnlineFailMessageCB(MessageBox.RESULT result)
    {
        print("關掉線上錯誤訊息框 ......");
    }


#if SAVE_TO_PLAYERPREFS
    protected void SaveDataLocal()
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

    protected void DeleteDataLocal()
    {
        print("......PlayerPrefs Deleted !!");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        thePlayerData.InitData();
    }

    protected bool LoadDataLocal()
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
    protected void DeleteDataLocal()
    {
        //print("ERROR!! TODO.... DeleteData");
        string filePath = Application.persistentDataPath + "/" + strSaveFile;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        thePlayerData.InitData();
    }


    protected void SaveDataLocal()
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


    protected bool LoadDataLocal()
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

    protected void OfflineInit()
    {
        ChatGPT.GetKeyStaticAsync();
        if (!LoadDataLocal())
        {
            thePlayerData.InitData();
            hasSaveGame = false;
        }
        else
            hasSaveGame = true;
        thePlayerData.SetDataReady();
    }

    protected void OnlineInitAsync()
    {
        onlineID = PlayerPrefs.GetString(PREF_ONLINE_ID, "");
        nickName = PlayerPrefs.GetString(PREF_NICK_NAME, "");
        print("Online ID = " + onlineID + "  Nick Name = " + nickName);

        thePlayerData.InitData();   //無論如何先初始化
        StartCoroutine(OnlineInitProcess());
    }

    protected void SaveDataOnlineAsync()
    {
        StartCoroutine(OnlineSaveProcess());
    }

    //======================= 網路處理程序 ===========================
    //
    public const string urlGetID = "getid.php";
    public const string urlSaveGame = "savegame.php";
    public const string urlLoadGame = "loadgame.php";
    public const string urlSetNickName = "setnickname.php";
    public const string urlCheckID = "checkidnickname.php";
    public const string urlRetrieveAccount = "retrieveaccount.php";

    public const string urlGAME_ID = "game_id";
    public const string urlNICK_NAME = "nickname";
    public const string ONLINE_ERROR_PREFIX = "ERROR";

    protected  IEnumerator OnlineInitProcess()
    {
        //首先是 API key
        string url = urlRoot + "k.k";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            print("API Key 獲取成功 (OnlineInitProcess)");
            ChatGPT.SetAPIKeyEncrypted(www.downloadHandler.text);
        }
        else
        {
            print(ONLINE_ERROR_PREFIX + " : " + url);
            print("ERROR !! API Key 獲取失敗");
        }
        www.Dispose();

        //接下來檢查 ID 跟 nickname

        //檢查 ID 正確性
        if (onlineID != "")
        {
            url = urlRoot + urlCheckID + "?" + urlGAME_ID + "=" + onlineID + "&" + urlNICK_NAME + "=" + nickName;
            www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            bool isOK = false;
            string erMsg = "";
            if (www.result == UnityWebRequest.Result.Success)
            {
                if (www.downloadHandler.text.StartsWith("SUCCESS"))
                {
                    print("帳號暱稱檢查通過 .....(OnlineInitProcess)");
                    isOK = true;
                }
                else
                    erMsg += www.downloadHandler.text;
            }
            else
            {
                erMsg += www.error;
            }

            if (!isOK)
            {
                print("帳號暱稱檢查失敗 " + erMsg);
                onlineID = INVALID_ID;
                nickName = "";
                SystemUI.ShowMessageBox(null, "帳號暱稱檢查失敗 .... 請檢查網路或開新帳號 " + erMsg);
            }
            www.Dispose();
        }

        // =============== 載入存檔 ====================
        if (onlineID != INVALID_ID && onlineID != "")
        {
            url = urlRoot + urlLoadGame + "?" + urlGAME_ID + "=" + onlineID;
            www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            string errorStr = "";
            if (www.result == UnityWebRequest.Result.Success)
            {
                string strSave = www.downloadHandler.text;
                if (strSave == "" || strSave.StartsWith(ONLINE_ERROR_PREFIX))
                {
                    //ID 有問題，保持錯誤狀態
                    print("載入存檔有誤......設定為錯誤 Online ID" + onlineID);
                    onlineID = INVALID_ID;
                    nickName = "";
                    errorStr = "載入存檔內容錯誤...." + strSave;
                }
                else
                {
                    print("載入存檔成功......(OnlineInitProcess) " + onlineID);
                    SaveData loadData = JsonUtility.FromJson<SaveData>(strSave);
                    thePlayerData.LoadSavedData(loadData);
                    
                    thePlayerData.SetDataReady();
                    hasSaveGame = true;
                }
            }
            else
            {
                print("載入存檔失敗......" + www.error);
                onlineID = INVALID_ID;
                nickName = "";
                errorStr = "載入存檔錯誤...." + www.error;
            }
            if (errorStr != "")
            {
                SystemUI.ShowMessageBox(null, errorStr);
            }

            www.Dispose();
        }

    }


    protected IEnumerator OnlineSaveProcess()
    {
        string url;
        UnityWebRequest www;
        SaveData theSaveData = thePlayerData.GetSaveData();
        string dataStr = JsonUtility.ToJson(theSaveData);

        if (onlineID == "")
        {
            url = urlRoot + urlGetID;
            www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {

                string id = www.downloadHandler.text;
                print("獲得的新 ID 是 (OnlineSaveProcess)：" + id);
                SetAndSaveOnlineID(id);
                SetAndSaveNickName("");
            }
            else
            {
                print("ERROR!!!! OnlineSaveLoad::GetNewID 失敗 ...." + url);
                print(www.error);
                SystemUI.ShowMessageBox(null, "存檔失敗，無法建新帳號");
                yield break;
            }
            www.Dispose();
        }

        url = urlRoot + urlSaveGame;
        www = new UnityWebRequest(url, "POST");

        // 將進度數據作為參數添加到請求中
        WWWForm form = new WWWForm();
        form.AddField("game_id", onlineID);
        form.AddField("game_data", dataStr);
        www.uploadHandler = new UploadHandlerRaw(form.data);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            print("ERROR !! 存檔失敗 (OnlineSaveProcess) !!");
            print(www.error);
            SystemUI.ShowMessageBox(null, "存檔失敗" + www.error);
        }
        else
        {
            print("OnlineSaveProcess 存檔成功回傳資訊:\n" + www.downloadHandler.text);
            SystemUI.ShowMessageBox(null, "存檔成功 !!");
        }

        www.Dispose();
    }

}
