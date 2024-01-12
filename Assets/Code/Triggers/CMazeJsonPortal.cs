using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[System.Serializable]
public class ContinuousMazeJsonData : ContinuousMazeData
{
    public string dungeonEnemyManager_ID;
    public string initGampleyRef_ID;

    public void Convert(Dictionary<string, GameObject> refMap)
    {
        if (dungeonEnemyManager_ID != null && dungeonEnemyManager_ID != "")
        {
            GameObject o = refMap[dungeonEnemyManager_ID];
            if (o)
            {
                dungeonEnemyManager = o.GetComponent<DungeonEnemyManager>();
                //Debug.Log("�������A���ݨ� DungeonManager: " + dungeonEnemyManager_ID);
            }
        }
        if (initGampleyRef_ID != null && initGampleyRef_ID != "")
        {
            //Debug.Log("�������A���ݨ� initGampleyRef: " + initGampleyRef_ID);
            initGameplayRef = refMap[initGampleyRef_ID];
        }

    }
}

[System.Serializable]
public class CMazeJsonData  //�@��Ӧa������Ƥ��e (�]�t�C�@�h���g�c)
{
    public string ID;
    public string name;
    public ContinuousMazeJsonData[] battles;
    public string levelID;  //�p�G�O�@�Ӧa�� level ���ɭԤ~�|����

    public void Convert(Dictionary<string, GameObject> refMap)
    {
        //Debug.Log("battles: " + battles);
        for (int i = 0; i < battles.Length; i++)
        {
            //Debug.Log("battle: " + i + " => " + battles[i]);
            battles[i].Convert(refMap);
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
        if (mazeData.battles.Length > 0 && mazeData.battles[0].scene != "")
        {
            //ContinuousBattleManager.StartNewBattle(mazeData.battles);

            //sceneName = mazeData.battles[0].scene;
            base.DoTeleport();
        }
    }

    protected override void DoLoadScene()
    {
        //base.DoLoadScene();
        DoLoadJsonMazeScene(mazeData, backScene, backEntrance);
    }

    public static void DoLoadJsonMazeScene(CMazeJsonData data, string backScene = "", string backEntrance = "")
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
