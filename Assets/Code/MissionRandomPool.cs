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
        data.battles[0] = scenes[si].baseBattleData;
        mList.Add(data);
        return mList;
    }

}
