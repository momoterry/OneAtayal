using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoardTrigger : MonoBehaviour
{
    public MissionBoardMenu theMenu;

    public MissionRandomPool missionPool; //如果有指定的話，用 Pool 所產生的任務

    public bool directGo = false;   //任務一接就進入關卡


    protected List<MissionData> missionList;

    void Start()
    {
        GenerateMissionList();
    }

    virtual protected void GenerateMissionList()
    {
        if (missionPool != null)
        {
            missionList = missionPool.GenerateMissions();
        }
        else
        {
            missionList = MissionManager.GenerateMissions();
        }
    }

    public void OnTG(GameObject whoTG)
    {
        theMenu.OpenMenu(missionList, directGo);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }


    //void AcceptMissionCB(MissionData mission)
    //{
    //    if (directGo)
    //    {
    //        print("直接進任務吧 !!" + mission.Title);
    //        MissionManager.StartCurrMission();
    //    }
    //}

    //void OnDestroy()
    //{
    //    MissionManager.AddAcceptMissionCB(AcceptMissionCB, true);
    //}

}
