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

    float missionMessageBoxTimeLeft = 0;

    public void ShowObjectiveDoneMessage( string title, string objective, int objDone, int objTotal)
    {
        countText.text = objDone.ToString() + " / " + objTotal.ToString();
        msgText.text = objective;
        titleText.text = title;
        MissionMessageBox.SetActive(true);
        missionMessageBoxTimeLeft = 2.0f;
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
    }
}
