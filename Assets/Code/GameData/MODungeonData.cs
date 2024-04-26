using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 用來描述一個 MazeOneBase 迷宮參數的資料結構
// 目前主要用來支援 CSV 的檔案描述
// 以後可以跟 CMazeJsonData 也做某種程度上的整合




//一個 MazeOne Dungeion 的描述
public class MODungeonData
{
    public string DungeonID;
    public List<MODungeonStageData> stageList = new List<MODungeonStageData>();      //基本上按照表單上的順序排列


    protected ContinuousMOData StageToContinuousMOData(MODungeonStageData stage)
    {
        //Debug.Log("StageToContinuousMOData" + stage.Level);
        ContinuousMOData data = new ContinuousMOData();
        data.scene = stage.SceneName;
        data.puzzleWidth = stage.PuzzleWidth;
        data.puzzleHeight = stage.PuzzleHeight;
        data.pathRate = stage.PathRate;
        data.name = "地城 " + stage.Level;
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

//MazeOne Dungeion 中每一「層」的描述

public class MODungeonStageData
{
    public string DungeonID;
    public int Level;           //第幾層
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
    public int Level;           //第幾層

    public float DifficultStart;
    public float DifficultEnd;
    public string Reward1;
    public float Reward1_Num;
}

