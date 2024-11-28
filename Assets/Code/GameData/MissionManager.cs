using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string Title;
    public string ObjectiveText;
    public string scene;
    public string sceneText;
    public string rewardText;
    public ContinuousBattleDataBase[] battles;
    public enum TYPE
    {
        NORMAL,     //��¨��쩳
        BOSS,       //�����ؼ�
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
    public ContinuousMORoomPathData[] DemoBattleData;   //���ծɥ�

    static MissionManager instance;
    static public MissionManager GetInstance() { return instance; }

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� MissionManager �s�b ... ");
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
                return "�p";
            case MissionData.SCALE.MEDIUM:
                return "��";
            case MissionData.SCALE.LARGE:
                return "�j";
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
        MissionData.TYPE[] types = {MissionData.TYPE.EXPLORE, MissionData.TYPE.NORMAL, MissionData.TYPE.BOSS };
        int[] dollLimits = { 10, 15, 20 };
        string[] titles = {"�a�}����","�F�z�j�|��","�a�������v" };
        string[] ObjectiveTexts = { "��F�a�}�`�B", "�M���ĤH","���� BOSS" };
        string[] scenes = { "DemoRoomPath", "RoomPathDesert", "RoomPathDungeon" };
        string[] sceneTexts = { "�a�}","�F�z","�a��" };
        string[] rewardTexts = {"����","�F�ۯ���","���_��" };



        List < MissionData > missions = new List < MissionData >();
        for (int i = 0; i < 3; i ++) 
        {
            MissionData data = new();
            data.Title = titles[i];
            data.type = types[i];// MissionData.TYPE.EXPLORE;
            data.scene = scenes[i];
            //data.ObjectiveText = "��F�a�}�`�B";
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

}
