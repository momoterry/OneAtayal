using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在關卡中的任務狀態控制，決定任務完成與否的計算等
//為了支援連續戰鬥的地城，所以也需要 BattleSave

public class MissionController : MonoBehaviour
{
    public GameObject missionCompltePortalRef;

    public class MissionControllerSave : BattleSaveBase
    {
        public List<MissionObjective> todoList = new();
        public List<MissionObjective> doneList = new();
    }
    protected MissionControllerSave saveData = new MissionControllerSave();
    protected MissionData currMission;

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
        currMission = MissionManager.GetCurrMission();
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
        saveData.todoList.Add(objective);
    }

    public void CompleteObjective(MissionObjective objective)
    {
        //print("========== 完成一個任務: " + objective.objectiveText);
        saveData.todoList.Remove(objective);
        saveData.doneList.Add(objective);
        //print("==========  完成任務目標: " + saveData.doneList.Count + "  總共: " + (saveData.todoList.Count + saveData.doneList.Count));
        string missionTitle = currMission == null ? "----" : currMission.Title;
        BattleSystem.GetHUD().missionControlUI.ShowObjectiveDoneMessage(missionTitle, objective.objectiveText, saveData.doneList.Count, saveData.todoList.Count + saveData.doneList.Count);
        if (saveData.todoList.Count == 0)
        {
            //print("========== 全部完成啦 !!!!!!!!!!!!" + objective.completePortalPos);
            BattleSystem.GetHUD().missionControlUI.ShowMissionCompleteWindow(currMission);
            if (objective.completePortalPos)
            {
                //print("========== 生成 Portal !!!!!!!!!!!!" + missionCompltePortalRef);
                BattleSystem.SpawnGameObj(missionCompltePortalRef, objective.completePortalPos.position);
            }
        }
    }

}
