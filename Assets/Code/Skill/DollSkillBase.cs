using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSkillBase : MonoBehaviour
{
    public string ID = "";
    public int order;
    public Sprite icon;
    //public GameObject activeHint;

    public GameObject[] enableObjs;
    public GameObject[] disableObjs;

    //protected DollAuto doll;
    protected Doll doll;
    protected Damage myDamage;
    protected bool isActive = false;


    private void Awake()
    {
        //print("DollSkillBase.Awake");
        doll = GetComponent<Doll>();
        myDamage.Init(0, Damage.OwnerType.DOLL, doll.ID, doll.gameObject);
        //if (activeHint)
        //    activeHint.SetActive(false);
    }


    public void OnJoinPlayer()
    {
        //DollSkillManager dsm = BattleSystem.GetPC().GetDollManager().GetDollSkillManager();
        DollSkillManager dsm = BattleSystem.GetDollSkillManager();
        if (enabled && dsm)
        {
            dsm.RegisterDollSkill(this);
        }
    }

    public void OnLeavePlayer() 
    {
        //DollSkillManager dsm = BattleSystem.GetPC().GetDollManager().GetDollSkillManager();
        DollSkillManager dsm = BattleSystem.GetDollSkillManager();
        if (enabled && dsm)
        {
            dsm.UnRegisterDollSkill(this);
        }
    }

    virtual public void OnStartSkill(bool active = true) { 
        isActive = active;
        //if (activeHint)
        //    activeHint.SetActive(active);
        foreach (GameObject o in enableObjs)
        {
            o.SetActive(active);
        }
        foreach (GameObject o in disableObjs)
        {
            o.SetActive(!active);
        }

        if (active)
        {
            doll.StartDollSkill();
        }
        else
        {
            doll.StopDollSkill();
        }
    }

}
