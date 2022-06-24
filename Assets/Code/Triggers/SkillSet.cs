using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSet : MonoBehaviour
{
    public SkillBase autoSkillToSet;

    public SkillBase skillOneToSet;
    public SkillBase skillTwoToSet;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    void OnTG(GameObject whoTG)
    {
        //print("Do it !!");

        PC_One thePC = BattleSystem.GetInstance().GetPlayer().GetComponent<PC_One>();
        if (thePC==null)
        {
            print("ERROR!! SkillSet only supoort PC_One!!!!");
            return;
        }

        if (autoSkillToSet)
        {
            thePC.SetAutoSkill(autoSkillToSet);
        }

        if (skillOneToSet)
        {
            thePC.SetActiveSkill(skillOneToSet, 0);
        }

        if (skillTwoToSet)
        {
            thePC.SetActiveSkill(skillTwoToSet, 1);
        }

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應

    }
}
