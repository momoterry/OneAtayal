using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    public Text TitleText;
    public Text leftText;
    public Text rightText;

    //public Color[] typeColors;

    protected MissionBoardMenu theMenu;

    protected MissionData myData;
    public void InitValue(MissionData data, MissionBoardMenu _menu)
    {
        theMenu = _menu;

        myData = data;
        TitleText.text = data.Title;
        TitleText.color = theMenu.typeColors[(int)data.type];

        leftText.text = "���ȥؼ�:\r\n";
        leftText.text += "�W��:  " + MissionManager.GetScaleText(data.scale) + "\r\n";
        leftText.text += "�a�I:  " + data.sceneText;

        rightText.text = data.ObjectiveText + "\r\n";
        rightText.text += "���F:  " + data.dollLimit + "\r\n";
        rightText.text += "���y:  " + data.rewardText;// ����";
    }

    public void OnButtonClicked()
    {
        print("�ڳQ��W�F " + myData.Title);

        theMenu.OnItemClicked(this, myData);
    }

}
