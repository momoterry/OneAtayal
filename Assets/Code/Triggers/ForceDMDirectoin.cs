using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDMDirectoin : MonoBehaviour
{
    public float angle = 0;
    void OnTG(GameObject whoTG)
    {
        DollManager dm = BattleSystem.GetPC().GetDollManager();
        if (dm)
        {
            dm.ForceSetDirection(angle);
        }
    }
}
