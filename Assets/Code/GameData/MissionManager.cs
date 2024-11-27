using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string Title;
    public string ObjectiveText;
    public string scene;
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
}

public class MissionManager : GlobalSystemBase
{
    public ContinuousMORoomPathData[] DemoBattleData;   //測試時用

    static MissionManager instance;
    static public MissionManager GetInstance() { return instance; }

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

    static public void StartMission(MissionData mission)
    {
        ContinuousBattleManager.StartNewBattle(mission.battles);

        string sceneName = mission.battles[0].scene;
        BattleSystem.GetInstance().OnGotoScene(sceneName, "");
    }

    protected List<MissionData> _GenerateMissions()
    {
        MissionData.SCALE[] scales = {MissionData.SCALE.SMALL,MissionData.SCALE.MEDIUM,MissionData.SCALE.LARGE,};
        int[] dollLimits = { 10, 15, 20 };
        string[] titles = {"地洞探索","深入地洞","大型地洞探險" };
        //string[] ObjectiveText = {  };

        List < MissionData > missions = new List < MissionData >();
        for (int i = 0; i < 3; i ++) 
        {
            MissionData data = new();
            data.Title = titles[i];
            data.type = MissionData.TYPE.EXPLORE;
            data.scene = "DemoRoomPath";
            data.ObjectiveText = "到達地洞深處";
            data.scale = scales[i];
            data.dollLimit = dollLimits[i];
            data.battles = new ContinuousBattleDataBase[1];
            data.battles[0] = DemoBattleData[i];
            missions.Add(data);
        }
        return missions;
    }

}
