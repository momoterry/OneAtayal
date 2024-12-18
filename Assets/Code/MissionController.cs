using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStat;

//�b���d�������Ȫ��A����A�M�w���ȧ����P�_���p�ⵥ
//���F�䴩�s��԰����a���A�ҥH�]�ݭn BattleSave

public class MissionController : MonoBehaviour
{
    public class MissionControllerSave : BattleSaveBase
    {
        public List<MissionObjective> todoList = new();
        public List<MissionObjective> doneList = new();
    }
    protected MissionControllerSave saveData = new MissionControllerSave();

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
        print("========== �����@�ӥ���: " + objective.objectiveText);
        saveData.todoList.Remove(objective);
        saveData.doneList.Add(objective);
        print("==========  �������ȥؼ�: " + saveData.doneList.Count + "  �`�@: " + (saveData.todoList.Count + saveData.doneList.Count));
        if (saveData.todoList.Count == 0)
        {
            print("========== ���������� !!!!!!!!!!!!");
        }
    }

}
