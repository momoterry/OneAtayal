using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSkillManager : MonoBehaviour
{
    const int MAX_DOLL_SKILL = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int testNum = 0;

    public void RegisterDollSkill( DollSkillBase dSkill)
    {
        testNum++;
        print("----Register DollSkill: " + dSkill.ID + " num = " + testNum);
    }

    public void UnRegisterDollSkill(DollSkillBase dSkill)
    {
        testNum--;
        print("----UnRegister DollSkill: " + dSkill.ID + " num = " + testNum);
    }
}
