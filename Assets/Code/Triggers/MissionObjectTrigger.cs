using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionObjective
{
    public string OBJECTIVE_ID;
    public string objectiveText;
    public Transform completePortalPos;     //如果沒有指定，就不會產生 Portal
}

public class MissionObjectTrigger : MonoBehaviour
{
    public MissionObjective myObjective;

    // Start is called before the first frame update
    void Start()
    {
        MissionController.GetInstance().RegisterObjective(myObjective);
    }

    public void OnTG(GameObject whoTG)
    {
        MissionController.GetInstance().CompleteObjective(myObjective);
    }
}
