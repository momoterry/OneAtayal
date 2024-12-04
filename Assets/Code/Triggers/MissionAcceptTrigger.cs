using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionAcceptTrigger : MonoBehaviour
{
    public GameObject[] triggerTargetsOnAccept;
    public GameObject[] triggerTargetsOnCancel;

    // Start is called before the first frame update
    void Start()
    {
        MissionManager.AddAcceptMissionCB(OnMissionAccept);
        MissionManager.AddCancelMissionCB(OnMissionCancel);
    }

    public void OnMissionAccept( MissionData mission)
    {
        print("OnMissionAccept !!!!");
        foreach (GameObject obj in triggerTargetsOnAccept) 
        { 
            obj.SendMessage("OnTG", gameObject);
        }
    }

    public void OnMissionCancel(MissionData mission)
    {
        print("���Ȩ��� !!!!");
        foreach (GameObject obj in triggerTargetsOnCancel)
        {
            obj.SendMessage("OnTG", gameObject);
        }
    }

    void OnDestroy()
    {
        MissionManager.AddAcceptMissionCB(OnMissionAccept, true);
        MissionManager.AddCancelMissionCB(OnMissionCancel, true);
    }

}
