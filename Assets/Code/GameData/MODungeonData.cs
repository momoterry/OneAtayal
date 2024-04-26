using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �ΨӴy�z�@�� MazeOneBase �g�c�Ѽƪ���Ƶ��c
// �ثe�D�n�ΨӤ䴩 CSV ���ɮ״y�z
// �H��i�H�� CMazeJsonData �]���Y�ص{�פW����X




//�@�� MazeOne Dungeion ���y�z
public class MODungeonData
{
    public string DungeonID;
    public List<MODungeonStageData> stageList = new List<MODungeonStageData>();      //�򥻤W���Ӫ��W�����ǱƦC


    protected ContinuousMOData StageToContinuousMOData(MODungeonStageData stage)
    {
        //Debug.Log("StageToContinuousMOData" + stage.Level);
        ContinuousMOData data = new ContinuousMOData();
        data.scene = stage.SceneName;
        data.puzzleWidth = stage.PuzzleWidth;
        data.puzzleHeight = stage.PuzzleHeight;
        data.pathRate = stage.PathRate;
        data.name = "�a�� " + stage.Level;
        data.gameDiffcultRateMin = stage.DifficultStart;
        data.gameDiffcultRateMax = stage.DifficultEnd;
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
    public float PathRate;
    public float DifficultStart;
    public float DifficultEnd;
}

public class MOStageGameplayData_Simple
{
    public string DungeonID;
    public int Level;           //�ĴX�h

    public float DifficultStart;
    public float DifficultEnd;
    public string Reward1;
    public float Reward1_Num;
}

