using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理需要連續通過多個關卡來完成的戰鬥，比如連續地城
//需要記錄必要的關卡參數，讓 MapGenerate 能透過參數變化來產生不同地表，一個 Scene 就可以產生不同的關卡

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

    protected void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 ContinuousBattleManager 存在: ");
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
        collectedDolls.Clear();
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
                //print("連續戰鬥結束 ................ !!");
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

    //有關半路撿到的 Doll 的記錄
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
