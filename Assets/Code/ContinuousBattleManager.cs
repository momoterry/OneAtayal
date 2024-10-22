using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�޲z�ݭn�s��q�L�h�����d�ӧ������԰��A��p�s��a��
//�ݭn�O�����n�����d�ѼơA�� MapGenerate ��z�L�Ѽ��ܤƨӲ��ͤ��P�a��A�@�� Scene �N�i�H���ͤ��P�����d
//�@�� ContinuousBattleDataBase �N��a�������@�h

public class ContinuousBattleDataBase
{
    public string scene;
    public string name;
    public string levelID;  //�p�G�԰��O�ݩ� LevelManager �����@�� level �ɤ~�|����
    public float cameraAdjust;  // > 0 ���ܩԻ�
}

public class ContinuousBattleManager : GlobalSystemBase
{
    static ContinuousBattleManager instance;

    protected ContinuousBattleDataBase[] currBattleArray = null;
    protected int currBattleIndex;
    //�s��԰����A�ݭn�� Scene �~��������԰���ơA�p�԰��ɯŵ��A�u�b�� Scene �ɱq BattlePlayerData ���o
    protected BattlePlayerCrossSceneData battlePlayerData = null;

    protected bool isGoingContinuousBattle = false; //�b�}�ҩζi�J ContinuousBattle �ɥ��}�� Flag�A���԰����}�ɪ��D�O�_�ݭn�d�U�O��

    static public ContinuousBattleManager GetInstance() { return instance; }
    static public BattlePlayerCrossSceneData GetBattlePlayerCrossSceneData()
    {
        return instance.battlePlayerData;
    }

    //protected void Awake()
    //{
    //    if (instance != null)
    //        print("ERROR !! �W�L�@�� ContinuousBattleManager �s�b: ");
    //    instance = this;
    //}

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� ContinuousBattleManager �s�b: ");
        instance = this;
        base.InitSystem();
    }

    static public void StartNewBattle(ContinuousBattleDataBase[] battleArray)
    {
        instance._StartNewBattle(battleArray);
    }

    protected void _StartNewBattle(ContinuousBattleDataBase[] battleArray)
    {
        //battleList.Clear();
        currBattleArray = battleArray;
        currBattleIndex = 0;

        isGoingContinuousBattle = true;
    }

    protected void DoEndAllBattle()
    {
        currBattleArray = null;
        currBattleIndex = 0;
        //collectedDolls.Clear();
        formationDollList.Clear();
        battlePlayerData = null;
    }

    static public void GotoNextBattle()
    {
        instance._GotoNextBattle();
    }
    protected void _GotoNextBattle()
    {
        //print("_GotoNextBattle!!");
        //print("currBattleArray: " + currBattleArray.Length);

        if (currBattleArray != null)
        {
            currBattleIndex++;
            if (currBattleIndex == (currBattleArray.Length))
            {
                //print("�s��԰����� ................ !!");
                //DoEndAllBattle();
                isGoingContinuousBattle = false;
            }
            else
                isGoingContinuousBattle = true;
        }
    }

    static public ContinuousBattleDataBase GetNextBattleData()      //�Ψ��ˬd�ثe�i�椤���԰��O�_�w�g�O�̲׾�
    {
        return instance._GetNextBattleData();
    }
    protected ContinuousBattleDataBase _GetNextBattleData()
    {
        if (currBattleArray != null && (currBattleIndex+1) < currBattleArray.Length)
        {
            return currBattleArray[currBattleIndex+1];
        }
        return null;
    }


    static public ContinuousBattleDataBase GetCurrBattleData()
    {
        return instance._GetCurrBattleData();
    }

    protected ContinuousBattleDataBase _GetCurrBattleData()
    {

        if (currBattleArray != null && currBattleIndex < currBattleArray.Length)
        {
            return currBattleArray[currBattleIndex];
        }
        return null;
    }

    //���} Scene �ɩI�s�A�p�G�S���ƥ��I�s GoToNext �����p�A���������԰��Q����A�M���O��
    public static void OnSceneExit() { instance._OnSceneExit(); }
    protected void _OnSceneExit()
    {
        //print("_OnSceneExit !!!!  isGoingContinuousBattle = " + isGoingContinuousBattle);
        if (isGoingContinuousBattle)
        {
            battlePlayerData = BattlePlayerData.GetInstance().GetCrossSceneData();
        }
        else
        {
            DoEndAllBattle();
        }

        isGoingContinuousBattle = false;
    }

    //========== �s�P��m�@�_�O�����s���� ===============  TODO: ���Ӿ�X�� BattlePlayerData ��
    protected List<FormationDollInfo> formationDollList = new List<FormationDollInfo>();

    public static int GetCollectedDollNum() {
        if (instance == null)
            return 0;
        return instance._GetCollectedDollNum();
    }

    public int _GetCollectedDollNum() { return formationDollList.Count; }

    public static void AddCollectedDoll(string dollID, int group, int index) { instance._AddCollectedDoll(dollID, group, index); }
    protected void _AddCollectedDoll(string dollID, int group, int index)
    {
        //print("�����԰����F: " + dollID + " Group: " + group + " Index: " + index);

        FormationDollInfo info;
        info.dollID = dollID;
        info.group = group;
        info.index = index;
        formationDollList.Add(info);
    }
    public static FormationDollInfo[] GetAllBattleSavedDolls() { return instance._GetAllBattleSavedDolls(); }
    protected FormationDollInfo[] _GetAllBattleSavedDolls()
    {
        if (formationDollList.Count > 0)
        {
            FormationDollInfo[] temp = new FormationDollInfo[formationDollList.Count];
            for (int i = 0; i < formationDollList.Count; i++)
            {
                temp[i] = formationDollList[i];
            }
            return temp;
        }
        return null;
    }

    public static void ResetBattleSavedDolls() { instance._ResetBattleSavedDolls(); }
    protected void _ResetBattleSavedDolls() { formationDollList.Clear(); }
    //========================================================

}
