using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���T�O�s�ɪ��@�P�ʡA�H�U�C�|���Ȥ��n�������
public enum DOLL_BUFF_TARGET
{
    MYSELF      = 1,
    FRONT       = 2,
    MIDDLE      = 3,
    BACK        = 4,
    MASTER      = 5,
    ALL         = 10,
}

public enum DOLL_BUFF_TYPE
{
    DAMAGE          = 1,
    ATTACK_SPEED    = 2,
    MOVE_SPEED      = 3,
    HP              = 4,
}

//DollBuff
public class DollBuffBase
{
    public DOLL_BUFF_TYPE type;
    public DOLL_BUFF_TARGET target;
    public int value1;
    public string desc;
    public virtual void InitValue(DOLL_BUFF_TYPE _type, DOLL_BUFF_TARGET _target, int _value1) 
    {
        type = _type;
        target = _target;
        value1 = _value1;
    }

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

    static public DollBuffBase GenerateFromData(DollBuffData data)
    {
        DollBuffBase newBuff = null;
        switch ((DOLL_BUFF_TYPE)data.buffType)
        {
            case DOLL_BUFF_TYPE.DAMAGE:
                newBuff = new DollBuffDamage();
                break;
            case DOLL_BUFF_TYPE.HP:
                newBuff = new DollBuffHP();
                break;
            case DOLL_BUFF_TYPE.ATTACK_SPEED:
                newBuff = new DollBuffAttackSpeed();
                break;
            case DOLL_BUFF_TYPE.MOVE_SPEED:
                newBuff = new DollBuffMoveSpeed();
                break;
        }
        if (newBuff != null)
        {
            newBuff.InitValue((DOLL_BUFF_TYPE)data.buffType, (DOLL_BUFF_TARGET)data.buffTarget, data.buffValue1);
        }
        return newBuff;
    }
}


public class DollBuffDamage : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE _type, DOLL_BUFF_TARGET _target, int _value1)
    {
        base.InitValue(_type, _target, _value1);

        desc = GetTargetText(target) + "�ˮ` +" + value1 + "%";
    }
}

public class DollBuffAttackSpeed : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE _type, DOLL_BUFF_TARGET _target, int _value1)
    {
        base.InitValue(_type, _target, _value1);

        desc = GetTargetText(target) + "�����t�� +" + value1 + "%";
    }
}

public class DollBuffMoveSpeed : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE _type, DOLL_BUFF_TARGET _target, int _value1)
    {
        base.InitValue(_type, _target, _value1);

        desc = GetTargetText(target) + "���ʳt�� +" + value1 + "%";
    }
}

public class DollBuffHP : DollBuffBase
{
    public override void InitValue(DOLL_BUFF_TYPE type, DOLL_BUFF_TARGET target, int value1)
    {
        base.InitValue(type, target, value1);

        desc = GetTargetText(target) + "��q +" + value1 + "%";
    }
}



