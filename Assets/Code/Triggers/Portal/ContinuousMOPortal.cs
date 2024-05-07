using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMOData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public float pathRate;
    public MG_MazeOne.MAZE_DIR mazeDir;
    public bool finishAtDeepest;

    public MazeGameManagerBase gameManagerRef;
    public float gameDiffcultRateMin = 1.0f;
    public float gameDiffcultRateMax = 2.0f;
    public int gameEnemyLV = 1;

    //public DungeonEnemyManagerBase dungeonEnemyManager;     //�p�G�����w�A�U���� normalEnemy ��T�L��
    //public int normalEnemyNum;
    //public float normalEnemyRate;
    //public int bigRoomNum;
    //public int maxExploreReward;
    //public MG_MazeDungeon.BigRoomInfo[] bigRooms;           //�p�G�ϥΡA�h bigRoomNum �L��
    //public GameObject[] exploreRewards;
    public GameObject initGameplayRef;
    //public bool portalAfterFirstRoomGamplay;
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
