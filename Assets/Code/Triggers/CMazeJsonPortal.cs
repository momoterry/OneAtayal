using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMazeDataSimple : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    //public float dungeonDifficulty; //�j�� 0 �~���@�ΡA��ǭȬ� 1.0f
    //public DungeonEnemyManagerBase dungeonEnemyManager;     //�p�G�����w�A�U���� normalEnemy ��T�L��
    public int normalEnemyNum;
    public float normalEnemyRate;
    //public int bigRoomNum;
    //public int maxExploreReward;
    //public MG_MazeDungeon.BigRoomInfo[] bigRooms;           //�p�G�ϥΡA�h bigRoomNum �L��
    //public GameObject[] exploreRewards;
    //public GameObject initGameplayRef;
    public bool portalAfterFirstRoomGamplay;
}

[System.Serializable]
public class CMazeJsonData
{
    public ContinuousMazeData[] levels;
}

public class CMazeJsonPortal : ScenePortal
{
    public TextAsset jsonFile;
    // Start is called before the first frame update

    protected CMazeJsonData mazeData;

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

        print("CMazeJsonPortal: " + jsonFile.text);

        mazeData = JsonUtility.FromJson<CMazeJsonData>(jsonFile.text);
        print("puzzleWidth" + mazeData.levels[0].puzzleWidth);



        //SaveData testSave = JsonUtility.FromJson <SaveData>(jsonFile.text);
        //print("testSave: " + testSave);
        //print("Money: " + testSave.Money);
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
