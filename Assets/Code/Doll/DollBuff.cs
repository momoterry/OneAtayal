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
    public string desc;
}



