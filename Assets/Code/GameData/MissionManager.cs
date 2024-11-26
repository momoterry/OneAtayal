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
        NORMAL,     //單純走到底
        BOSS,       //擊殺目標
        EXPLORE,
    }
    public TYPE type;
}

public class MissionManager : GlobalSystemBase
{

}
