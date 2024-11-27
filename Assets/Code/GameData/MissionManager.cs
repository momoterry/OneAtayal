using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string Title;
    public string ObjectiveText;
    public string scene;
    public GameManagerDataBase game;
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
    static public List<MissionData> GenerateMissions() 
    {
        return _GenerateMissions();
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

    static protected List<MissionData> _GenerateMissions()
    {
        MissionData.SCALE[] scales = {MissionData.SCALE.SMALL,MissionData.SCALE.MEDIUM,MissionData.SCALE.LARGE,};
        int[] dollLimits = { 10, 15, 20 };
        string[] titles = {"�a�}����","�`�J�a�}","�j���a�}���I" };
        //string[] ObjectiveText = {  };

        List < MissionData > missions = new List < MissionData >();
        for (int i = 0; i < 3; i ++) 
        {
            MissionData data = new();
            data.Title = titles[i];
            data.type = MissionData.TYPE.EXPLORE;
            data.scene = "DemoRoomPath";
            data.ObjectiveText = "��F�a�}�`�B";
            data.scale = scales[i];
            data.dollLimit = dollLimits[i];
            missions.Add(data);
        }
        return missions;
    }

}
