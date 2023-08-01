using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousSerialMazePortal : ScenePortal
{
    public int maxLevels = 10;
    public string scene;
    public string levelName;

    public int puzzleWidthInit = 4;
    public float puzzleWidthAdd = 1.0f;
    public int puzzleHeightInit = 4;
    public float puzzleHeightAdd = 1.0f;
    public int normalEnemyNumInit = 4;
    public float normalEnemyNumAdd = 2.0f;
    public float normalEnemyRateInit = 0.4f;
    public float normalEnemyRateAdd = 0.0f;

    protected ContinuousMazeData[] mazeLevelDatas;

    protected void Awake()
    {
        mazeLevelDatas = new ContinuousMazeData[maxLevels];
        for (int i=0; i<maxLevels; i++)
        {
            mazeLevelDatas[i] = new ContinuousMazeData();
            mazeLevelDatas[i].scene = scene;
            mazeLevelDatas[i].name = levelName + " " + (i + 1) + "/" + maxLevels;
            mazeLevelDatas[i].puzzleWidth = puzzleWidthInit + (int)(puzzleWidthAdd * i);
            mazeLevelDatas[i].puzzleHeight = puzzleHeightInit + (int)(puzzleHeightAdd * i);
            mazeLevelDatas[i].normalEnemyNum = normalEnemyNumInit + (int)(normalEnemyNumAdd * i);
            mazeLevelDatas[i].normalEnemyRate = normalEnemyRateInit + (normalEnemyRateAdd * i);
            mazeLevelDatas[i].maxExploreReward = 2+i;
        }
    }
    protected override void DoTeleport()
    {
        if (mazeLevelDatas.Length > 0 && mazeLevelDatas[0].scene != "")
        {
            ContinuousBattleManager.StartNewBattle(mazeLevelDatas);

            sceneName = mazeLevelDatas[0].scene;
            base.DoTeleport();
        }
    }

}
