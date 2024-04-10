using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatItem : MonoBehaviour
{
    public Slider slider;
    public Text numText;
    public Image dollIcon;
    public void InitValue( BattleStatMenu.ItemData data, float maxDatamge, float totalDamage)
    {
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(data.dollID);
        if (dInfo != null)
        {
            dollIcon.sprite = dInfo.icon;
        }
        numText.text = (Mathf.RoundToInt(data.totalDamage)).ToString();
        slider.SetValueWithoutNotify(data.totalDamage / maxDatamge);

        if (data.isMax)
        {
            numText.color = Color.yellow;
            Image im = slider.fillRect.GetComponent<Image>();
            im.color = Color.yellow;
        }
    }
}
