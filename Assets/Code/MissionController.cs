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
    }

    public void CompleteObjective(MissionObjective objective)
    {
        print("========== �����@�ӥ���: " + objective.objectiveText);
    }

}
