using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAdd : MonoBehaviour
{
    public SkillBase skillOneToAdd;
    public bool forceAddIfFull = false;     //�p�G�� true�A�j���Ĥ@�ӧޯ�M��

    void OnTG(GameObject whoTG)
    {
        const int maxSkill = 4;

        PC_One thePC = BattleSystem.GetInstance().GetPlayer().GetComponent<PC_One>();
        if (thePC == null)
        {
            One.LOG("ERROR!! SkillAdd only supoort PC_One!!!!");
            return;
        }

        if (skillOneToAdd)
        {
            int foundIndex = -1;
            for (int i = 1; i < maxSkill; i++)
            {
                if (thePC.GetActiveSkill(i) == null)
                {
                    print("���Ū����: " + i);
                    thePC.SetActiveSkill(skillOneToAdd, i);
                    foundIndex = i;
                    break;
                }
            }
            if (foundIndex < 0 && forceAddIfFull)
            {
                //�S���Ū��ޯ���A�j���M���ޯ� I
                thePC.SetActiveSkill(skillOneToAdd, 1);
            }
        }
        else
        {
            for (int i = 1; i < maxSkill; i++)
            {
                thePC.SetActiveSkill(null, i);
            }
        }

        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: ��� Trigger ���覡�^��

    }
}
