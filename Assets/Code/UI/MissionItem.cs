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

        leftText.text = "���ȥؼ�:";
        leftText.text += "\r\n�W��:  " + MissionManager.GetScaleText(data.scale);
        leftText.text += "\r\n�a�I:  " + data.sceneText;

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
                    leftText.text += "\r\n�����F: " + dInfo.dollName;
                }
            }
        }

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
