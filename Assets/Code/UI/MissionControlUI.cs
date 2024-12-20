using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionControlUI : MonoBehaviour
{
    public Text titleText;
    public Text msgText;
    public Text countText;
    public GameObject MissionMessageBox;
    public GameObject MissionCoompleteWindowRoot;

    float missionMessageBoxTimeLeft = 0;
    float missionCompleteWindowTimeLeft = 0;

    public void ShowObjectiveDoneMessage( string title, string objective, int objDone, int objTotal)
    {
        countText.text = objDone.ToString() + " / " + objTotal.ToString();
        msgText.text = objective;
        titleText.text = title;
        MissionMessageBox.SetActive(true);
        missionMessageBoxTimeLeft = 2.0f;
    }

    public void ShowMissionCompleteWindow( MissionData _mission)
    {
        MissionCoompleteWindowRoot.SetActive(true);
        missionCompleteWindowTimeLeft = 2.0f;
    }


    void Update()
    {
        if (missionMessageBoxTimeLeft > 0)
        {
            missionMessageBoxTimeLeft -= Time.deltaTime;
            if (missionMessageBoxTimeLeft < 0)
            {
                MissionMessageBox.SetActive(false);
            }
        }
        if (missionCompleteWindowTimeLeft > 0)
        {
            missionCompleteWindowTimeLeft -= Time.deltaTime;
            if (missionCompleteWindowTimeLeft < 0)
            {
                MissionCoompleteWindowRoot.SetActive(false);
            }
        }
    }
}
