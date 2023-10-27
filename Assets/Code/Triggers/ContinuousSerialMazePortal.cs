using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RandomSpawner;

public class ContinuousSerialMazePortal : ScenePortal
{
    public int maxLevels = 10;
    public string scene;
    public string levelName;

    public float puzzleWidthInit = 4.0f;
    public float puzzleWidthAdd = 1.0f;
    public float puzzleHeightInit = 4.0f;
    public float puzzleHeightAdd = 1.0f;
    public float dungeonDifficultyInit = 1.0f;
    public float dungeonDifficultyAdd = 0.5f;
    public DungeonEnemyManagerBase dungeonEnemyManager;     //如果有指定，下面的 normalEnemy 資訊無用
    public float normalEnemyNumInit = 4.0f;
    public float normalEnemyNumAdd = 2.0f;
    public float normalEnemyRateInit = 0.4f;
    public float normalEnemyRateAdd = 0.0f;

    public Vector2Int bigRoomSize = new Vector2Int(2, 2);
    public float bigRoomNumInit = 1.0f;
    public float bigRoomNumAdd = 0.5f;

    [System.Serializable]
    public class GameplayInfo
    {
        public GameObject gameplayRef;
        public float difficultyAdd;
    }
    [System.Serializable] 
    public class FinalGameplayInfo
    {
        public GameplayInfo[] randomGameplays;
    }
    public FinalGameplayInfo[] FinalGamplays;

    //TODO: Clear
    //public GameObject[] FinalRoomGamplays;

    [System.Serializable]
    public class RewardItem 
    { 
        public GameObject item;
        public float rate;
    }
    public float exploreRewardNumInit = 2.0f;
    public float exploreRewardNumAdd = 1.0f;
    public RewardItem[] ExploreRewardInfo;
    public GameObject initGameplayRefInFirstLevel;

    protected ContinuousMazeData[] mazeLevelDatas;

    protected override void Awake()
    {
        mazeLevelDatas = new ContinuousMazeData[maxLevels];
        for (int i=0; i<maxLevels; i++)
        {
            mazeLevelDatas[i] = new ContinuousMazeData();
            mazeLevelDatas[i].scene = scene;
            mazeLevelDatas[i].name = levelName + " " + (i + 1) + "/" + maxLevels;
            mazeLevelDatas[i].puzzleWidth = (int)(puzzleWidthInit + (puzzleWidthAdd * i));
            mazeLevelDatas[i].puzzleHeight = (int)(puzzleHeightInit + (puzzleHeightAdd * i));
            mazeLevelDatas[i].dungeonDifficulty = dungeonDifficultyInit + (dungeonDifficultyAdd * i);
            mazeLevelDatas[i].dungeonEnemyManager = dungeonEnemyManager;
            mazeLevelDatas[i].normalEnemyNum = (int)(normalEnemyNumInit + (normalEnemyNumAdd * i));
            mazeLevelDatas[i].normalEnemyRate = normalEnemyRateInit + (normalEnemyRateAdd * i);
            int roomNum = (int)(bigRoomNumInit + (bigRoomNumAdd * i));
            if (roomNum > 0)
            {
                mazeLevelDatas[i].bigRooms = new MG_MazeDungeon.BigRoomInfo[roomNum];
                for (int j = 0; j < roomNum; j++)
                {
                    mazeLevelDatas[i].bigRooms[j] = new MG_MazeDungeon.BigRoomInfo();
                    mazeLevelDatas[i].bigRooms[j].numDoor = 1;
                    mazeLevelDatas[i].bigRooms[j].size = bigRoomSize;
                }
                if (i < FinalGamplays.Length)
                {
                    if (FinalGamplays[i].randomGameplays.Length > 0)
                    {
                        int k = Random.Range(0, FinalGamplays[i].randomGameplays.Length);
                        mazeLevelDatas[i].bigRooms[0].gameplayRef = FinalGamplays[i].randomGameplays[k].gameplayRef;
                        mazeLevelDatas[i].bigRooms[0].difficultyAdd = FinalGamplays[i].randomGameplays[k].difficultyAdd;
                        mazeLevelDatas[i].portalAfterFirstRoomGamplay = true;
                    }
                }

                //if (i < FinalRoomGamplays.Length)
                //{
                //    mazeLevelDatas[i].bigRooms[0].gameplayRef = FinalRoomGamplays[i];
                //    mazeLevelDatas[i].portalAfterFirstRoomGamplay = true;
                //}
            }
            //mazeLevelDatas[i].firstRoomDifficultyAdd;
            int rewardNum = (int)(exploreRewardNumInit + (exploreRewardNumAdd * i));
            mazeLevelDatas[i].maxExploreReward = rewardNum;
            if (ExploreRewardInfo.Length > 0)
            {
                mazeLevelDatas[i].exploreRewards = new GameObject[rewardNum];
                for (int j=0; j<rewardNum; j++)
                {
                    mazeLevelDatas[i].exploreRewards[j] = GetOneRandomReward();
                }
            }
            if (initGameplayRefInFirstLevel)
            {
                mazeLevelDatas[0].initGameplayRef = initGameplayRefInFirstLevel;
            }
        }
        base.Awake();
    }

    protected GameObject GetOneRandomReward()
    {
        float rdTotal = 0;
        for (int i = 0; i < ExploreRewardInfo.Length; i++)
        {
            rdTotal += ExploreRewardInfo[i].rate;
        }
        //print("rdTotal " + rdTotal);

        float rdSum = 0;
        float rd = Random.Range(0, rdTotal);
        int result = -1;
        for (int i = 0; i < ExploreRewardInfo.Length; i++)
        {
            rdSum += ExploreRewardInfo[i].rate;
            if (rd < rdSum)
            {
                result = i;
                break;
            }
        }
        if (result >=0)
        {
            return ExploreRewardInfo[result].item;
        }
        return null;
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
