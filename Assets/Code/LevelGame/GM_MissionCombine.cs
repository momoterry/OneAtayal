using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��զX���Ȫ� GM

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
                print(" ==== �B�z���ȥؼЩж�");
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
