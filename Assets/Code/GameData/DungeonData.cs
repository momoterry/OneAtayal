using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//盢┮Τ笴栏い穦ㄏノ戈把计癘魁癬ㄓよ
//˙ Json 郎よΑㄓ絪胯把计

[System.Serializable]
public class DungeonListJsonData
{
    public CMazeJsonData[] dungeons;
}

public class DungeonData : MonoBehaviour
{
    public TextAsset[] csvFiles;
    //public TextAsset[] csvGamefiles;
    public TextAsset[] jsonFiles;
    public GameObject[] objectRefs;

    protected Dictionary<string , CMazeJsonData> allDungeons = new Dictionary<string, CMazeJsonData>();
    protected Dictionary<string, MODungeonData> allMoDungeons = new Dictionary<string, MODungeonData>();
    protected Dictionary<string, GameObject> objRefMap = new Dictionary<string, GameObject>();

    private void Awake()
    {
        for (int i = 0; i < objectRefs.Length; i++)
        {
            objRefMap.Add(objectRefs[i].name, objectRefs[i]);
        }

        for (int i=0; i<csvFiles.Length; i++)
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
            }
        }

        //for (int i = 0; i < csvGamefiles.Length; i++)
        //{
        //    MOStageGameplayData_Simple[] moGames = CSVReader.FromCSV<MOStageGameplayData_Simple>(csvGamefiles[i].text);
        //    for (int t = 0; t < moGames.Length; t++)
        //    {
        //        //print("Game " + moGames[t].DungeonID + "_" + moGames[t].Level);
        //        if (allMoDungeons.ContainsKey(moGames[t].DungeonID))
        //        {
        //            MODungeonData myDungeon = allMoDungeons[moGames[t].DungeonID];
        //            MODungeonStageData myStage = myDungeon.stageList[moGames[t].Level-1];
        //            if (myStage == null)
        //                print("ERROR!!!! 硂 Stage ぃ " + moGames[t].DungeonID + " : " + moGames[t].Level);
        //        }
        //        else
        //        {
        //            print("ERROR!!!! 硂 Dungeon ぃ: " + moGames[t].DungeonID);
        //        }
        //        //moDungeon.stageList.Add(moGames[t]);
        //    }
        //}

        for (int i=0; i<jsonFiles.Length; i++)
        {
            //print("秨﹍ Parse  DungeonData Json");
            DungeonListJsonData dgList = JsonUtility.FromJson<DungeonListJsonData>(jsonFiles[i].text);
            //print("Parse ЧΘт Dungeon 计秖: " + dgList.dungeons.Length);
            for (int j=0; j<dgList.dungeons.Length; j++)
            {
                dgList.dungeons[j].Convert(objRefMap);
                allDungeons.Add(dgList.dungeons[j].ID, dgList.dungeons[j]);
                //print(": " + dgList.dungeons[j].name);
            }
        }
    }

    public CMazeJsonData GetMazeJsonData(string ID)
    {
        if (allDungeons.ContainsKey(ID))
        {
            return allDungeons[ID];
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
}
