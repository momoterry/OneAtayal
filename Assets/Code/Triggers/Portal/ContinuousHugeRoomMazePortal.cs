using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MG_HugeRoomMaze;

[System.Serializable]
public class ContinuousHugeRoomMazeData : ContinuousBattleDataBase
{
    public int pathWidth = 5;
    public int pathHeight = 5;
    public int roomWidth = -1;                  //如果 roomWidth/roomHeight 沒有大於 pathWidth/pathHeight 的話，不作用
    public int roomHeight = -1;
    public MazeGameManagerBase gameManagerRef;
    public GameManagerDataBase gameManagerData;
    public BlockInfo[] blocks;
}

public class ContinuousHugeRoomMazePortal : ScenePortal
{
    public ContinuousHugeRoomMazeData[] mazeLevelDatas;

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
