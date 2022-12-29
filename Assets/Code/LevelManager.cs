using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//�t�d�޲z���d�}�񶶧ǡA�H�ΰO�����d��ƪ��t��
//����}��P�_���s�ɡA�浹 PlayerData �� Event �ӰO��

[System.Serializable]
public class LevelInfo
{
    public string ID;
    public string sceneName;
    public string prefix;
    public string name;
    public int requireLevel;    //-1 ��� ??
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

        //���ɤO�k�}��Ĥ@��
        //SetLevelOpen(mainLevels[0].ID);
        //print("���ɤO�k�}��Ĥ@��");
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
        print("�o�ӳ������O Level: " + sceneName);
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

        //TODO: �O�_���b���j��s��? �٬O�N�̿�U�@�ӳ����}�l�ɪ��s��?
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
