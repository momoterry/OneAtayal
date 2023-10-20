using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDir : bullet
{
    public GameObject dirTarget; 

    public override void InitValue(FACTION_GROUP g, Damage _damage, Vector3 targetVec, GameObject targetObject = null)
    {
        base.InitValue(g, _damage, targetVec, targetObject);
        if (!dirTarget)
        {
            dirTarget = gameObject;
        }

        dirTarget.transform.rotation = Quaternion.Euler(90.0f, Vector3.SignedAngle(Vector3.back, targetVec, Vector3.up), 0);
    }
}
