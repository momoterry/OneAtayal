using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//負責管理關卡開放順序，以及記錄關卡資料的系統
//關於開放與否的存檔，交給 PlayerData 的 Event 來記錄

[System.Serializable]
public class LevelInfo
{
    public enum LEVEL_TYPE
    {
        SCENE,
        DUNGEON,        //來自 DungeonData 定義
    }
    public string ID;
    public LEVEL_TYPE type;
    public string sceneName;        //當關卡類型是 Scene 時使用，如果是 Dungeon ，作為 Dungeon ID 使用
    public string prefix;
    public string name;
    public int requireLevel;    //-1 表示 ??
}

public class LevelManager : GlobalSystemBase
{
    public LevelInfo[] mainLevels;
    // Start is called before the first frame update

    protected Dictionary<string, int> levelMap = new Dictionary<string, int>();
    protected Dictionary<string, string> sceneLevelMap = new Dictionary<string, string>();

    //void Awake()
    //{
    //    for (int i = 0; i < mainLevels.Length; i++)
    //    {
    //        levelMap.Add(mainLevels[i].ID, i);
    //        sceneLevelMap.Add(mainLevels[i].sceneName, mainLevels[i].ID);
    //    }

    //    //先暴力法開放第一關
    //    //SetLevelOpen(mainLevels[0].ID);
    //    //print("先暴力法開放第一關");
    //}

    public override void InitSystem()
    {
        base.InitSystem();
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
        if (BattleSystem.GetInstance() && BattleSystem.GetInstance().levelID != "")
        {
            return BattleSystem.GetInstance().levelID;
        }
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
            One.LOG("ERROR!! Not level ID : " + levelID);
            return;
        }
        int i = levelMap[levelID];
        if (i + 1 < mainLevels.Length)
        {
            string toOpenLevelID = mainLevels[i + 1].ID;
            SetLevelOpen(toOpenLevelID);
            print("Open New Level : " + toOpenLevelID);
        }

        //TODO: 是否應在此強制存檔? 還是就依賴下一個場景開始時的存檔?
    }

    public bool IsLevelOpen(string levelID)
    {
        if (DebugMenu.IsLevelFree())
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

    public LevelInfo GetLevelInfo(string levelID)
    {
        if (levelMap.ContainsKey(levelID))
        {
            return mainLevels[levelMap[levelID]];
        }
        return null;
    }

    public void DebugClearAllMainLevels()
    {
        for (int i = 0; i < mainLevels.Length; i++)
        {
            SetLevelClear(mainLevels[i].ID);
            print("Clear : " + mainLevels[i].ID);
        }
    }

    //==================================== 直接由 LevelManager 處理關卡載入的實操作
    public void GotoLevel(string levelID, string backScene = "", string backEntrance = "")
    {
        LevelInfo info = GetLevelInfo(levelID);
        if (info == null)
        {
            One.ERROR("GotoLevel Error, no such level ID: " + levelID);
            return;
        }

        switch (info.type)
        {
            case LevelInfo.LEVEL_TYPE.SCENE:
                //print("Goto Level: (SCENE):" + info.sceneName);
                BattleSystem.GetInstance().OnGotoScene(info.sceneName);
                break;
            case LevelInfo.LEVEL_TYPE.DUNGEON:
                //CMazeJsonData data = GameSystem.GetInstance().theDungeonData.GetMazeJsonData(info.sceneName);
                //if (data != null)
                //{
                //    //print("Goto Level: (DUNGEON):" + info.sceneName);
                //    data.levelID = levelID;
                //    foreach (ContinuousMazeData mData in data.battles)
                //    {
                //        mData.levelID = levelID;
                //    }
                //    CMazeJsonPortal.DoLoadJsonMazeScene(data, backScene, backEntrance);
                //}
                CDungeonDataBase data = GameSystem.GetInstance().theDungeonData.GetCDungeonData(info.sceneName);
                if (data != null)
                {
                    //print("Goto Level: (CDungeonDataBase DUNGEON):" + info.sceneName);
                    data.levelID = levelID;
                    foreach (ContinuousBattleDataBase mData in data.battles)
                    {
                        mData.levelID = levelID;
                    }
                    //CMazeJsonPortal.DoLoadJsonMazeScene(data, backScene, backEntrance);
                    DungeonData.DoLoadCDungeonDataScene(data, backScene, backEntrance);
                }
                else
                {
                    One.ERROR("找不到指定的 Dungeon: " + info.sceneName);
                }
                break;
        }

    }

    public LevelInfo GetNextLevel(string levelID)
    {
        for (int i=0; i < (mainLevels.Length-1); i++)
        {
            if (mainLevels[i].ID == levelID)
            {
                return mainLevels[i + 1];
            }
        }
        return null;
    }
}
