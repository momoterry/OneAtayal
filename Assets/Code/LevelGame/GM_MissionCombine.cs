using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//能組合任務的 GM

public class GM_MissionCombine : MazeGameManager
{
    public override void BuildLayout()
    {
        AddMissionObjectiveGames(); //在設定每個 Room 的 Gameplay 之前加入任務 Game
        base.BuildLayout();
    }

    protected void AddMissionObjectiveGames()
    {
        MissionData mission = MissionManager.GetCurrMission();
        if (mission != null)
        {
            if (mission.objectiveGame != null)
            {
                GameObject o = Instantiate(mission.objectiveGame.objectiveGameRef.gameObject);
                RoomGameplayBase roomGame = o.GetComponent<RoomGameplayBase>();
                o.transform.parent = transform;
                print(" ==== 處理任務目標房間");
                switch (mission.objectiveGame.objectiveType)
                {
                    case MissionData.OBJECTIVE_TYPE.BRANCH_END:
                        int bNum = branchEndRoomList.Count + branchEndPathList.Count;
                        print("bRoom: " + branchEndRoomList.Count + " bPath" + branchEndPathList.Count);
                        fixBranchEndGames = new FixBranchEndGameInfo[bNum];
                        for (int i = 0; i < bNum; i++)
                        {
                            fixBranchEndGames[i] = new();
                            fixBranchEndGames[i].game = roomGame;
                        }
                        break;
                }
            }

            //加入野巫靈的部份
            if (mission.helpDoll != null)
            {
                if (theOPM && mission.helpDoll.dollRef)
                {
                    theOPM.randomObjects[0].objRef = mission.helpDoll.dollRef;
                    theOPM.forceRandomNum *= (mission.helpDoll.ratioAdd + 1.0f);
                    print("修正了野巫靈獎勵 " + theOPM.forceRandomNum);

                }
            }
        }
    }

    //public override void SetupData(GameManagerDataBase data)
    //{
    //    base.SetupData(data);


    //}

}
