using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DOLL_BUFF_TARGET
{
    MYSELF,
    FRONT,
    MIDDLE,
    BACK,
    MASTER,
    ALL,
}

public enum DOLL_BUFF_TYPE
{
    DAMAGE,
    ATTACK_SPEED,
    MOVE_SPEED,
    HP,
}

//DollBuff
public class DollBuffBase
{
    public DOLL_BUFF_TYPE type;
    public DOLL_BUFF_TARGET target;
    public float value1;
    public string desc;
    public virtual void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1) { }

    protected string GetTargetText(DOLL_BUFF_TARGET target)
    {
        switch (target)
        {
            case DOLL_BUFF_TARGET.MYSELF:
                return "自身";
            case DOLL_BUFF_TARGET.FRONT:
                return "前排";
            case DOLL_BUFF_TARGET.MIDDLE:
                return "中排";
            case DOLL_BUFF_TARGET.BACK:
                return "後排";
            case DOLL_BUFF_TARGET.ALL:
                return "全體";
            case DOLL_BUFF_TARGET.MASTER:
                return "主角";

        }
        return "";
    }
}


public class DollBuffDamage : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "傷害 +" + value1 + "%";
    }
}

public class DollBuffAttackSpeed : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "攻擊速度 +" + value1 + "%";
    }
}

public class DollBuffMoveSpeed : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "移動速度 +" + value1 + "%";
    }
}

public class DollBuffHP : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "血量 +" + value1 + "%";
    }
}



