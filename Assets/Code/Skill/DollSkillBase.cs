using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSkillBase : MonoBehaviour
{
    public string ID = "";
    public Sprite icon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnJoinPlayer()
    {
        //DollSkillManager dsm = BattleSystem.GetPC().GetDollManager().GetDollSkillManager();
        DollSkillManager dsm = BattleSystem.GetDollSkillManager();
        if (dsm)
        {
            dsm.RegisterDollSkill(this);
        }
    }

    public void OnLeavePlayer() 
    {
        //DollSkillManager dsm = BattleSystem.GetPC().GetDollManager().GetDollSkillManager();
        DollSkillManager dsm = BattleSystem.GetDollSkillManager();
        if (dsm)
        {
            dsm.UnRegisterDollSkill(this);
        }
    }

}
