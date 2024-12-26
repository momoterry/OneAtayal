using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//能組合任務的 GM

public class GM_MissionCombine : MazeGameManager
{
    public override void BuildAll()
    {
        MissionData mission = MissionManager.GetCurrMission();
        if (mission != null) 
        {
            if (mission.objectiveGame != null) 
            {
                GameObject o = Instantiate( mission.objectiveGame.objectiveGameRef.gameObject);
                RoomGameplayBase roomGame = o.GetComponent<RoomGameplayBase>();
                //o.transform.parent = transform;
                print(" ==== 處理任務目標房間");
                switch (mission.objectiveGame.objectiveType)
                {
                    case MissionData.OBJECTIVE_TYPE.BRANCH_END:
                        fixBranchEndGames = new FixBranchEndGameInfo[2];
                        for (int i = 0; i < fixBranchEndGames.Length; i++) 
                        {
                            fixBranchEndGames[i] = new();
                            fixBranchEndGames[i].game = roomGame;
                        }
                        break;
                }
            }
        }

        base.BuildAll();
    }
}
