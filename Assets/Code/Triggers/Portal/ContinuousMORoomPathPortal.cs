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

    public MazeGameManagerBase gameManagerRef;      //指定 Gameplay 細節的 Ref 檔
    public GameManagerDataBase gameManagerData;     //難度和獎勵等基本參數設定，參數會蓋掉 MazeGameManagerBase 內的值
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
