using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLaser : bullet_base
{
    public GameObject theBeam;
    public float attackPeriod = 0.2f;

    protected float startShift = 0.0f;

    protected float timeToAttack = 0;

    //protected Vector3 debugFrom;
    //protected Vector3 debugTo;

    public override void InitValue(DAMAGE_GROUP g, Damage theDamage, Vector3 targetVec, GameObject targetObject = null)
    {
        base.InitValue(g, theDamage, targetVec, targetObject);

        baseDamage = baseDamage * timeToAttack;
        theDamage.damage = baseDamage;
    }

    protected void DoOneDamage()
    {
        targetObj.SendMessage("OnDamage", myDamage);
    }

    public void UpdateLaser( GameObject targetObj, Vector3 fromPos )
    {
        Vector3 targetPos = targetObj.transform.position;
        targetPos.y = 0;
        fromPos.y = 0;
        Vector3 targetVec = (targetPos - fromPos).normalized;

        fromPos += targetVec * startShift;
        Vector3 vec = targetPos - fromPos;

        theBeam.transform.position = (targetPos + fromPos) * 0.5f;
        theBeam.transform.localScale = new Vector3(1.0f, vec.magnitude, 1.0f);
        theBeam.transform.rotation = Quaternion.Euler(90.0f, Vector3.SignedAngle(Vector3.back, targetVec, Vector3.up), 0);

        timeToAttack -= Time.deltaTime;
        if (timeToAttack <= 0)
        {
            DoOneDamage();
            timeToAttack += attackPeriod;
        }

        //debugFrom = fromPos;
        //debugTo = targetObj.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (targetObj)
        //{

        //}
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(debugFrom, debugTo);
    //}

}
