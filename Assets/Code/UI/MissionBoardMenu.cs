using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoardMenu : MonoBehaviour
{
    public Transform MenuRoot;
    public void OpenMenu(List<MissionData> missionList)
    {

        //CreateLevelMenuItems();
        MenuRoot.gameObject.SetActive(true);
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        //ClearLevelMenuItems();
        MenuRoot.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(false);
    }
}
