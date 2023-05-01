using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //���F�z�L Scene �۰ʥ[�� GameSystem
using UnityEngine.Networking;
using System.IO;    //�sŪ��
using System.Text;  //�sŪ��
using System.Data;
using UnityEngine.InputSystem;
using System;

public class GameSystem : MonoBehaviour
{
    //�����s�ɸ��|����
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

    //TODO: �o�������ӧ�� PlayerData ��
    protected GameObject playerCharacterRef = null;

    //Skill ���� //TODO: �o�������ӧ�� PlayerData ��
    protected Dictionary<string, SkillBase> skillMap = new Dictionary<string, SkillBase>();

    //Option ���� //TODO: ���Ӳ���u���� PlayerPref ��
    protected bool useVpadControl = true;


    public bool IsHasSaveGame() { return hasSaveGame; }

    //�����s�ɬ���
    public string GetID() { return onlineID; }
    public string GetNickName() { return nickName; }

    protected const string PREF_ONLINE_ID = "ONLINE_ID";
    protected const string PREF_NICK_NAME = "NICK_NAME";

    //�g�c�p�C������
    protected int MazeUserSize = -1;        // -1 ��ܪ��a�S���]�w
    public int GetMazeUserSize() { return MazeUserSize; }
    public void SetMazeUserSize(int _size) { MazeUserSize = _size; }

    protected string strSaveFile = "mySave.txt";

    static private GameSystem instance;

