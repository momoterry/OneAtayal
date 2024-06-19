using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleUtility : MonoBehaviour
{
    public const float HITTABLE_OBJ_DISTANCE = 4.0f;

    static public GameObject SearchClosestTargetForPlayer(Vector3 myCenter, float distance, string sTag = "Enemy")
    {
        Collider[] cols = Physics.OverlapSphere(myCenter, distance, LayerMask.GetMask("Character"));
        if (cols.Length == 0)
            return null;

        GameObject bestObj = null;
        float bestSDis = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag(sTag))
            {
                Vector3 vDis = col.transform.position - myCenter;
                float sDis = vDis.magnitude;
                if (sDis < bestSDis)
                {
                    //HitBody hBody = col.gameObject.GetComponent<HitBody>();
                    //if (hBody && hBody.GetRangeLimint() < sDis)
                    //{
                    //    continue;
                    //}
                    bestObj = col.gameObject;
                    bestSDis = sDis;
                }
            }
        }

        //if (bestObj == null)
        //{
        //    bestSDis = HITTABLE_OBJ_DISTANCE;
        //    foreach (Collider col in cols)
        //    {
        //        if (col.gameObject.CompareTag("Hittable"))
        //        {
        //            Vector3 vDis = col.transform.position - myCenter;
        //            float sDis = vDis.magnitude;
        //            //print("Hittable!!" + sDis);
        //            if (sDis < bestSDis)
        //            {
        //                bestObj = col.gameObject;
        //                bestSDis = sDis;
        //            }
        //        }
        //    }
        //}

        return bestObj;
    }

    static public GameObject SearchClosestTargetForEnemy(Vector3 myCenter, float distance)
    {
        Collider[] cols = Physics.OverlapSphere(myCenter, distance, LayerMask.GetMask("Character"));
        if (cols.Length == 0)
            return null;

        GameObject bestObj = BattleSystem.GetPC().gameObject;
        float bestSDis = (bestObj.transform.position - myCenter).magnitude;
        if (bestSDis > distance)
        {
            bestObj = null;
            bestSDis = Mathf.Infinity;
        }
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Doll"))
            {
                Vector3 vDis = col.transform.position - myCenter;
                float sDis = vDis.magnitude;
                if (sDis < bestSDis)
                {
                    //HitBody hBody = col.gameObject.GetComponent<HitBody>();
                    //if (hBody && hBody.GetRangeLimint() < sDis)
                    //{
                    //    continue;
                    //}
                    bestObj = col.gameObject;
                    bestSDis = sDis;
                }
            }
        }

        return bestObj;
    }
}
