using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStep : bullet_base
{
    public GameObject Step_Object;
    public int Step_Count = 4;
    public float Init_Distance = 0.5f;
    public float Step_Distance = 0.5f;
    public float Wait_Time = 0;
    public float Step_Time = 0.125f;

    protected float timeToStep = 0;
    protected int currStep = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeToStep = Wait_Time;
    }

    // Update is called once per frame
    void Update()
    {
        timeToStep -= Time.deltaTime;
        if (timeToStep <= 0)
        {
            DoOneStep();
            timeToStep = Step_Time;
            currStep++;
            if (currStep == Step_Count)
            {
                DoStepDone();
            }
        }
    }

    void DoOneStep()
    {
        if (Step_Object)
        {
            Vector3 objPoint = transform.position + targetDir * (Step_Distance * (float)currStep + Init_Distance);
#if XZ_PLAN
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rm = Quaternion.identity;
#endif
            GameObject newObj = Instantiate(Step_Object, objPoint, rm, null);
            if (newObj)
            {
                //DamageByAnimation d = newObj.GetComponent<DamageByAnimation>();
                //if (d)
                //{
                //    d.SetGroup(group);
                //    d.baseDamage = baseDamage;
                //}
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {

                    newBullet.InitValue(group, baseDamage, targetDir);
                }
            }
        }
    }

    void DoStepDone()
    {
        Destroy(gameObject);
    }
}
