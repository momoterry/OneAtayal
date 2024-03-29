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
    UNCOMMON = 1,
    RARE = 2,
    EPIC = 3,
    UNIQUE = 4,
    LEGENDARY = 5,
}
public class GameDef
{
    public static Color GetQaulityColor( ITEM_QUALITY quality )
    {
        switch (quality)
        {
            case ITEM_QUALITY.UNCOMMON:
                return new Color(0.2f, 0.95f, 0.3f); //ºñ
            case ITEM_QUALITY.RARE:
                return new Color(0.4f, 0.5f, 1.0f); //ÂÅ
            case ITEM_QUALITY.EPIC:
                return new Color(0.9f, 0.2f, 1.0f); //µµ
            case ITEM_QUALITY.UNIQUE:
                return new Color(1.0f, 0.7f, 0.2f); //·tª÷
            case ITEM_QUALITY.LEGENDARY:
                return new Color(1.0f, 0.1f, 0.1f); //¬õ

            default:
                return Color.white;
        }
    }
}


