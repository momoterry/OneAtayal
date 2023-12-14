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
            backScene = BattleSystem.GetInstance().backScene;
            backEntrance = BattleSystem.GetInstance().backEntrance;
            //print("ContinuousNextPortal1!! ����ơA���U�@���X�o!! �åB�]�w�^�{:" + backScene + " - " + backEntrance);
        }
        else
        {
            sceneName = "";
            //print("ContinuousNextPortal1!! �S����ơA�^����a !!");
        }
        base.DoTeleport();
    }
}
