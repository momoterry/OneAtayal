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

        leftText.text = "���ȥؼ�:\r\n";
        leftText.text += "�W��:  " + MissionManager.GetScaleText(data.scale) + "\r\n";
        leftText.text += "�a�I:  �a�}";

        rightText.text = data.ObjectiveText + "\r\n";
        rightText.text += "���F:  " + data.dollLimit + "\r\n";
        rightText.text += "���y:  ����";
    }

    public void OnButtonClicked()
    {
        print("�ڳQ��W�F " + myData.Title);
    }

}
