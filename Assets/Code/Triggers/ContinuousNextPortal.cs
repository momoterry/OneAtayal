using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousNextPortal : ScenePortal
{
    //protected override void DoTeleport()
    //{
    //    ContinuousBattleManager.GotoNextBattle();
    //    ContinuousBattleDataBase nextBattle = ContinuousBattleManager.GetCurrBattleData();
    //    if (nextBattle != null)
    //    {
    //        sceneName = nextBattle.scene;
    //        backScene = BattleSystem.GetInstance().backScene;
    //        backEntrance = BattleSystem.GetInstance().backEntrance;
    //        //print("ContinuousNextPortal1!! ����ơA���U�@���X�o!! �åB�]�w�^�{:" + backScene + " - " + backEntrance);
    //    }
    //    else
    //    {
    //        sceneName = "";
    //        //print("ContinuousNextPortal1!! �S����ơA�^����a !!");
    //        print("ContinuousNextPortal1!! �S�����, �s��԰�����!!");
    //        if (BattleSystem.GetInstance().levelID != "")
    //        {
    //            print("ContinuousNextPortal1!! ���d���� " + BattleSystem.GetInstance().levelID);
    //            GameSystem.GetInstance().theLevelManager.SetLevelClear(BattleSystem.GetInstance().levelID);
    //        }
    //    }
    //    base.DoTeleport();
    //}

    override protected void DoLoadScene()
    {
        ContinuousBattleManager.GotoNextBattle();
        ContinuousBattleDataBase nextBattle = ContinuousBattleManager.GetCurrBattleData();
        if (nextBattle != null)
        {
            sceneName = nextBattle.scene;
            backScene = BattleSystem.GetInstance().backScene;
            backEntrance = BattleSystem.GetInstance().backEntrance;
            //print("ContinuousNextPortal1!! ����ơA���U�@���X�o!! �åB�]�w�^�{:" + backScene + " - " + backEntrance);
            if (backScene != "")
            {
                BattleSystem.GetInstance().OnGotoSceneWithBack(sceneName, entraceName, backScene, backEntrance);
            }
            else
            {
                BattleSystem.GetInstance().OnGotoScene(sceneName, entraceName);
            }
        }
        else
        {
            //sceneName = "";
            //print("ContinuousNextPortal1!! �S�����, �s��԰�����, �ǳƦ^��!!");
            if (BattleSystem.GetInstance().levelID != "")
            {
                //print("ContinuousNextPortal1!! ���d���� " + BattleSystem.GetInstance().levelID);
                GameSystem.GetInstance().theLevelManager.SetLevelClear(BattleSystem.GetInstance().levelID);
            }

            BattleSystem.GetInstance().OnBackPrevScene();
        }

    }
}
