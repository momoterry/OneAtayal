using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�޲z�ݭn�s��q�L�h�����d�ӧ������԰��A��p�s��a��
//�ݭn�O�����n�����d�ѼơA�� MapGenerate ��z�L�Ѽ��ܤƨӲ��ͤ��P�a��A�@�� Scene �N�i�H���ͤ��P�����d

public class ContinuousBattleDataBase
{
    public string scene;
    public string name;
}

public class ContinuousBattleManager : MonoBehaviour
{
    static ContinuousBattleManager instance;

    //protected List<ContinuousBattleDataBase> battleList = new List<ContinuousBattleDataBase>();
    protected ContinuousBattleDataBase[] currBattleArray = null;
    protected int currBattleIndex;
    protected BattlePlayerCrossSceneData battlePlayerData = null;

    protected bool isGoingContinuousBattle = false; //�b�}�ҩζi�J ContinuousBattle �ɥ��}�� Flag�A���԰����}�ɪ��D�O�_�ݭn�d�U�O��

    static public ContinuousBattleManager GetInstance() { return instance; }
    static public BattlePlayerCrossSceneData GetBattlePlayerCrossSceneData()
    {
        return instance.battlePlayerData;
    }

    protected void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� ContinuousBattleManager �s�b: ");
        instance = this;
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
        collectedDolls.Clear();
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

    //�����b���ߨ쪺 Doll ���O��  TODO: ���Ӿ�X�� BattlePlayerData ��
    protected List<string> collectedDolls = new List<string>();
    public static void AddCollectedDoll(string dollID) { instance._AddCollectedDoll(dollID); }
    protected void _AddCollectedDoll(string dollID) 
    {
        collectedDolls.Add(dollID);
    }

    public static string[] GetAllCollectedDolls() { return instance._GetAllCollectedDolls(); }
    protected string[] _GetAllCollectedDolls() 
    { 
        if (collectedDolls.Count > 0)
        {
            string[] temp = new string[collectedDolls.Count];
            for (int i=0; i<collectedDolls.Count; i++)
            {
                temp[i] = collectedDolls[i];
            }
            return temp;
        }
        return null;
    }

    public static void ResetCollectedDolls() { instance._ResetCollectedDolls(); }
    protected void _ResetCollectedDolls() { }

}
