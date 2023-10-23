using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUFF_TYPE
{
    DAMAGE = 1,
    ATTACK_SPEED = 2,
    MOVE_SPEED = 3,
    HP = 4,
}

[System.Serializable]
public class BuffBase
{
    public FACTION_GROUP group;
    public BUFF_TYPE type;

    public float value;
}
