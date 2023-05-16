using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLaser : bullet_base
{
    public GameObject theBeam;
    public float startShift = 0.5f;

    public void UpdateLaser( GameObject targetObj, Vector3 fromPos )
    {
        Vector3 targetPos = targetObj.transform.position;
        Vector3 targetVec = (targetPos - fromPos).normalized;

        fromPos += targetVec * startShift;
        Vector3 vec = targetPos - fromPos;

        theBeam.transform.position = (targetPos + fromPos) * 0.5f;
        theBeam.transform.localScale = new Vector3(1.0f, vec.magnitude, 1.0f);
        theBeam.transform.rotation = Quaternion.Euler(90.0f, Vector3.SignedAngle(Vector3.back, targetVec, Vector3.up), 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
