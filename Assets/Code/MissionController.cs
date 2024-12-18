using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStat;

//在關卡中的任務狀態控制，決定任務完成與否的計算等
//為了支援連續戰鬥的地城，所以也需要 BattleSave

public class MissionController : MonoBehaviour
{
    public class MissionControllerSave : BattleSaveBase
    {
    }
    protected MissionControllerSave saveData = new MissionControllerSave();

    static private MissionController instance;
    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份  MissionController 存在 ");
        instance = this;
    }

    static public MissionController GetInstance()
    {
        return instance;
    }

    void Start()
    {
        saveData = (MissionControllerSave)BattlePlayerData.GetInstance().SysncSaveData("MissionControllerSave", saveData);

        MissionData currMissionData = MissionManager.GetCurrMission();
        if (currMissionData != null)
        {
            print("========== 開始關卡中的任務: " + currMissionData.Title);
        }
        else
        {
            print("========== 無任務開局 ........");
        }
    }

    public void RegisterObjective(MissionObjective objective)
    {
        print("========== 加入一個任務: " + objective.objectiveText);
    }

    public void CompleteObjective(MissionObjective objective)
    {
        print("========== 完成一個任務: " + objective.objectiveText);
    }

}
