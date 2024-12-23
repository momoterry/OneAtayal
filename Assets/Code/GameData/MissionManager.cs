using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string Title;
    public string ObjectiveText;
    //public string scene;
    public string sceneText;
    public string rewardText;
    public ContinuousBattleDataBase[] battles;
    public enum TYPE
    {
        NORMAL,     //單純走到底
        BOSS,       //擊殺目標
        EXPLORE,
    }
    public TYPE type;
    public enum SCALE
    {
        LARGE,
        MEDIUM,
        SMALL,
    }
    public SCALE scale;
    public int dollLimit;

    [System.Serializable]
    public class RewardData
    {
        public int Monney = 1000;
        [System.Serializable]
        public class RewardItemDef
        {
            public string ITEM_ID;
            public int num_min;
            public int num_max;
        }
        public RewardItemDef[] items;
    }
    public RewardData rewardData;
}

public class MissionManager : GlobalSystemBase
{
    public ContinuousMORoomPathData[] DemoBattleData;   //測試時用

    static MissionManager instance;
    static public MissionManager GetInstance() { return instance; }

    // ====== 註冊任務開關時的 CallBack
    public delegate void MissionManagerCB( MissionData mission);

    static public void AddAcceptMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        instance._AddAcceptMissionCB(cb, isRemove);
    }
    static public void AddCancelMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        instance._AddCancelMissionCB(cb, isRemove);
    }

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! 超過一份 MissionManager 存在 ... ");
        instance = this;
        base.InitSystem();
    }

    static public List<MissionData> GenerateMissions() 
    {
        return instance._GenerateMissions();
    }

    static public string GetScaleText(MissionData.SCALE scale)
    {
        switch (scale)
        {
            case MissionData.SCALE.SMALL:
                return "小";
            case MissionData.SCALE.MEDIUM:
                return "中";
            case MissionData.SCALE.LARGE:
                return "大";
        }
        return "XX";
    }

    static public MissionData GetCurrMission()
    {
        return instance._GetCurrMission();
    }

    static public void AcceptMission(MissionData mission)
    {
        instance._AcceptMission(mission);
    }
    static public void CancelCurrMission()
    {
        instance._CancelCurrMission();
    }

    static public void CompleteCurrMission()        //任務成功完成，得到獎勵 (但還沒「結束」任務)
    {
        print("MissionManager.CompleteCurrMission()");
        instance._CompleteCurrMission();
    }

    static public void FinishCurrMission()          //不管是成功與否，任務「結束」
    {
        print("MissionManager.FinishCurrMission()");
        instance._FinishCurrMission();
    }

    static public void StartCurrMission()
    {
        instance._StartCurrMission();
    }

    static public void StartMission(MissionData mission)
    {
        ContinuousBattleManager.StartNewBattle(mission.battles);

        string sceneName = mission.battles[0].scene;
        BattleSystem.GetInstance().OnGotoScene(sceneName, "");
    }

    //================================================================

    protected MissionData currMission = null;

    protected void _AcceptMission(MissionData mission)
    {
        if (currMission != null)
        {
            One.ERROR("AcceptMission 錯誤，已存在 Mission: " + mission.Title);
        }
        currMission = mission;
        foreach (MissionManagerCB cb in acceptCBs)
        {
            cb(currMission);
        }
    }

    protected MissionData _GetCurrMission() { return currMission; }

    protected void _StartCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("StartCurrMission 錯誤，沒有接收中的任務");
            return;
        }
        StartMission(currMission);
    }


    protected void _CancelCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("CancelCurrMission 錯誤，沒有接收中的任務");
        }
        foreach (MissionManagerCB cb in cancelCBs) 
        {
            cb(currMission);
        }

        currMission = null;
    }


    protected void _FinishCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("FinishCurrMission 錯誤，沒有接收中的任務");
        }
        currMission = null;
    }


    //任務完成得到獎勵，因為還留在關卡中的關係，任務還沒 Finish
    protected void _CompleteCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("CompleteCurrMission 錯誤，沒有接收中的任務");
            return;
        }
        SystemUI.ShowMessageBox(null, "得到獎勵錢錢 " + currMission.rewardData.Monney);
        GameSystem.GetPlayerData().AddMoney(currMission.rewardData.Monney);
        for (int i=0; i<currMission.rewardData.items.Length; i++)
        {
            int itemNum = Random.Range(currMission.rewardData.items[i].num_min, currMission.rewardData.items[i].num_max+1);
            GameSystem.GetPlayerData().AddItem(currMission.rewardData.items[i].ITEM_ID, itemNum);
            print("加入了 " + currMission.rewardData.items[i].ITEM_ID + " " + itemNum + "個");
        }
    }


    protected List<MissionData> _GenerateMissions()
    {
        MissionData.SCALE[] scales = {MissionData.SCALE.SMALL,MissionData.SCALE.MEDIUM,MissionData.SCALE.LARGE,};
        MissionData.TYPE[] types = {MissionData.TYPE.EXPLORE, MissionData.TYPE.NORMAL, MissionData.TYPE.BOSS };
        int[] dollLimits = { 10, 15, 20 };
        string[] titles = {"地洞探索","沙漠大會戰","地城的陰影" };
        string[] ObjectiveTexts = { "到達地洞深處", "清除敵人","擊殺 BOSS" };
        string[] scenes = { "DemoRoomPath", "RoomPathDesert", "RoomPathDungeon" };
        string[] sceneTexts = { "地洞","沙漠","地城" };
        string[] rewardTexts = {"晶石","沙石素材","紅寶石" };



        List < MissionData > missions = new List < MissionData >();
        for (int i = 0; i < 3; i ++) 
        {
            MissionData data = new();
            data.Title = titles[i];
            data.type = types[i];// MissionData.TYPE.EXPLORE;
            //data.scene = scenes[i];
            //data.ObjectiveText = "到達地洞深處";
            data.ObjectiveText = ObjectiveTexts[i];
            data.sceneText = sceneTexts[i];
            data.rewardText = rewardTexts[i];
            data.scale = scales[i];
            data.dollLimit = dollLimits[i];
            data.battles = new ContinuousBattleDataBase[1];
            data.battles[0] = DemoBattleData[i];
            missions.Add(data);
        }
        return missions;
    }

    //==================== 處理任務接受、取消時的行為用
    protected List<MissionManagerCB> acceptCBs = new List<MissionManagerCB>();
    protected List<MissionManagerCB> cancelCBs = new List<MissionManagerCB>();
    protected void _AddAcceptMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        if (!isRemove)
        {
            acceptCBs.Add(cb);
        }
        else
        {
            if (!acceptCBs.Remove(cb))
            {
                One.ERROR("MissionManager Remove AcceptCB ERROR");
            }
            else
                print("移除 Accept CB 成功");
        }
    }
    protected void _AddCancelMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        if (!isRemove)
        {
            cancelCBs.Add(cb);
        }
        else
        {
            if (!cancelCBs.Remove(cb))
            {
                One.ERROR("MissionManager Remove Cancel CB ERROR");
            }
            else
                print("移除 Cancel CB 成功");
        }
    }
}
