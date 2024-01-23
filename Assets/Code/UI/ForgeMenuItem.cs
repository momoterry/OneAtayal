using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//目前都是以鍛造出巫靈的方式來實作，如果有其它的鍛造，需要另外改 Code

public class ForgeMenuItem : MonoBehaviour
{
    public Image resultIcon;
    public Text resultText;
    public Text costText;
    public GameObject matItemRef;
    public Transform matMenuRoot;

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
        costText.text = formula.requireMoney.ToString();
        int iHasMoney = GameSystem.GetPlayerData().GetMoney();
        if (iHasMoney < formula.requireMoney)
        {
            costText.color = Color.red;
        }

        RectTransform refRT = matItemRef.GetComponent<RectTransform>();
        Vector2 pos = refRT.anchoredPosition;
        float itemStep = refRT.sizeDelta.y;
        //print("itemHeight = " + refRT.sizeDelta.y);
        for (int i=0; i<formula.inputs.Length; i++)
        {
            GameObject o = Instantiate(matItemRef, matMenuRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;

            ForgeMaterialItem item = o.GetComponent<ForgeMaterialItem>();
            item.InitValue(formula.inputs[i]);

            o.SetActive(true);
            pos.y -= itemStep;
        }
        matItemRef.SetActive(false);
    }
}
