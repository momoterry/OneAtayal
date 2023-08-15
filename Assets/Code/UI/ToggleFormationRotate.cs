using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFormationRotate : MonoBehaviour
{
    public Image toggleIcon;

    public Sprite onSprite;
    public Sprite offSprite;

    public void OnToggleFormationRotation()
    {
        if (!BattleSystem.GetPC() || !BattleSystem.GetPC().GetDollManager())
            return;

        DollManager dm = BattleSystem.GetPC().GetDollManager();
        if (dm.FixDirection)
        {
            dm.FixDirection = false;
            toggleIcon.sprite = onSprite;
        }
        else
        {
            dm.FixDirection = true;
            toggleIcon.sprite = offSprite;
        }
    }
}
