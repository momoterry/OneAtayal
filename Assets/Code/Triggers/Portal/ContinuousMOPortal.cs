using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContinuousMOData : ContinuousBattleDataBase
{
    public int puzzleWidth;
    public int puzzleHeight;
    public float pathRate;
    public int roomWidth = 15;
    public int roomHeight = 20;
    public int pathWidth = 5;
    public int pathHeight = 5;
    public MG_MazeOne.MAZE_DIR mazeDir;
    public bool finishAtDeepest;

    public MazeGameManagerBase gameManagerRef;
    public GameManagerDataBase gameManagerData;
    public GameObject initGameplayRef;
}

//��� ContinuousMOData ���� Gameplay �ѼƩw�q�򩳡A�i�H�Q�Ψ��X�R�H�䴩���P�� Gameplay ����
[System.Serializable]
public class GameManagerDataBase
{
    public float difficultRateMin = 1.0f;   //�̤p���ײv�A�Ψӽվ�ĤH�ƶq
    public float difficultRateMax = 2.0f;   //�̤j���ײv�A�Ψӽվ�ĤH�ƶq
    public int enmeyLV = 1;
    public string specialReward;
    public float specialRewardNum;
}

public class ContinuousMOPortal : ScenePortal
{
    public ContinuousMOData[] mazeLevelDatas;

    protected override void DoTeleport()
    {
        if (mazeLevelDatas.Length > 0 && mazeLevelDatas[0].scene != "")
        {
            ContinuousBattleManager.StartNewBattle(mazeLevelDatas);

            sceneName = mazeLevelDatas[0].scene;
            base.DoTeleport();
        }
    }
}
