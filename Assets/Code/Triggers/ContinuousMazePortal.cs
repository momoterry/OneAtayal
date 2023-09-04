using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMazeData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public float dungeonDifficulty; //大於 0 才有作用，基準值為 1.0f
    public DungeonEnemyManagerBase dungeonEnemyManager;     //如果有指定，下面的 normalEnemy 資訊無用
    public int normalEnemyNum;
    public float normalEnemyRate;
    public int bigRoomNum;
    public int maxExploreReward;
    public MG_MazeDungeon.BigRoomInfo[] bigRooms;           //如果使用，則 bigRoomNum 無視
    public GameObject[] exploreRewards;
    public GameObject initGameplayRef;
    public bool portalAfterFirstRoomGamplay;
}

public class ContinuousMazePortal : ScenePortal
{
    public ContinuousMazeData[] mazeLevelDatas;

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
