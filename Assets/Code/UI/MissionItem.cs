using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    public Text TitleText;
    public Text leftText;
    public Text rightText;

    protected MissionData myData;
    public void InitValue(MissionData data)
    {
        myData = data;
        TitleText.text = data.Title;

        leftText.text = "任務目標:\r\n";
        leftText.text += "規模:  " + MissionManager.GetScaleText(data.scale) + "\r\n";
        leftText.text += "地點:  地洞";

        rightText.text = data.ObjectiveText + "\r\n";
        rightText.text += "巫靈:  " + data.dollLimit + "\r\n";
        rightText.text += "獎勵:  晶石";
    }

    public void OnButtonClicked()
    {
        print("我被選上了 " + myData.Title);
    }

}
