using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoardTrigger : MonoBehaviour
{
    public MissionBoardMenu theMenu;

    protected List<MissionData> missionList;

    void Start()
    {
        missionList = MissionManager.GenerateMissions();
    }

    void OnTG(GameObject whoTG)
    {
        theMenu.OpenMenu(missionList);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }

}
