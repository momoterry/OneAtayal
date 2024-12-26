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

        //public enum OBJECTIVE_TYPE
        //{
        //    MAIN_END,
        //    BRANCH_END,
        //}
        //public MissionData.OBJECTIVE_TYPE objectiveType;
        //public RoomGameplayBase objectiveGameRef;
        public MissionData.ObjectiveGame game;
        public int MainPathAdd = -1;
        public int BranchAdd = 2;
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



    public List<MissionData> GenerateMissions()
    {
        List<MissionData> mList = new List<MissionData>();

        int oi = 0;
        int si = 0;

        MissionData data = new();
        data.Title = "怎麼取名好呢";                          //TODO
        data.type = objectives[oi].misisonType;
        data.ObjectiveText = objectives[oi].objectiveText;
        data.sceneText = scenes[si].sceneName;
        data.rewardText = "獎勵還沒設定";                     //TODO
        data.scale = MissionData.SCALE.MEDIUM;                  //TODO
        data.dollLimit = 10;                                    //TODO
        data.objectiveGame = objectives[oi].game;
        data.battles = new ContinuousBattleDataBase[1];
        //Battle 的生成
        ContinuousMORoomPathData battle = new();
        //ContinuousMORoomPathData bRef = scenes[si].baseBattleData;

        //battle.scene = bRef.scene;
        //battle.name = bRef.name;
        //battle.cameraAdjust = bRef.cameraAdjust;
        //battle.puzzleWidth = bRef.puzzleWidth;
        //battle.puzzleHeight = bRef.puzzleHeight;
        //battle.roomWidth = bRef.roomWidth;
        //battle.roomHeight = bRef.roomHeight;
        //battle.pathWidth = bRef.pathWidth;
        //battle.pathHeight = bRef.pathHeight;
        //battle.mazeDir = bRef.mazeDir;
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
        battle.MaxBranchDeep = 3 + objectives[oi].BranchAdd;
        battle.gameManagerRef = scenes[si].GM_Ref;
        battle.gameManagerData = new();
        battle.gameManagerData.difficultRateMin = 1.0f;
        battle.gameManagerData.difficultRateMax = 2.0f;
        battle.gameManagerData.forceRandomObjectNum = 12;

        data.battles[0] = battle;
        //data.battles[0] = scenes[si].baseBattleData;
        mList.Add(data);
        return mList;
    }

}
