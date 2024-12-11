using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMissionBoard : MissionBoardTrigger
{
    [System.Serializable]
    public class MissionDataRoomPathTest
    {
        public MissionData missionData;
        public ContinuousMORoomPathData battleData;
    }
    public MissionDataRoomPathTest testMission;

    [System.Serializable]
    public class MissionDataRoomPathContinuousTest
    {
        public MissionData missionData;
        public ContinuousMORoomPathData[] battles;
    }

    public MissionDataRoomPathContinuousTest[] testLongMission;


    void Start()
    {
        missionList = new List<MissionData>();
        //if (testMissions != null && testMissions.Length > 0)
        //{
        //    for (int i = 0; i < testMissions.Length; i++)
        //    {
        //testMissions[i].missionData.battles = new ContinuousBattleDataBase[1];
        //testMissions[i].missionData.battles[0] = testMissions[i].battleData;
        //missionList.Add(testMissions[i].missionData);
        //    }
        //}
        testMission.missionData.battles = new ContinuousBattleDataBase[1];
        testMission.missionData.battles[0] = testMission.battleData;
        missionList.Add(testMission.missionData);

        if (testLongMission != null && testLongMission.Length > 0)
        {
            for (int i = 0; i < testLongMission.Length; i++)
            {
                testLongMission[i].missionData.battles = testLongMission[i].battles;
                missionList.Add(testLongMission[i].missionData);
            }
        }
    }
}
