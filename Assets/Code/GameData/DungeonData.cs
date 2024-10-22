using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基本的「一組地城資料」，相對於一組 ContinuousBattleDataBase
[System.Serializable]
public class CDungeonDataBase  //一整個地城的資料內容 (包含每一層的迷宮)
{
    public string ID;
    public string name;
    public ContinuousBattleDataBase[] battles;
    public string levelID;                          //如果是一個地城 level 的時候才會有值
}

//將所有遊戲中會使用到的地城資料參數記錄起來的地方
//初步先以 Json 檔案的方式來編輯各個地城的參數

[System.Serializable]
public class DungeonListJsonData
{
    public CMazeJsonData[] dungeons;
}


public class DungeonData : GlobalSystemBase
{
    public TextAsset[] csvFiles;
    //public TextAsset[] csvGamefiles;
    public TextAsset[] jsonFiles;
    public GameObject[] objectRefs;

    public CDungueonDataContainterBase[] dungeionContainters;

    protected Dictionary<string , CMazeJsonData> allMazeJsonDungeons = new Dictionary<string, CMazeJsonData>();
    protected Dictionary<string, MODungeonData> allMoDungeons = new Dictionary<string, MODungeonData>();
    protected Dictionary<string, GameObject> objRefMap = new Dictionary<string, GameObject>();

    protected Dictionary<string, CDungeonDataBase> allDungeons = new();

    public override void InitSystem()
    {
        base.InitSystem();
        for (int i = 0; i < objectRefs.Length; i++)
        {
            objRefMap.Add(objectRefs[i].name, objectRefs[i]);
        }

        for (int i = 0; i < csvFiles.Length; i++)
        {
            MODungeonStageData[] moStages = CSVReader.FromCSV<MODungeonStageData>(csvFiles[i].text);
            for (int t = 0; t < moStages.Length; t++)
            {
                //print(moStages[t].DungeonID + "_" + moStages[t].Level + "Diff End: " + moStages[i].DifficultEnd);
                MODungeonData moDungeon;
                if (!allMoDungeons.ContainsKey(moStages[t].DungeonID))
                {
                    moDungeon = new MODungeonData();
                    moDungeon.DungeonID = moStages[t].DungeonID;
                    allMoDungeons.Add(moDungeon.DungeonID, moDungeon);
                }
                else
                    moDungeon = allMoDungeons[moStages[t].DungeonID];
                moDungeon.stageList.Add(moStages[t]);
                //print("reward: " + moStages[t].Reward1);
            }
        }

        for (int i = 0; i < jsonFiles.Length; i++)
        {
            //print("開始 Parse 一個 DungeonData Json");
            DungeonListJsonData dgList = JsonUtility.FromJson<DungeonListJsonData>(jsonFiles[i].text);
            //print("Parse 完成，找到的 Dungeon 數量: " + dgList.dungeons.Length);
            for (int j = 0; j < dgList.dungeons.Length; j++)
            {
                dgList.dungeons[j].Convert(objRefMap);
                allMazeJsonDungeons.Add(dgList.dungeons[j].ID, dgList.dungeons[j]);
                //print("加入了地城: " + dgList.dungeons[j].name);

                allDungeons.Add(dgList.dungeons[j].ID, dgList.dungeons[j].ToCDungeonData());
            }
        }

        for (int i = 0; i < dungeionContainters.Length; i++) 
        {
            CDungeonDataBase[] datas = dungeionContainters[i].GetDungeons();
            foreach (CDungeonDataBase data in datas)
            {
                allDungeons.Add(data.ID, data);
            }
        }
    }

    public CMazeJsonData GetMazeJsonData(string ID)
    {
        if (allMazeJsonDungeons.ContainsKey(ID))
        {
            return allMazeJsonDungeons[ID];
        }
        return null;
    }

    public MODungeonData GetMODungeonData(string ID)
    {
        if (allMoDungeons.ContainsKey(ID))
        {
            return allMoDungeons[ID];
        }
        return null;
    }

    public CDungeonDataBase GetCDungeonData(string ID)
    {
        if (allDungeons.ContainsKey(ID))
        {
            return allDungeons[ID];
        }
        return null;
    }

    public static void DoLoadCDungeonDataScene(CDungeonDataBase data, string backScene = "", string backEntrance = "")
    {
        ContinuousBattleManager.StartNewBattle(data.battles);

        string sceneName = data.battles[0].scene;
        if (backScene != "")
        {
            BattleSystem.GetInstance().OnGotoSceneWithBack(sceneName, "", backScene, backEntrance);
            //BattleSystem.GetInstance().OnGotoScene(sceneName, backEntrance);
        }
        else
        {
            BattleSystem.GetInstance().OnGotoScene(sceneName, "");
        }
    }
}
