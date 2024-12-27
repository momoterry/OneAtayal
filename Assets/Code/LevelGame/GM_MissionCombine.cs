using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��զX���Ȫ� GM

public class GM_MissionCombine : MazeGameManager
{
    public override void BuildLayout()
    {
        AddMissionObjectiveGames(); //�b�]�w�C�� Room �� Gameplay ���e�[�J���� Game
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
                print(" ==== �B�z���ȥؼЩж�");
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
        }
    }

    //public override void SetupData(GameManagerDataBase data)
    //{
    //    base.SetupData(data);


    //}

}
