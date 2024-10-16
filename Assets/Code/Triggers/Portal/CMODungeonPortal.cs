using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//類似 ContinuousMOData，但直接指定 CSV/Json 檔中的 Dungeon ID

public class CMODungeonPortal : ScenePortal
{
    public string DungeonID;

    protected override void DoTeleport()
    {
        MODungeonData dungeonData = GameSystem.GetInstance().theDungeonData.GetMODungeonData(DungeonID);
        //print("CMODungeonPortal => " + dungeonData);
        if (dungeonData == null)
        {
            return;
        }

        ContinuousMOData[] mazeLevelDatas = dungeonData.ToContinuousMODataArray();

        if (mazeLevelDatas.Length > 0 && mazeLevelDatas[0].scene != "")
        {
            ContinuousBattleManager.StartNewBattle(mazeLevelDatas);

            sceneName = mazeLevelDatas[0].scene;
            base.DoTeleport();
        }
    }
}
