using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�b���d�������Ȫ��A����A�M�w���ȧ����P�_���p�ⵥ
//���F�䴩�s��԰����a���A�ҥH�]�ݭn BattleSave

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
            print("ERROR !! �W�L�@��  MissionController �s�b ");
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
            print("========== �}�l���d��������: " + currMissionData.Title);
        }
        else
        {
            print("========== �L���ȶ}�� ........");
        }
    }

    public void RegisterObjective(MissionObjective objective)
    {
        print("========== �[�J�@�ӥ���: " + objective.objectiveText);
        saveData.todoList.Add(objective);
    }

    public void CompleteObjective(MissionObjective objective)
    {
        //print("========== �����@�ӥ���: " + objective.objectiveText);
        saveData.todoList.Remove(objective);
        saveData.doneList.Add(objective);
        //print("==========  �������ȥؼ�: " + saveData.doneList.Count + "  �`�@: " + (saveData.todoList.Count + saveData.doneList.Count));
        string missionTitle = currMission == null ? "----" : currMission.Title;
        BattleSystem.GetHUD().missionControlUI.ShowObjectiveDoneMessage(missionTitle, objective.objectiveText, saveData.doneList.Count, saveData.todoList.Count + saveData.doneList.Count);
        if (saveData.todoList.Count == 0)
        {
            ////print("========== ���������� !!!!!!!!!!!!" + objective.completePortalPos);
            //BattleSystem.GetHUD().missionControlUI.ShowMissionCompleteWindow(currMission);
            //if (objective.completePortalPos)
            //{
            //    //print("========== �ͦ� Portal !!!!!!!!!!!!" + missionCompltePortalRef);
            //    BattleSystem.SpawnGameObj(missionCompltePortalRef, objective.completePortalPos.position);
            //}
            OnCompleteMission(objective);
        }
    }

    protected void OnCompleteMission(MissionObjective lastObjective)
    {
        //�o����y������
        MissionManager.MissionRewardResult rewardResult = MissionManager.CompleteCurrMission();

        BattleSystem.GetHUD().missionControlUI.ShowMissionCompleteWindow(currMission, rewardResult);
        if (lastObjective.completePortalPos)
        {
            BattleSystem.SpawnGameObj(missionCompltePortalRef, lastObjective.completePortalPos.position);
        }
    }


}
