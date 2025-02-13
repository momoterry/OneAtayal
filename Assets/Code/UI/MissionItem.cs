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

        leftText.text = "任務目標:";
        leftText.text += "\r\n規模:  " + MissionManager.GetScaleText(data.scale);
        leftText.text += "\r\n地點:  " + data.sceneText;

        if (data.helpDoll.dollRef)
        {
            Doll d = data.helpDoll.dollRef.GetComponentInChildren<Doll>();
            string dollID = null;
            if (d == null) 
            {
                DollCollect dCollect = data.helpDoll.dollRef.GetComponentInChildren<DollCollect>();
                if (dCollect)
                {
                    dollID = dCollect.spawnDollID;
                }
            }
            else
            {
                dollID = d.ID;
            }
            if (dollID != null && dollID != "")
            {
                DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(dollID);
                if (dInfo != null)
                {
                    leftText.text += "\r\n野巫靈: " + dInfo.dollName;
                }
            }
        }

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
