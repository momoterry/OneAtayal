using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

//在關卡戰鬥中的玩家資料，在戰鬥結束後會被清除重設
public class BattlePlayerCrossSceneData
{
    public int currExp;
    public int currExpMax;
    public int currBattleLV;
    public int battleLVPoint;
    public BattlePlayerData.BattleEvent battleEvent;
    public Dictionary<string, BattleSaveBase> customSaves = new Dictionary<string, BattleSaveBase>();       //各自系統記錄的資訊
}
//給其它戰鬥系統在連續戰鬥時需要記錄的資料基礎
public class BattleSaveBase { }

public class BattlePlayerData : MonoBehaviour
{
    const int MAX_BATTLE_LEVEL = 20;
    const int INIT_EXP_MAX = 50;
    public float EXP_MAX_STEP = 1.5f;

    protected int currExp = 0;
    protected int currExpMax = INIT_EXP_MAX;
    protected int currBattleLV = 1;
    protected int battleLVPoint = 4;
    //protected BattlePlayerCrossSceneData data;
    protected Dictionary<string, BattleSaveBase> customSaves = new Dictionary<string, BattleSaveBase>();

    protected int[] maxExpArray = new int[MAX_BATTLE_LEVEL];

    static private BattlePlayerData instance;
    static public BattlePlayerData GetInstance() { return instance; }
    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 BattlePlayerData 存在 ");
        instance = this;

        maxExpArray[0] = 1;
        maxExpArray[1] = INIT_EXP_MAX;
        for (int i=2; i<MAX_BATTLE_LEVEL; i++)
        {
            maxExpArray[i] = (int)(maxExpArray[i - 1] * EXP_MAX_STEP);
        }

        if (ContinuousBattleManager.GetInstance() != null)
        {
            BattlePlayerCrossSceneData data = ContinuousBattleManager.GetBattlePlayerCrossSceneData();
            if (data != null)
            {
                //print("有 BattlePlayerCrossSceneData，復元資料 !!");
                currBattleLV = data.currBattleLV;
                currExp = data.currExp;
                currExpMax = data.currExpMax;
                battleLVPoint = data.battleLVPoint;
                battleEvent = data.battleEvent;
                customSaves = data.customSaves;
            }
        }
        //else
        //{
        //    battleEvent = new BattleEvent();
        //    customSaves = new Dictionary<string, BattleSaveBase>();
        //}
    }

    // Public Functions
    public BattlePlayerCrossSceneData GetCrossSceneData()
    {
        BattlePlayerCrossSceneData data = new BattlePlayerCrossSceneData();
        data.currBattleLV = currBattleLV;
        data.currExp = currExp;
        data.currExpMax = currExpMax;
        data.battleLVPoint = battleLVPoint;
        data.battleEvent = battleEvent;
        data.customSaves = customSaves;
        return data;
    }
    public int GetBattleLVPoints() { return battleLVPoint; }
    public void AddBattleLVPoints( int point ) { 
        battleLVPoint += point;
        BattleSystem.GetPC().OnBattlePointsChange(point);
        BattleSystem.GetInstance().theBattleHUD.bLVMenu.OnSetBattlePoint(battleLVPoint);
    }
    public int GetBattleLevel() { return currBattleLV; }
    public int GetBattleExp() { return currExp; }
    public int GetBattleExpMax() { return currExpMax; }
    public int GetMaxBattleLevel() { return MAX_BATTLE_LEVEL; }

    // Start is called before the first frame update
    void Start()
    {
        BattleSystem.GetInstance().theBattleHUD.bLVMenu.OnSetBattlePoint(battleLVPoint);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnKillEnemy(Enemy e)
    {
        //暴力法  TODO: 用表格設定經驗值
        if (BattleSystem.GetInstance().IsBattleLevelUp)
        {
            //AddExp(30);
            AddExp(DropManager.GetInstance().GetExpByID(e.GetDropID()));
        }
    }
    public void AddExp(int value)
    {
        if (currBattleLV == MAX_BATTLE_LEVEL)
            return;
        currExp += value;
        int originalLV = currBattleLV;
        while (currExp >= currExpMax)
        {
            currBattleLV++;
            battleLVPoint++;
            if (currBattleLV < MAX_BATTLE_LEVEL)
            {
                currExp -= currExpMax;
                currExpMax = maxExpArray[currBattleLV];
            }
            else
            {
                currBattleLV = MAX_BATTLE_LEVEL;    //確保
                currExp = 0;       //滿級了
            }
        }
        if (currBattleLV != originalLV)
        {
            DoBattleLVUp(currBattleLV - originalLV);
        }
    }

    protected void DoBattleLVUp(int addLV)
    {
        BattleSystem.GetPC().OnBattlePointsChange(addLV);
        BattleSystem.GetInstance().theBattleHUD.bLVMenu.OnSetBattlePoint(battleLVPoint);
        //print("升級啦，升了" + addLV + " 級，現在是 " + currBattleLV + " 級");
    }


    //============================= BattleEvent 相關 ========================================
    //目前 BattleEvent 先不支援跨 Scene 的記錄
    //如果之後要的話，得放到 BattlePlayerCrossSceneData 當中

    public class BattleEvent
    {
        public Dictionary<string, bool> boolEvents = new Dictionary<string, bool>();
        public Dictionary<string, int> intEvents = new Dictionary<string, int>();
    }
    protected BattleEvent battleEvent = new BattleEvent();

    public bool GetEventBool(string eventID)
    {
        if (battleEvent.boolEvents.ContainsKey(eventID))
            return battleEvent.boolEvents[eventID];
        return false;
    }

    public int GetEventInt(string eventID)
    {
        if (battleEvent.intEvents.ContainsKey(eventID))
            return battleEvent.intEvents[eventID];
        return 0;
    }

    public void SetEventBool(string eventID, bool isTrue)
    {
        if (battleEvent.boolEvents.ContainsKey(eventID))
            battleEvent.boolEvents[eventID] = isTrue;
        else
            battleEvent.boolEvents.Add(eventID, isTrue);
    }

    public void AddEventInt(string eventID, int numAdded = 1)
    {
        if (battleEvent.intEvents.ContainsKey(eventID))
            battleEvent.intEvents[eventID] += numAdded;
        else
            battleEvent.intEvents.Add(eventID, numAdded);
    }

    //============================= 給其它戰鬥系統使用 ========================================

    public BattleSaveBase SysncSaveData(string dataID, BattleSaveBase newData)
    {
        if (!customSaves.ContainsKey(dataID))
        {
            customSaves.Add(dataID, newData);
        }

        return customSaves[dataID];
    }

}
