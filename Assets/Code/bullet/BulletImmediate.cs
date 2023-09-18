using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImmediate : bullet_base
{
    public GameObject fxRef;
    // Start is called before the first frame update
    void Start()
    {
        if (targetObj)
        {
            if (fxRef)
                BattleSystem.SpawnGameObj(fxRef, targetObj.transform.position);
            targetObj.SendMessage("OnDamage", myDamage);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
