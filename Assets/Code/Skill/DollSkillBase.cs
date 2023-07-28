using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollSkillBase : MonoBehaviour
{
    public string ID = "";
    public int order;
    public Sprite icon;
    public GameObject activeHint;

    protected DollAuto doll;
    protected Damage myDamage;
    protected bool isActive = false;


    private void Awake()
    {
        doll = GetComponent<DollAuto>();
        myDamage.Init(0, Damage.OwnerType.PLAYER, gameObject.name, gameObject);
        if (activeHint)
            activeHint.SetActive(false);
    }

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

    virtual public void OnStartSkill(bool active = true) { 
        isActive = active;
        if (activeHint)
            activeHint.SetActive(active);
    }

}
