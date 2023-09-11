using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBlazer : bullet_base
{
    public GameObject Step_Object;

    public float timeTotal = 2.0f;
    public float Step_Distance = 1.0f;

    protected Vector3 prevStepPos;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = myDamage.Owner.transform;    //Attack ¤W¥h
        DoOneStep();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, prevStepPos) >= Step_Distance)
        {
            DoOneStep();
        }
        timeTotal -= Time.deltaTime;
        if (timeTotal < 0.0f)
        {
            Destroy(gameObject);
        }
    }

    void DoOneStep()
    {
        Vector3 objPoint = transform.position;
        if (Step_Object)
        {
            GameObject newObj = BattleSystem.SpawnGameObj(Step_Object, objPoint);
            if (newObj)
            {
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {
                    newBullet.InitValue(group, myDamage, targetDir);
                    //newBullet.SetResultCB(OnBulletResult);
                }
            }
        }
        prevStepPos = objPoint;
    }
}
