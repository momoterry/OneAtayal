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
    //        //print("ContinuousNextPortal1!! 有資料，往下一關出發!! 並且設定回程:" + backScene + " - " + backEntrance);
    //    }
    //    else
    //    {
    //        sceneName = "";
    //        //print("ContinuousNextPortal1!! 沒有資料，回到營地 !!");
    //        print("ContinuousNextPortal1!! 沒有資料, 連續戰鬥結束!!");
    //        if (BattleSystem.GetInstance().levelID != "")
    //        {
    //            print("ContinuousNextPortal1!! 關卡完成 " + BattleSystem.GetInstance().levelID);
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
            //print("ContinuousNextPortal1!! 有資料，往下一關出發!! 並且設定回程:" + backScene + " - " + backEntrance);
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
            //print("ContinuousNextPortal1!! 沒有資料, 連續戰鬥結束, 準備回城!!");
            string currLevelID = BattleSystem.GetInstance().levelID;
            if (currLevelID != "")
            {
                //print("ContinuousNextPortal1!! 關卡完成 " + BattleSystem.GetInstance().levelID);
                GameSystem.GetInstance().theLevelManager.SetLevelClear(currLevelID);
                if (gotoNextLevelOnFinish)
                {
                    LevelInfo nextLevel = GameSystem.GetInstance().theLevelManager.GetNextLevel(currLevelID);
                    if (nextLevel != null)
                    {
                        print("直接往下一關進發 !! " + nextLevel.ID);
                        GameSystem.GetInstance().theLevelManager.GotoLevel(nextLevel.ID);
                        return;
                    }
                }
            }

            BattleSystem.GetInstance().OnBackPrevScene();
        }

    }
}
