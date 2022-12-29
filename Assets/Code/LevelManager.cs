using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//負責管理關卡開放順序，以及記錄關卡資料的系統
//關於開放與否的存檔，交給 PlayerData 的 Event 來記錄

[System.Serializable]
public class LevelInfo
{
    public string ID;
    public string sceneName;
    public string prefix;
    public string name;
    public int requireLevel;    //-1 表示 ??
}

public class LevelManager : MonoBehaviour
{
    public LevelInfo[] mainLevels;
    // Start is called before the first frame update

    protected Dictionary<string, int> levelMap = new Dictionary<string, int>();
    protected Dictionary<string, string> sceneLevelMap = new Dictionary<string, string>();

    void Awake()
    {
        for (int i = 0; i < mainLevels.Length; i++)
        {
            levelMap.Add(mainLevels[i].ID, i);
            sceneLevelMap.Add(mainLevels[i].sceneName, mainLevels[i].ID);
        }

        //先暴力法開放第一關
        //SetLevelOpen(mainLevels[0].ID);
        //print("先暴力法開放第一關");
    }

    public void InitFirstLevel()
    {
        SetLevelOpen(mainLevels[0].ID);
    }

    public string GetCurrLevelID()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneLevelMap.ContainsKey(sceneName))
        {
            return sceneLevelMap[sceneName];
        }
        print("這個場景不是 Level: " + sceneName);
        return "";
    }

    protected string GetOpenEvent(string levelID)
    {
        return "LEVEL_" + levelID + "_OPEN";
    }

    protected string GetClearEvent(string levelID)
    {
        return "LEVEL_" + levelID + "_CLEAR";
    }

    public void SetLevelOpen(string levelID)
    {
        GameSystem.GetPlayerData().SaveEvent(GetOpenEvent(levelID), true);
    }

    public void SetLevelClear(string levelID)
    {
        string str = GetClearEvent(levelID);
        GameSystem.GetPlayerData().SaveEvent(str, true);

        if (!levelMap.ContainsKey(levelID))
        {
            print("ERROR!! Not level ID : " + levelID);
            return;
        }
        int i = levelMap[levelID];
        if (i+1 < mainLevels.Length)
        {
            string toOpenLevelID = mainLevels[i + 1].ID;
            SetLevelOpen(toOpenLevelID);
            print("Open New Level : " + toOpenLevelID);
        }

        //TODO: 是否應在此強制存檔? 還是就依賴下一個場景開始時的存檔?
    }

    public bool IsLevelOpen(string levelID)
    {
        if (DebugMenu.GetIsLevelFree())
            return true;
        string str = GetOpenEvent(levelID);
        bool isOpen = GameSystem.GetPlayerData().GetEvent(str);
        return isOpen;
    }

    public bool IsLevelClear(string levelID)
    {
        string str = GetClearEvent(levelID);
        bool isClear = GameSystem.GetPlayerData().GetEvent(str);
        return isClear;
    }

    public LevelInfo GetLevelInfo( string levelID)
    {
        if (levelMap.ContainsKey(levelID))
        {
            return mainLevels[levelMap[levelID]];
        }
        return null;
    }

    public void DebugClearAllMainLevels()
    {
        for (int i=0; i<mainLevels.Length; i++)
        {
            SetLevelClear(mainLevels[i].ID);
            print("Clear : " + mainLevels[i].ID);
        }
    }

}
