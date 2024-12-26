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
        public RoomGameplayBase objectiveGameRef;
    }
    public ObjectiveData[] objectives;

    [System.Serializable]
    public class SceneData
    {
        public string sceneName;
        public ContinuousMORoomPathData baseBattleData;
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
        data.battles = new ContinuousBattleDataBase[1];
        //Battle 的生成
        ContinuousMORoomPathData battle = new();
        ContinuousMORoomPathData bRef = scenes[si].baseBattleData;

        battle.scene = bRef.scene;
        battle.name = bRef.name;
        battle.cameraAdjust = bRef.cameraAdjust;
        battle.puzzleWidth = bRef.puzzleWidth;
        battle.puzzleHeight = bRef.puzzleHeight;
        battle.roomWidth = bRef.roomWidth;
        battle.roomHeight = bRef.roomHeight;
        battle.pathWidth = bRef.pathWidth;
        battle.pathHeight = bRef.pathHeight;
        battle.mazeDir = bRef.mazeDir;
        battle.MaxMainDeep = bRef.MaxMainDeep;
        battle.MaxBranchDeep = bRef.MaxBranchDeep;
        battle.gameManagerRef = bRef.gameManagerRef;
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
