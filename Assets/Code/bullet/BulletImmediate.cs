using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletImmediate : bullet_base
{
    public GameObject fxRef;
    public GameObject castFxRef;
    public bool fxLinkTarget = true;
    public float randomDelayMax = 0;

    protected float waitTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (randomDelayMax > 0)
        {
            waitTime = Random.Range(0, randomDelayMax);
            print("Wait Time: " + waitTime);
        }
        else
            DoDamage();
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
                DoDamage();
        }
    }

    protected void DoDamage()
    {
        if (targetObj)
        {
            if (castFxRef && myDamage.Owner)
            {
                GameObject fxC = BattleSystem.SpawnGameObj(castFxRef, myDamage.Owner.transform.position);
                if (fxC)
                    fxC.transform.parent = myDamage.Owner.transform;
            }

            if (fxRef)
            {
                GameObject fxO = BattleSystem.SpawnGameObj(fxRef, targetObj.transform.position);
                if (fxLinkTarget && fxO)
                    fxO.transform.parent = targetObj.transform;
            }
            targetObj.SendMessage("OnDamage", myDamage);
        }
        Destroy(gameObject);
    }

}
