using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MG_MazeOneBase;


// �ΨӴy�z�@�� MazeOneBase �g�c�Ѽƪ���Ƶ��c
// �ثe�D�n�ΨӤ䴩 CSV ���ɮ״y�z�A�N���e�ন ContinuousMOData 
// �H��i�H�� CMazeJsonData �]���Y�ص{�פW����X


//�@�� MazeOne Dungeion ���y�z
public class MODungeonData
{
    public string DungeonID;
    public List<MODungeonStageData> stageList = new List<MODungeonStageData>();      //�򥻤W���Ӫ��W�����ǱƦC

    protected MG_MazeOne.MAZE_DIR GetMazeDir(string str)
    {
        switch (str)
        {
            case "DOWN_TO_TOP":
                return MAZE_DIR.DONW_TO_TOP;
            case "TOP_TO_DOWN":
                return MAZE_DIR.TOP_TO_DOWN;
            case "LEFT_TO_RIGHT":
                return MAZE_DIR.LEFT_TO_RIGHT;
            case "RIGHT_TO_LEFT":
                return MAZE_DIR.RIGHT_TO_LEFT;
            case "INSIDE_OUT":
                break;
        }
        return MAZE_DIR.NONE;
    }

    protected ContinuousMOData StageToContinuousMOData(MODungeonStageData stage)
    {
        //Debug.Log("StageToContinuousMOData" + stage.Level);
        ContinuousMOData data = new ContinuousMOData();
        data.scene = stage.SceneName;
        data.puzzleWidth = stage.PuzzleWidth;
        data.puzzleHeight = stage.PuzzleHeight;
        data.mazeDir = GetMazeDir(stage.MazeDir);
        data.pathRate = stage.PathRate;
        data.name = "�a�� " + stage.Level;
        //data.gameDiffcultRateMin = stage.DifficultStart;
        //data.gameDiffcultRateMax = stage.DifficultEnd;
        //data.gameEnemyLV = stage.EnemyLV;
        data.gameManagerData = new GameManagerDataBase();
        if (stage.GameName != null && stage.GameName != "")
        {
            GameObject o = GameData.GetObjectRef(stage.GameName);
            data.gameManagerRef = o.GetComponent<MazeGameManagerBase>();
            if (!data.gameManagerRef)
                Debug.Log("ERROR!!!! Not valid GameManager: " + stage.GameName);
        }
        data.gameManagerData.difficultRateMin = stage.DifficultStart;
        data.gameManagerData.difficultRateMax = stage.DifficultEnd;
        data.gameManagerData.enmeyLV = stage.EnemyLV;
        data.gameManagerData.specialReward = stage.Reward1;
        data.gameManagerData.specialRewardNum = stage.RewardNum1;
        //Debug.Log("DifficultStart: " + stage.DifficultStart + " DifficultEnd: " + stage.DifficultEnd);
        return data;
    }

    public ContinuousMOData[] ToContinuousMODataArray()
    {
        ContinuousMOData[] data = new ContinuousMOData[stageList.Count];
        for (int i=0; i<stageList.Count; i++)
        {
            data[i] = StageToContinuousMOData(stageList[i]);
        }
        return data;
    }
}

//MazeOne Dungeion ���C�@�u�h�v���y�z

public class MODungeonStageData
{
    public string DungeonID;
    public int Level;           //�ĴX�h
    public string SceneName;
    public int PuzzleWidth;
    public int PuzzleHeight;
    public string MazeDir;
    public float PathRate;
    public bool StartAsPath;
    public bool EndAsPath;

    public string GameName;         //���w�� GM
    public float DifficultStart;
    public float DifficultEnd;
    public int EnemyLV;

    //���y������
    public string Reward1;
    public float RewardNum1;
}


//TODO: �U���ثe�ȮɥΤ���A������
//public class MOStageGameplayData_Simple
//{
//    public string DungeonID;
//    public int Level;           //�ĴX�h

//    public float DifficultStart;
//    public float DifficultEnd;
//    public string Reward1;
//    public float Reward1_Num;
//}

