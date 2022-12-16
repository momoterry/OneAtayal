using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletForward : bullet_base
{
    public bullet_base[] forwardBullets;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void InitValue(DAMAGE_GROUP g, float damage, Vector3 targetVec, GameObject targetObject = null)
    {
        foreach (bullet_base b in forwardBullets)
        {
            b.InitValue(g, damage, targetVec, targetObject);
        }
    }
}
