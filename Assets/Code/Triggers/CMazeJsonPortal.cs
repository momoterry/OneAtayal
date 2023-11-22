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
public class CMazeJsonData
{
    public ContinuousMazeJsonData[] levels;
}

public class CMazeJsonPortal : ScenePortal
{
    public TextAsset jsonFile;
    public GameObject[] objectRefs;    

    protected CMazeJsonData mazeData;
    protected Dictionary<string, GameObject> objRefMap = new Dictionary<string, GameObject>();

    void Start()
    {
        //CMazeJsonData jData = new CMazeJsonData();
        //jData.levels = new ContinuousMazeDataSimple[2];
        //for (int i=0; i< jData.levels.Length; i++)
        //{
        //    jData.levels[i] = new ContinuousMazeDataSimple();
        //    jData.levels[i].puzzleWidth = 10 + i;
        //    jData.levels[i].puzzleHeight = 10 + 2 * i;
        //}
        //string str = JsonUtility.ToJson(jData, true);
        //print(str);

        //print("CMazeJsonPortal: " + jsonFile.text);
        mazeData = JsonUtility.FromJson<CMazeJsonData>(jsonFile.text);

        for (int i=0; i< objectRefs.Length; i++)
        {
            objRefMap.Add(objectRefs[i].name, objectRefs[i]);
        }

        for (int i = 0; i < mazeData.levels.Length; i++)
        {
            mazeData.levels[i].Convert(objRefMap);
        }

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
