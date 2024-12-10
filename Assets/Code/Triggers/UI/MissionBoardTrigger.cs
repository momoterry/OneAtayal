using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoardTrigger : MonoBehaviour
{
    public MissionBoardMenu theMenu;

    public bool useTestMission = false;

    [System.Serializable]
    public class MissionDataRoomPathTest
    {
        public MissionData missionData;
        public ContinuousMORoomPathData battleData;
    }
    public MissionDataRoomPathTest[] testMissions;


    protected List<MissionData> missionList;

    void Start()
    {

        missionList = MissionManager.GenerateMissions();

        if (useTestMission && testMissions!= null && testMissions.Length > 0)
        {
            for (int i=0; i<testMissions.Length; i++)
            {
                testMissions[i].missionData.battles = new ContinuousBattleDataBase[1];
                testMissions[i].missionData.battles[0] = testMissions[i].battleData;
                if (i< missionList.Count)
                    missionList[i] = testMissions[i].missionData;
                else
                    missionList.Add(testMissions[i].missionData);
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        theMenu.OpenMenu(missionList);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }

}
