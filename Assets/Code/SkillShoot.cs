using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShoot : SkillBase
{
    public GameObject bulletRef;
    public float bulletInitDis;
    // Start is called before the first frame update

    public override void DoStart()
    {
        print("SkillShoot!!!!!! " + transform.position);
    }
}
