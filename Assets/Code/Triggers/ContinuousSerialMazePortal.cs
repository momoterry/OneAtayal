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
    public float normalEnemyNumInit = 4.0f;
    public float normalEnemyNumAdd = 2.0f;
    public float normalEnemyRateInit = 0.4f;
    public float normalEnemyRateAdd = 0.0f;

    public Vector2Int bigRoomSize = new Vector2Int(2, 2);
    public float bigRoomNumInit = 1.0f;
    public float bigRoomNumAdd = 0.5f;
    public GameObject[] FinalRoomGamplays;

    [System.Serializable]
    public class RewardItem 
    { 
        public GameObject item;
        public float rate;
    }
    public float exploreRewardNumInit = 2.0f;
    public float exploreRewardNumAdd = 1.0f;
    public RewardItem[] ExploreRewardInfo;

    protected ContinuousMazeData[] mazeLevelDatas;

    protected void Awake()
    {
        mazeLevelDatas = new ContinuousMazeData[maxLevels];
        for (int i=0; i<maxLevels; i++)
        {
            mazeLevelDatas[i] = new ContinuousMazeData();
            mazeLevelDatas[i].scene = scene;
            mazeLevelDatas[i].name = levelName + " " + (i + 1) + "/" + maxLevels;
            mazeLevelDatas[i].puzzleWidth = (int)(puzzleWidthInit + (puzzleWidthAdd * i));
            mazeLevelDatas[i].puzzleHeight = (int)(puzzleHeightInit + (puzzleHeightAdd * i));
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
                if (i < FinalRoomGamplays.Length)
                {
                    mazeLevelDatas[i].bigRooms[roomNum - 1].gameplayRef = FinalRoomGamplays[i];
                }
            }
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
        }
    }

    protected GameObject GetOneRandomReward()
    {
        float rdSum = 0;
        float rd = Random.Range(0, 1.0f);
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
