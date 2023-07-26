using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DollSkillManager : MonoBehaviour
{
    const int MAX_DOLL_SKILL = 3;

    protected class SkillMapInfo
    {
        public string ID;
        public Sprite icon;
        public int order;
        public List<DollSkillBase> list;
        public bool active;
    }

    //protected List<DollSkillBase> skillList;
    protected List<SkillMapInfo> skillInfoList = new List<SkillMapInfo>();
    //protected unityA
    //protected delegate void SkillButtonCB();
    protected UnityAction[] skillCBs = new UnityAction[MAX_DOLL_SKILL];


    void Awake()
    {
        skillCBs[0] = OnSkillButtonOne;
        skillCBs[1] = OnSkillButtonTwo;
        skillCBs[2] = OnSkillButtonThree;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RegisterDollSkill( DollSkillBase dSkill)
    {
        //print("----Register DollSkill: " + dSkill.ID + " num = ");
        bool newSkill = true;
        foreach (SkillMapInfo info in skillInfoList)
        {
            if (info.ID == dSkill.ID)
            {
                newSkill = false;
                info.list.Add(dSkill);
                break;
            }
        }
        if (newSkill)
        {
            SkillMapInfo newInfo = new SkillMapInfo();
            newInfo.ID = dSkill.ID;
            newInfo.icon = dSkill.icon;
            newInfo.order = dSkill.order;
            newInfo.list = new List<DollSkillBase>();
            newInfo.list.Add(dSkill);
            newInfo.active = false;
            skillInfoList.Add(newInfo);
            SetupDollSkillButtons();
        }
    }

    public void UnRegisterDollSkill(DollSkillBase dSkill)
    {
        //print("----UnRegister DollSkill: " + dSkill.ID + " num = ");
        SkillMapInfo deleteInfo = null;
        foreach (SkillMapInfo info in skillInfoList)
        {
            if (info.ID == dSkill.ID)
            {
                info.list.Remove(dSkill);
                if (info.list.Count == 0)
                {
                    //print("Doll Skill Empty !! " + info.ID);
                    deleteInfo = info;
                }
                break;
            }
        }
        if (deleteInfo != null)
        {
            skillInfoList.Remove(deleteInfo);
            SetupDollSkillButtons();
        }
    }

    public void OnSkillButtonOne()
    {
        print("----One----");
        OnSkillButton(0);
    }

    public void OnSkillButtonTwo()
    {
        print("----Two----");
        OnSkillButton(1);
    }

    public void OnSkillButtonThree()
    {
        print("----Three----");
        OnSkillButton(2);
    }

    protected void OnSkillButton(int index)
    {
        print("OnSkillButton " + index);
        if (index >= skillInfoList.Count)
        {
            print("ERROR !! OnSkillButton index = " + index);
            return;
        }

        SkillMapInfo info = skillInfoList[index];
        info.active = !info.active;
        foreach ( DollSkillBase skill in info.list)
        {
            skill.OnStartSkill(info.active);
        }
    }

    protected class SkillMapComparer : IComparer<SkillMapInfo>
    {
        public int Compare(SkillMapInfo x, SkillMapInfo y)
        {
            return x.order - y.order;
        }
    }

    protected void SetupDollSkillButtons()
    {
        skillInfoList.Sort(new SkillMapComparer());

        for (int i=0; i<MAX_DOLL_SKILL; i++)
        {
            DollSkillButton button = BattleSystem.GetHUD().GetDollSkillButton(i);
            if (button)
            {
                //print("SetupDollSkillButtons:" + i + "-" + skillInfoList.Count);
                if (i < skillInfoList.Count)
                {
                    button.gameObject.SetActive(true);
                    button.SetIcon(skillInfoList[i].icon);
                    button.UnBind();
                    button.Bind(skillCBs[i]);
                }
                else
                {
                    button.UnBind();
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

}
