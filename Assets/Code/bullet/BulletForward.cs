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

    public override void InitValue(FACTION_GROUP g, Damage _damage, Vector3 targetVec, GameObject targetObject = null)
    {
        foreach (bullet_base b in forwardBullets)
        {
            b.InitValue(g, _damage, targetVec, targetObject);
        }
    }
}
