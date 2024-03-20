using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FACTION_GROUP
{
    NONE,
    PLAYER,
    ENEMY,
}

public enum DIRECTION
{
    U, D, L, R, NONE,
}

public enum ITEM_QUALITY
{
    COMMON = 0,
    RARE = 2,
    EPIC = 4,
}
public class GameDef
{
    public static Color GetQaulityColor( ITEM_QUALITY quality )
    {
        switch (quality)
        {
            case ITEM_QUALITY.RARE:
                return new Color(0.4f, 0.5f, 1.0f);
            case ITEM_QUALITY.EPIC:
                return new Color(1, 0.7f, 0.2f);
            default:
                return Color.white;
        }
    }
}


