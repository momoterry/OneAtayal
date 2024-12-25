using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoardTrigger : MonoBehaviour
{
    public MissionBoardMenu theMenu;

    public MissionRandomPool missionPool; //�p�G�����w���ܡA�� Pool �Ҳ��ͪ�����

    public bool directGo = false;   //���Ȥ@���N�i�J���d


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
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��
    }


    //void AcceptMissionCB(MissionData mission)
    //{
    //    if (directGo)
    //    {
    //        print("�����i���ȧa !!" + mission.Title);
    //        MissionManager.StartCurrMission();
    //    }
    //}

    //void OnDestroy()
    //{
    //    MissionManager.AddAcceptMissionCB(AcceptMissionCB, true);
    //}

}
