using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�N�Ҧ��C�����|�ϥΨ쪺�a����ưѼưO���_�Ӫ��a��
//��B���H Json �ɮת��覡�ӽs��U�Ӧa�����Ѽ�

[System.Serializable]
public class DungeonListJsonData
{
    public CMazeJsonData[] dungeons;
}

public class DungeonData : MonoBehaviour
{
    public TextAsset[] csvFiles;
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
                print(moStages[t].DungeonID + "_" + moStages[t].Level + "Path Rate: " + moStages[i].PathRate);
                MODungeonData moDungeon;
                if (!allMoDungeons.ContainsKey(moStages[t].DungeonID))
                {
                    moDungeon = new MODungeonData();
                    moDungeon.DungeonID = moStages[t].DungeonID;
                    allMoDungeons.Add(moDungeon.DungeonID, moDungeon);
                }
                else
                    moDungeon = allMoDungeons[moStages[t].DungeonID];
                moDungeon.stageList.Add(moStages[i]);
            }
        }

        for (int i=0; i<jsonFiles.Length; i++)
        {
            //print("�}�l Parse �@�� DungeonData Json");
            DungeonListJsonData dgList = JsonUtility.FromJson<DungeonListJsonData>(jsonFiles[i].text);
            //print("Parse �����A��쪺 Dungeon �ƶq: " + dgList.dungeons.Length);
            for (int j=0; j<dgList.dungeons.Length; j++)
            {
                dgList.dungeons[j].Convert(objRefMap);
                allDungeons.Add(dgList.dungeons[j].ID, dgList.dungeons[j]);
                //print("�[�J�F�a��: " + dgList.dungeons[j].name);
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
}
