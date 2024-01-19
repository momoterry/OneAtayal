using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//目前都是以鍛造出巫靈的方式來實作，如果有其它的鍛造，需要另外改 Code

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
