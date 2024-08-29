using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMOData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public float pathRate;
    public int roomWidth = 15;
    public int roomHeight = 20;
    public int pathWidth = 5;
    public int pathHeight = 5;
    public MG_MazeOne.MAZE_DIR mazeDir;
    public bool finishAtDeepest;

    public MazeGameManagerBase gameManagerRef;
    public GameManagerDataBase gameManagerData;
    public GameObject initGameplayRef;
}

//放到 ContinuousMOData 中的 Gameplay 參數定義基底，可以被用來擴充以支援不同的 Gameplay 類型
[System.Serializable]
public class GameManagerDataBase
{
    public float difficultRateMin = 1.0f;   //最小難度率，用來調整敵人數量
    public float difficultRateMax = 2.0f;   //最大難度率，用來調整敵人數量
    public int enmeyLV = 1;
    public string specialReward;
    public float specialRewardNum;
}

public class ContinuousMOPortal : ScenePortal
{
    public ContinuousMOData[] mazeLevelDatas;

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
