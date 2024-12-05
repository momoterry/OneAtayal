using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MissionCard : MonoBehaviour
{
    public Text TitleText;
    public Text leftText;
    public Text rightText;

    protected MissionBoardMenu theMenu;

    protected MissionData myData;

    public void InitValue(MissionData data, MissionBoardMenu _menu)
    {
        theMenu = _menu;

        myData = data;
        TitleText.text = data.Title;
        TitleText.color = theMenu.typeColors[(int)data.type];

        //leftText.text = "任務目標:\r\n";
        //leftText.text += "規模:" + MissionManager.GetScaleText(data.scale) + "\r\n";
        

        rightText.text = data.ObjectiveText + "\r\n";
        rightText.text += MissionManager.GetScaleText(data.scale) + "\r\n";
        rightText.text += data.dollLimit + "\r\n";
        rightText.text += data.sceneText;
        rightText.text += data.rewardText;
    }
}
