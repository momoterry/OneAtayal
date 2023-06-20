using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMazeData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public int normalEnemyNum;
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
