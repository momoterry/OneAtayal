using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveArea : AreaEffectBase
{

    protected override bool CheckGameObject(GameObject obj)
    {
        if (obj.CompareTag("Player"))
            return true;

        return false;
    }
    protected override void ApplyEffect(GameObject obj)
    {
        PC_One pc = obj.GetComponent<PC_One>();

        if (pc)
        {
            pc.ReviveOneDoll();
        }
    }
}
