using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMORoomPathData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public int roomWidth = 15;
    public int roomHeight = 20;
    public int pathWidth = 5;
    public int pathHeight = 5;
    public MG_MazeOneBase.MAZE_DIR mazeDir;
    public int MaxMainDeep = 6;
    public int MaxBranchDeep = 2;

    public GameManagerDataBase gameManagerData;     //螟㎝贱纘单膀セ把计砞﹚把计穦籠奔 MazeGameManagerBase ず
}

public class ContinuousMORoomPathPortal : ScenePortal
{
    public ContinuousMORoomPathData[] mazeLevelDatas;

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
