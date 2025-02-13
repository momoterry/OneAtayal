using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionRandomPool : MonoBehaviour
{
    [System.Serializable]
    public class ObjectiveData
    {
        public MissionData.TYPE misisonType;
        public string objectiveText;

        public MissionData.ObjectiveGame game;
        public int MainPathAdd = -1;
        public int BranchAdd = 2;

        public MissionData.RewardData rewardData;
    }
    public ObjectiveData[] objectives;

    [System.Serializable]
    public class SceneData
    {
        public string sceneName;

        public string scene;
        public MazeGameManagerBase GM_Ref;

        //public ContinuousMORoomPathData baseBattleData;
    }
    public SceneData[] scenes;

    public MissionData.HelpDollData[] helpDolls;

    public List<MissionData> GenerateMissions()
    {
        List<MissionData> mList = new List<MissionData>();

        int oi = 0;
        int si = Random.Range(0, scenes.Length);
        int di = 0;

        MissionData data = new();
        data.Title = "怎麼取名好呢";                          //TODO
        data.type = objectives[oi].misisonType;
        data.ObjectiveText = objectives[oi].objectiveText;
        data.sceneText = scenes[si].sceneName;
        data.rewardText = "獎勵還沒設定";                     //TODO
        data.scale = MissionData.SCALE.MEDIUM;                  //TODO
        data.dollLimit = 10;                                    //TODO
        data.objectiveGame = objectives[oi].game;
        data.rewardData = objectives[oi].rewardData;

        //Battle 的生成
        data.battles = new ContinuousBattleDataBase[1];
        ContinuousMORoomPathData battle = new();
        battle.scene = scenes[si].scene;
        battle.name = scenes[si].sceneName;
        battle.cameraAdjust = 0;
        battle.puzzleWidth = 5;
        battle.puzzleHeight = 5;
        battle.roomWidth = 15;
        battle.roomHeight = 20;
        battle.pathWidth = 6;
        battle.pathHeight = 6;
        battle.mazeDir = MG_MazeOneBase.MAZE_DIR.DONW_TO_TOP;
        battle.MaxMainDeep = 6 + objectives[oi].MainPathAdd;
        battle.MaxBranchDeep = 2 + objectives[oi].BranchAdd;
        battle.gameManagerRef = scenes[si].GM_Ref;
        battle.gameManagerData = new();
        battle.gameManagerData.difficultRateMin = 1.0f;
        battle.gameManagerData.difficultRateMax = 2.0f;
        battle.gameManagerData.forceRandomObjectNum = 12;

        data.battles[0] = battle;

        //野巫靈的部份
        data.helpDoll = helpDolls[di];

        mList.Add(data);
        return mList;
    }

}
