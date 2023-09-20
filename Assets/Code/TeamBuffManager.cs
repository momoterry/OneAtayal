using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBuffManager : MonoBehaviour
{
    static public void AddTeamBuff(DollBuffBase buff)
    {
        List<Doll> dollList = BattleSystem.GetPC().GetDollManager().GetDolls();
        DOLL_POSITION_TYPE targetPositionType = GetTargetPositionType(buff.target);
        print("增加一個團隊 Buff " + buff.type);
        foreach (Doll doll in dollList)
        {
            if (buff.target == DOLL_BUFF_TARGET.ALL || targetPositionType == doll.positionType)
            {
                BuffApplierDoll buffApplier = doll.GetComponent<BuffApplierDoll>();
                if (buffApplier == null)
                {
                    buffApplier = doll.gameObject.AddComponent<BuffApplierDoll>();
                }
                print("團隊 Buff " + buff.type + "施加到: " + doll.name);
                buffApplier.ApplyBuff(buff);
            }
        }
    }

    static public void RemoveTeamBuff( DollBuffBase buff) 
    {
        List<Doll> dollList = BattleSystem.GetPC().GetDollManager().GetDolls();
        DOLL_POSITION_TYPE targetPositionType = GetTargetPositionType(buff.target);
        foreach (Doll doll in dollList)
        {
            if (buff.target == DOLL_BUFF_TARGET.ALL || targetPositionType == doll.positionType)
            {
                BuffApplierDoll buffApplier = doll.GetComponent<BuffApplierDoll>();
                if (buffApplier == null)
                {
                    print("ERROR: remove a buff form a doll without BuffApplierDoll !! ");
                    continue;
                }
                buffApplier.DeApplyBuff(buff);
            }
        }
    }

    static protected DOLL_POSITION_TYPE GetTargetPositionType( DOLL_BUFF_TARGET target)
    {
        switch (target)
        {
            case DOLL_BUFF_TARGET.FRONT:
                return DOLL_POSITION_TYPE.FRONT;
            case DOLL_BUFF_TARGET.MIDDLE:
                return DOLL_POSITION_TYPE.MIDDLE;
            case DOLL_BUFF_TARGET.BACK:
                return DOLL_POSITION_TYPE.BACK;
        }
        return DOLL_POSITION_TYPE.BACK;
    }

}
