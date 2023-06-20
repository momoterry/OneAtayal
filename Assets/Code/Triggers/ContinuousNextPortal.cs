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
            //print("ContinuousNextPortal1!! ����ơA���U�@���X�o!!");
        }
        else
        {
            sceneName = "";
            //print("ContinuousNextPortal1!! �S����ơA�^����a !!");
        }
        base.DoTeleport();
    }
}
