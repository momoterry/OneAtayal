using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            mazeLevelDatas[i].maxExploreReward = 2+i;
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
            }
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
