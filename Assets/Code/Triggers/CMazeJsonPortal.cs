using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[System.Serializable]
public class ContinuousMazeJsonData : ContinuousMazeData
{
    public string dungeonEnemyManager_ID;

    public void Convert(Dictionary<string, GameObject> refMap)
    {
        if (dungeonEnemyManager_ID != null)
        {
            GameObject o = refMap[dungeonEnemyManager_ID];
            if (o)
            {
                dungeonEnemyManager = o.GetComponent<DungeonEnemyManager>();
                //Debug.Log("有有有，有看到 DungeonManager: " + dungeonEnemyManager_ID);
            }
        }

    }
}

[System.Serializable]
public class CMazeJsonData  //一整個地城的資料內容 (包含每一層的迷宮)
{
    public string ID;
    public string name;
    public ContinuousMazeJsonData[] levels;
    public void Convert(Dictionary<string, GameObject> refMap)
    {
        Debug.Log("levels: " + levels);
        for (int i = 0; i < levels.Length; i++)
        {
            Debug.Log("level: " + i + " => " + levels[i]);
            levels[i].Convert(refMap);
        }
    }
}

public class CMazeJsonPortal : ScenePortal
{
    public TextAsset jsonFile;
    public GameObject[] objectRefs;    

    protected CMazeJsonData mazeData;
    protected Dictionary<string, GameObject> objRefMap = new Dictionary<string, GameObject>();

    void Start()
    {

        if (!jsonFile)
            return;

        mazeData = JsonUtility.FromJson<CMazeJsonData>(jsonFile.text);

        for (int i=0; i< objectRefs.Length; i++)
        {
            objRefMap.Add(objectRefs[i].name, objectRefs[i]);
        }

        mazeData.Convert(objRefMap);
    }

    public void SetupCMazeJsonData(CMazeJsonData data)
    {
        mazeData = data;
    }

    protected override void DoTeleport()
    {
        if (mazeData.levels.Length > 0 && mazeData.levels[0].scene != "")
        {
            ContinuousBattleManager.StartNewBattle(mazeData.levels);

            sceneName = mazeData.levels[0].scene;
            base.DoTeleport();
        }
    }
}
