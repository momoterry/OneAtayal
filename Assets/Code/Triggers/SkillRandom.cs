using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRandom : MonoBehaviour
{
    public SkillBase[] skillList;


    public int skillIndexStart = 2;

    public int skillIndexEnd = 3;

    void OnTG(GameObject whoTG)
    {
        int numToSet = skillIndexEnd - skillIndexStart + 1;
        PC_One thePC = BattleSystem.GetInstance().GetPlayer().GetComponent<PC_One>();
        if (thePC && numToSet <= skillList.Length)
        {
            OneUtility.Shuffle(skillList);
            for (int i = 0; i < numToSet; i++)
            {
                SkillBase skill = skillList[i];
                thePC.SetActiveSkill(skill, skillIndexStart + i);
                //print("SetActiveSkill " + (skillIndexStart + i) + " >> " + skill);
            }
        }

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }
}
