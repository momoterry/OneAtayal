using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 目前先實作成 Enemy 專用的 Buff 靈氣
// 等和 DollBuff 整合後，可以成為通用的 BuffAura

public class BuffAura : AreaEffectBase
{
    public BuffBase buffs;

    //protected FACTION_GROUP group = FACTION_GROUP.ENEMY;    //TODO: 之後也支援玩家方
}
