using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�ثe���O�H��y�X���F���覡�ӹ�@�A�p�G���䥦����y�A�ݭn�t�~�� Code

public class ForgeMenuItem : MonoBehaviour
{
    public Image resultIcon;
    public Text resultText;
    public GameObject matItemRef;

    public void InitValue(ForgeFormula formula)
    {
        string dollID = formula.outputID;
        DollInfo dInfo = GameSystem.GetInstance().theDollData.GetDollInfoByID(dollID);
        if (dInfo == null)
        {
            print("ERROR!! No such DollRef for ID:" + dollID);
            return;
        }
        Doll doll = dInfo.objRef.GetComponent<Doll>();
        resultIcon.sprite = doll.icon;
        resultText.text = dInfo.dollName;
    }
}
