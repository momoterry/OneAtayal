using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHealLaser : BulletLaser
{
    protected override void DoOneDamage(GameObject targetO)
    {
        //base.DoOneDamage(targetO);
        print("試著補一下血 ....");
    }
}
