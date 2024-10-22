using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理需要連續通過多個關卡來完成的戰鬥，比如連續地城
//需要記錄必要的關卡參數，讓 MapGenerate 能透過參數變化來產生不同地表，一個 Scene 就可以產生不同的關卡
//一個 ContinuousBattleDataBase 代表地城中的一層

public class ContinuousBattleDataBase
{
    public string scene;
    public string name;
    public string levelID;  //如果戰鬥是屬於 LevelManager 中的一個 level 時才會有值
    public float cameraAdjust;  // > 0 的話拉遠
}

public class ContinuousBattleManager : GlobalSystemBase
{
    static ContinuousBattleManager instance;

    protected ContinuousBattleDataBase[] currBattleArray = null;
    protected int currBattleIndex;
    //連續戰鬥中，需要跨 Scene 繼續持有的戰鬥資料，如戰鬥升級等，只在換 Scene 時從 BattlePlayerData 取得
    protected BattlePlayerCrossSceneData battlePlayerData = null;

    protected bool isGoingContinuousBattle = false; //在開啟或進入 ContinuousBattle 時打開的 Flag，讓戰鬥離開時知道是否需要留下記錄

    static public ContinuousBattleManager GetInstance() { return instance; }
    static public BattlePlayerCrossSceneData GetBattlePlayerCrossSceneData()
    {
        return instance.battlePlayerData;
    }

    //protected void Awake()
    //{
    //    if (instance != null)
    //        print("ERROR !! 超過一份 ContinuousBattleManager 存在: ");
    //    instance = this;
    //}

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! 超過一份 ContinuousBattleManager 存在: ");
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
                //print("連續戰鬥結束 ................ !!");
                //DoEndAllBattle();
                isGoingContinuousBattle = false;
            }
            else
                isGoingContinuousBattle = true;
        }
    }

    static public ContinuousBattleDataBase GetNextBattleData()      //用來檢查目前進行中的戰鬥是否已經是最終戰
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

    //離開 Scene 時呼叫，如果沒有事先呼叫 GoToNext 的情況，必須視為戰鬥被中止，清除記錄
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

    //========== 連同位置一起記錄的新版本 ===============  TODO: 應該整合到 BattlePlayerData 中
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
        //print("收集戰鬥巫靈: " + dollID + " Group: " + group + " Index: " + index);

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
