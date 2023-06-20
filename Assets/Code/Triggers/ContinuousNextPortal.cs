using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousNextPortal : ScenePortal
{
    protected override void DoTeleport()
    {
        ContinuousBattleManager.GotoNextBattle();
        ContinuousBattleDataBase nextBattle = ContinuousBattleManager.GetCurrBattleData();
        if (nextBattle != null)
        {
            sceneName = nextBattle.scene;
            //print("ContinuousNextPortal1!! 有資料，往下一關出發!!");
        }
        else
        {
            sceneName = "";
            //print("ContinuousNextPortal1!! 沒有資料，回到營地 !!");
        }
        base.DoTeleport();
    }
}
