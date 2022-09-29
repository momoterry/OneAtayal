using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //為了透過 Scene 自動加載 GameSystem
using System.IO;    //存讀檔
using System.Text;  //存讀檔

public class GameSystem : MonoBehaviour
{
    public PlayerData thePlayerData;

    //TODO: 這部份應該改到 PlayerData 中
    private GameObject playerCharacterRef = null;

    //Skill 相關 //TODO: 這部份應該改到 PlayerData 中
    private Dictionary<string, SkillBase> skillMap = new Dictionary<string, SkillBase>();

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
            SaveData(); //建立存檔 !!
        }
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

    static public PlayerData GetPlayerData()
    {
        if (!instance || !instance.thePlayerData)
        {
            print("ERROR !!! GameSystem 找不到 PlayerData !!" + instance);
            return null;
        }
        return instance.thePlayerData;
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
        //print("GameSystem :: LoadData !!.......");

        string filePath = Application.persistentDataPath + "/" + strSaveFile;
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

}
