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
                return "�ۨ�";
            case DOLL_BUFF_TARGET.FRONT:
                return "�e��";
            case DOLL_BUFF_TARGET.MIDDLE:
                return "����";
            case DOLL_BUFF_TARGET.BACK:
                return "���";
            case DOLL_BUFF_TARGET.ALL:
                return "����";
            case DOLL_BUFF_TARGET.MASTER:
                return "�D��";

        }
        return "";
    }
}


public class DollBuffDamage : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "�ˮ` +" + value1 + "%";
    }
}

public class DollBuffAttackSpeed : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "�����t�� +" + value1 + "%";
    }
}

public class DollBuffMoveSpeed : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "���ʳt�� +" + value1 + "%";
    }
}

public class DollBuffHP : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, float value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "��q +" + value1 + "%";
    }
}



