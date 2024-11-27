using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string Title;
    public string scene;
    public GameManagerDataBase game;
    public enum TYPE
    {
        NORMAL,     //��¨��쩳
        BOSS,       //�����ؼ�
        EXPLORE,
    }
    public TYPE type;
}

public class MissionManager : GlobalSystemBase
{
    static public List<MissionData> GenerateMissions() 
    {
        return _GenerateMissions();
    }

    static protected List<MissionData> _GenerateMissions()
    {
        List < MissionData > missions = new List < MissionData >();
        for (int i = 0; i < 3; i ++) 
        {
            MissionData data = new();
            data.Title = "�a�}����";
            data.type = MissionData.TYPE.EXPLORE;
            missions.Add(data);
        }
        return missions;
    }

}
