using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrace : bullet_base
{
    public float speed = 5.0f;
    public float turnSpeed = 180.0f;
    public float loseAngle = 90.0f;
    public float lifeTime = 2.0f;
    public float hitDistance = 0.25f;

    public GameObject hitFX;

    protected float myTime = 0.0f;
    protected bool loseTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        myTime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += targetDir * speed * Time.deltaTime;

        if (myTime < 0.0f)
        {
            Destroy(gameObject);
            return;
        }
        myTime -= Time.deltaTime;

        //Check if hit or need to turn
        if (targetObj && !loseTarget)
        {
            Vector3 targetObjVec = targetObj.transform.position - transform.position;
            float dis = targetObjVec.magnitude;
            if (dis <= hitDistance)
            {
                DoHitTarget();
                return;
            }

            //Turn to Target
            Vector3 targetObjDir = targetObjVec.normalized;
            float targetObjAngle = Vector3.Angle(targetDir, targetObjVec);
            float turnStep = turnSpeed * Time.deltaTime;
            if (targetObjAngle < turnStep)
            {
                targetDir = targetObjDir;
            }
            else if (targetObjAngle < loseAngle)
            {
                targetDir = Vector3.RotateTowards(targetDir, targetObjDir, turnStep * Mathf.Deg2Rad, 0);
            }
            else
            {
                loseTarget = true;
            }
        }
        //print("Trace..." + targetObj + "  dir  " + targetDir);
    }

    virtual protected void DoHitTarget()
    {

        Destroy(gameObject);

        if (hitFX)
        {
            BattleSystem.GetInstance().SpawnGameplayObject(hitFX, transform.position, false);
        }

        Damage myDamage;
        myDamage.damage = baseDamage;
        if (targetObj)
            targetObj.SendMessage("OnDamage", myDamage);
    }
}
