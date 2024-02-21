using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUtility : MonoBehaviour
{
    static public GameObject SearchClosestTargetForPlayer(Vector3 myCenter, float distance)
    {
        Collider[] cols = Physics.OverlapSphere(myCenter, distance, LayerMask.GetMask("Character"));
        if (cols.Length == 0)
            return null;

        GameObject bestObj = null;
        float bestSDis = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                Vector3 vDis = col.transform.position - myCenter;
                float sDis = vDis.magnitude;
                if (sDis < bestSDis)
                {
                    HitBody hBody = col.gameObject.GetComponent<HitBody>();
                    if (hBody && hBody.GetRangeLimint() < sDis)
                    {
                        continue;
                    }
                    bestObj = col.gameObject;
                    bestSDis = sDis;
                }
            }
        }

        //if (bestObj == null)
        //{
        //    bestSDis2 = HITTABLE_OBJ_DISTANCE * HITTABLE_OBJ_DISTANCE;
        //    foreach (Collider col in cols)
        //    {
        //        if (col.gameObject.CompareTag("Hittable"))
        //        {
        //            Vector3 vDis = col.transform.position - myCenter;
        //            float sDis = vDis.sqrMagnitude;
        //            print("Hittable!!" + sDis);
        //            if (sDis < bestSDis2)
        //            {
        //                bestObj = col.gameObject;
        //                bestSDis2 = sDis;
        //            }
        //        }
        //    }
        //}

        return bestObj;
    }
}