    public GameSystem() : base()
    {
        print("GameSystem : �ڳQ�ЫؤF!!!");
        if (instance != null)
            print("ERROR !! �W�L�@�� Game System �s�b ");
        instance = this;
        print("GameSystem �Ыا���");
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
        //    //SaveData(); //�إߦs�� !!     //�p�G�S���s�ɡA���O�� New Game ���A�A����Ĥ@���s�ɥX�{ (������)
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
            print("�٨S���Ы� GameSystem�A�ݭn�[�� Scene: GameSystem!!");
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

            //�ˬd ID ���T��
            if (onlineID != "")
            {
                if (theOnlineSaveLoad.CheckID(onlineID, nickName))
                {
                    print("�b���ʺ��ˬd�q�L .....");
                }
                else
                {
                    print("�b���ʺ��ˬd���ѡA�]�����~�b�����~���A .....");
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
            print("ERROR !!! GameSystem �䤣�� PlayerData !!" + instance);
            return null;
        }
        return instance.thePlayerData;
    }

    static public LevelManager GetLevelManager()
    {
        if (!instance || !instance.theLevelManager)
        {
            print("ERROR !!! GameSystem �䤣�� Level Manager !!" + instance);
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
        Application.targetFrameRate = 300;  //�j�� Android �}�� !!

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
            print("�μʺ٧�즳�� ID: " + newID);

            SetAndSaveOnlineID(newID);
            SetAndSaveNickName(_nickName);

            return LoadData();
        }
        return false;
    }

    // ======================================================================

    //Debug ���ե�
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
                //print("ERROR !! �L�k���o Online ID�A��� Local Save");
                //isOnlineSave = false;
                //SaveDataLocal();
                print("ERROR !! �L�k���o Online ID�A��ܿ��~�T��....");
                SystemUI.ShowMessageBox(OnlineFailMessageCB, "�u�W�s�ɥ��ѡA���ˬd�������A....");
                return;
            }
            else
            {
                //PlayerPrefs.SetString("ONLINE_ID", onlineID);
                //PlayerPrefs.Save();
                SetAndSaveOnlineID(newID);
                SetAndSaveNickName("");
                print("���o�s�� Online ID �æs�� PlayerPrefs: " + newID);
            }
        }
        else if (onlineID == INVALID_ID)
        {
            print("ERROR !! �b���~���A�U�i��u�W�s��...." + INVALID_ID);
            SystemUI.ShowMessageBox(OnlineFailMessageCB, "�L�Ī��b���A���ˬd�����í��ҹC��....");
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
                //ID �����D�A�O�����~���A
                print("LoadDataOnline �L���......�]�w�����~ Online ID" + onlineID);
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
        print("�����u�W���~�T���� ......");
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

        //�ϥΤ��� Doll
        if (theSaveData.usingDollList != null)
        {
            int usingDollSize = theSaveData.usingDollList.Length;
            PlayerPrefs.SetInt(playerName + "DollListSize", usingDollSize);
            for (int i = 0; i < usingDollSize; i++)
            {
                PlayerPrefs.SetString(playerName + "DollList_" + i, theSaveData.usingDollList[i]);
            }
        }
        // Doll �I�]
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

        //�ϥΤ��� Doll
        int usingDollSize = PlayerPrefs.GetInt(playerName + "DollListSize", 0);
        if (usingDollSize > 0)
        {
            loadData.usingDollList = new string[usingDollSize];
            for (int i=0; i<usingDollSize; i++)
            {
                loadData.usingDollList[i] = PlayerPrefs.GetString(playerName + "DollList_" + i, "");
            }
        }
        // Doll �I�]
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
        // ���զs��
        //print("GameSystem :: SaveData !!.......");
        //���զs��
        //const string strSaveFile = "mySave.txt";
        string filePath = Application.persistentDataPath + "/" + strSaveFile;


        SaveData theSaveData = thePlayerData.GetSaveData();
        string saveDataStr = JsonUtility.ToJson(theSaveData);
        //print(saveDataStr);
        //print("====================");


        //print("�Y�N�s�ɨ�: " + filePath);
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

        //print("�����s�ɡA�}�lŪ�� .......");
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

        thePlayerData.InitData();   //�L�צp�����l��
        StartCoroutine(OnlineInitProcess());
    }

    protected void SaveDataOnlineAsync()
    {
        StartCoroutine(OnlineSaveProcess());
    }

    //======================= �����B�z�{�� ===========================
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
        //�����O API key
        string url = urlRoot + "k.k";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            print("API Key ������\ (OnlineInitProcess)");
            ChatGPT.SetAPIKeyEncrypted(www.downloadHandler.text);
        }
        else
        {
            print(ONLINE_ERROR_PREFIX + " : " + url);
            print("ERROR !! API Key �������");
        }
        www.Dispose();

        //���U���ˬd ID �� nickname

        //�ˬd ID ���T��
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
                    print("�b���ʺ��ˬd�q�L .....(OnlineInitProcess)");
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
                print("�b���ʺ��ˬd���� " + erMsg);
                onlineID = INVALID_ID;
                nickName = "";
                SystemUI.ShowMessageBox(null, "�b���ʺ��ˬd���� .... ���ˬd�����ζ}�s�b�� " + erMsg);
            }
            www.Dispose();
        }

        // =============== ���J�s�� ====================
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
                    //ID �����D�A�O�����~���A
                    print("���J�s�ɦ��~......�]�w�����~ Online ID" + onlineID);
                    onlineID = INVALID_ID;
                    nickName = "";
                    errorStr = "���J�s�ɤ��e���~...." + strSave;
                }
                else
                {
                    print("���J�s�ɦ��\......(OnlineInitProcess) " + onlineID);
                    SaveData loadData = JsonUtility.FromJson<SaveData>(strSave);
                    thePlayerData.LoadSavedData(loadData);
                    
                    thePlayerData.SetDataReady();
                    hasSaveGame = true;
                }
            }
            else
            {
                print("���J�s�ɥ���......" + www.error);
                onlineID = INVALID_ID;
                nickName = "";
                errorStr = "���J�s�ɿ��~...." + www.error;
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
                print("��o���s ID �O (OnlineSaveProcess)�G" + id);
                SetAndSaveOnlineID(id);
                SetAndSaveNickName("");
            }
            else
            {
                print("ERROR!!!! OnlineSaveLoad::GetNewID ���� ...." + url);
                print(www.error);
                SystemUI.ShowMessageBox(null, "�s�ɥ��ѡA�L�k�طs�b��");
                yield break;
            }
            www.Dispose();
        }

        url = urlRoot + urlSaveGame;
        www = new UnityWebRequest(url, "POST");

        // �N�i�׼ƾڧ@���ѼƲK�[��ШD��
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
            print("ERROR !! �s�ɥ��� (OnlineSaveProcess) !!");
            print(www.error);
            SystemUI.ShowMessageBox(null, "�s�ɥ���" + www.error);
        }
        else
        {
            print("OnlineSaveProcess �s�ɦ��\�^�Ǹ�T:\n" + www.downloadHandler.text);
            SystemUI.ShowMessageBox(null, "�s�ɦ��\ !!");
        }

        www.Dispose();
    }

}
