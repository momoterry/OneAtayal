using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理需要連續通過多個關卡來完成的戰鬥，比如連續地城
//需要記錄必要的關卡參數，讓 MapGenerate 能透過參數變化來產生不同地表，一個 Scene 就可以產生不同的關卡

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
                print("連續戰鬥結束 ................ !!");
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
