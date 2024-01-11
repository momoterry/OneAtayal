using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousNextPortal : ScenePortal
{
    public bool gotoNextLevelOnFinish = false;
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
            string currLevelID = BattleSystem.GetInstance().levelID;
            if (currLevelID != "")
            {
                //print("ContinuousNextPortal1!! ���d���� " + BattleSystem.GetInstance().levelID);
                GameSystem.GetInstance().theLevelManager.SetLevelClear(currLevelID);
                if (gotoNextLevelOnFinish)
                {
                    LevelInfo nextLevel = GameSystem.GetInstance().theLevelManager.GetNextLevel(currLevelID);
                    if (nextLevel != null)
                    {
                        print("�������U�@���i�o !! " + nextLevel.ID);
                        GameSystem.GetInstance().theLevelManager.GotoLevel(nextLevel.ID);
                        return;
                    }
                }
            }

            BattleSystem.GetInstance().OnBackPrevScene();
        }

    }
}
