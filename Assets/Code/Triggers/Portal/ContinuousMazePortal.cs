using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMazeData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public float dungeonDifficulty; //�j�� 0 �~���@�ΡA��ǭȬ� 1.0f
    public DungeonEnemyManagerBase dungeonEnemyManager;     //�p�G�����w�A�U���� normalEnemy ��T�L��
    public int normalEnemyNum;
    public float normalEnemyRate;
    public int bigRoomNum;
    public int maxExploreReward;
    public MG_MazeDungeon.BigRoomInfo[] bigRooms;           //�p�G�ϥΡA�h bigRoomNum �L��
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
