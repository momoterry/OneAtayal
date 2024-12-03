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

        leftText.text = "任務目標:\r\n";
        leftText.text += "規模:  " + MissionManager.GetScaleText(data.scale) + "\r\n";
        leftText.text += "地點:  " + data.sceneText;

        rightText.text = data.ObjectiveText + "\r\n";
        rightText.text += "巫靈:  " + data.dollLimit + "\r\n";
        rightText.text += "獎勵:  " + data.rewardText;// 晶石";
    }

    public void OnButtonClicked()
    {
        print("我被選上了 " + myData.Title);

        theMenu.OnItemClicked(this, myData);
    }

}
