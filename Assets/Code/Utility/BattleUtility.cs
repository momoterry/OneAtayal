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

        return bestObj;
    }


    static public GameObject SearchBestTargetForPlayer(Vector3 searchCenter, Vector3 rangeCenter, float distance, string sTag = "Enemy")
    {
        Collider[] cols = Physics.OverlapSphere(rangeCenter, distance, LayerMask.GetMask("Character"));
        if (cols.Length == 0)
            return null;

        GameObject bestObj = null;
        float bestSDis = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag(sTag))
            {
                Vector3 vDis = col.transform.position - searchCenter;
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

    static Vector3 compareCenter;
    static int ComparerDistance(GameObject A, GameObject B)
    {
        if (Vector3.Distance(A.transform.position, compareCenter) < Vector3.Distance(B.transform.position, compareCenter))
            return -1;
        else
            return 1;
    }

    static List<GameObject> sortList = new List<GameObject>();
    static public GameObject SearchBestTargetsForPlayer(Vector3 searchCenter, Vector3 rangeCenter, float distance, int randomBestNum, string sTag = "Enemy")
    {
        Collider[] cols = Physics.OverlapSphere(rangeCenter, distance, LayerMask.GetMask("Character"));
        if (cols.Length == 0)
            return null;

        foreach (Collider c in cols)
        {
            if (c.gameObject.CompareTag(sTag))
                sortList.Add(c.gameObject);
        }
        compareCenter = searchCenter;

        sortList.Sort(ComparerDistance);

        int iChoose = Random.Range(0, randomBestNum <= sortList.Count ? randomBestNum : sortList.Count );
        GameObject bestObj = sortList[iChoose];
        sortList.Clear();
        return bestObj;
    }


    static public List<GameObject> SearchAllTargets(Vector3 myCenter, float distance, string sTag = "Enemy")
    {
        List<GameObject> targets = new List<GameObject>();
        Collider[] cols = Physics.OverlapSphere(myCenter, distance, LayerMask.GetMask("Character"));
        if (cols.Length == 0)
            return targets;

        //GameObject bestObj = null;
        //float bestSDis = Mathf.Infinity;
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag(sTag))
            {
                //    Vector3 vDis = col.transform.position - myCenter;
                //    float sDis = vDis.magnitude;
                //    if (sDis < bestSDis)
                //    {
                //        bestObj = col.gameObject;
                //        bestSDis = sDis;
                //        targets.Insert(0, col.gameObject);  //確保最近的目標放在第一個
                //    }
                //    else
                //        targets.Add(col.gameObject);
                //}
                targets.Add(col.gameObject);
            }
        }
        return targets;
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
