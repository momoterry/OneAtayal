using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�޲z�ݭn�s��q�L�h�����d�ӧ������԰��A��p�s��a��
//�ݭn�O�����n�����d�ѼơA�� MapGenerate ��z�L�Ѽ��ܤƨӲ��ͤ��P�a��A�@�� Scene �N�i�H���ͤ��P�����d

public class ContinuousBattleDataBase
{
    public string scene;
}

public class ContinuousBattleManager : MonoBehaviour
{
    static ContinuousBattleManager instance;

    //protected List<ContinuousBattleDataBase> battleList = new List<ContinuousBattleDataBase>();
    protected ContinuousBattleDataBase[] currBattleArray = null;
    protected int currBattleIndex;

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

    }

    protected void DoEndAllBattle()
    {
        currBattleArray = null;
        currBattleIndex = 0;
    }

    static public void GotoNextBattle()
    {
        instance._GotoNextBattle();
    }
    protected void _GotoNextBattle()
    {
        print("_GotoNextBattle!!");
        print("currBattleArray: " + currBattleArray.Length);

        if (currBattleArray != null)
        {
            currBattleIndex++;
            if (currBattleIndex == (currBattleArray.Length))
            {
                print("�s��԰����� ................ !!");
                DoEndAllBattle();
            }
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

}
