using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSkillOne : DollSkillBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartSkill(bool active = true)
    {
        base.OnStartSkill(active);

        DollAuto doll = GetComponent<DollAuto>();
        if (doll)
        {
            if (active)
            {
                doll.StartHoldPosition(doll.transform.position);
            }
            else
            {
                doll.StopHoldPosition();
            }
        }

        print("DollSkillOne::OnStartSkill " + active);
    }

}
