using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveTrigger : MonoBehaviour
{
    public void OnTG(GameObject whoTG)
    {
        PC_One pc = (PC_One)BattleSystem.GetPC();

        if (pc)
        {
            print("有 PC，試著復活全部 !!");
            pc.ReviveAllDoll();
        }
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }
}
