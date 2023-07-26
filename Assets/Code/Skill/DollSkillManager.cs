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
        public List<DollSkillBase> list;
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

    int testNum = 0;

    public void RegisterDollSkill( DollSkillBase dSkill)
    {
        testNum++;
        print("----Register DollSkill: " + dSkill.ID + " num = " + testNum);
        //skillList.Add(dSkill);
        bool newSkill = true;
        foreach (SkillMapInfo info in skillInfoList)
        {
            if (info.ID == dSkill.ID)
            {
                newSkill = false;
                info.list.Add(dSkill);
            }
        }
        if (newSkill)
        {
            SkillMapInfo newInfo = new SkillMapInfo();
            newInfo.ID = dSkill.ID;
            newInfo.icon = dSkill.icon;
            newInfo.list = new List<DollSkillBase>();
            newInfo.list.Add(dSkill);
            skillInfoList.Add(newInfo);
            SetupDollSkillButtons();
        }
    }

    public void UnRegisterDollSkill(DollSkillBase dSkill)
    {
        testNum--;
        print("----UnRegister DollSkill: " + dSkill.ID + " num = " + testNum);
        //skillList.Remove(dSkill);
    }

    public void OnSkillButtonOne()
    {
        print("----One----");
    }

    public void OnSkillButtonTwo()
    {
        print("----Two----");
    }

    public void OnSkillButtonThree()
    {
        print("----Three----");
    }

    protected void SetupDollSkillButtons()
    {
        for (int i=0; i<MAX_DOLL_SKILL; i++)
        {
            DollSkillButton button = BattleSystem.GetHUD().GetDollSkillButton(i);
            if (button)
            {
                print("SetupDollSkillButtons:" + i + "-" + skillInfoList.Count);
                if (i < skillInfoList.Count)
                {
                    button.gameObject.SetActive(true);
                    button.SetIcon(skillInfoList[i].icon);
                    button.Bind(skillCBs[i]);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

}
