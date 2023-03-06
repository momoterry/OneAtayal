using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class TileGroup
{
    public Tile baseTile;
    public Tile[] decorateTiles;
    public float decorateRate;
    public Tile GetOneTile()
    {
        if (decorateTiles.Length > 0 && decorateRate > 0)
        {
            float rd = Random.Range(0, 1.0f);
            if (rd < decorateRate)
            {
                return decorateTiles[Random.Range(0, decorateTiles.Length)];
            }
        }
        return baseTile;
    }
}

public enum MAP_EDGE_TYPE
{
    NOT,// = 0,
    UU,// = 11,
    DD,// = 12,
    LL,// = 13,
    RR,// = 14,
    LU,// = 21,
    RU,// = 22,
    LD,// = 23,
    RD,// = 24,
    LU_S,// = 31,
    RU_S,// = 32,
    LD_S,// = 33,
    RD_S,// = 34,

    DD2,
    LD2,
    RD2,
    LD_S2,
    RD_S2,    
}

[System.Serializable]
public class TileEdgeGroup
{
    //上、下、左、右
    public Tile UU;
    public Tile DD;
    public Tile LL;
    public Tile RR;
    //左上、右上、左下、右下
    public Tile LU;
    public Tile RU;
    public Tile LD;
    public Tile RD;
    //左上陷、右上陷、左下陷、右下陷
    public Tile LU_S;
    public Tile RU_S;
    public Tile LD_S;
    public Tile RD_S;


    protected bool IsOut(int value, int inValue, int outValue)
    {
        return value != inValue && (outValue == OneMap.INVALID_VALUE || value == outValue);
    }

    //確認這個點是否是外部邊界，必須檢查週邊是否為 in
    virtual public int GetOutEdgeType(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        bool bUU = theMap.GetValue(x, y + 1) == inValue;
        bool bDD = theMap.GetValue(x, y - 1) == inValue;
        bool bLL = theMap.GetValue(x - 1, y) == inValue;
        bool bRR = theMap.GetValue(x + 1, y) == inValue;

        if (bUU)
        {
            if (bLL)
            {
                return (int)MAP_EDGE_TYPE.RD_S;
            }
            else if (bRR)
            {
                return (int)MAP_EDGE_TYPE.LD_S;
            }
            else
            {
                return (int)MAP_EDGE_TYPE.DD;
            }
        }
        if (bDD)
        {
            if (bLL)
            {
                return (int)MAP_EDGE_TYPE.RU_S;
            }
            else if (bRR)
            {
                return (int)MAP_EDGE_TYPE.LU_S;
            }
            else
            {
                return (int)MAP_EDGE_TYPE.UU;
            }
        }

        if (bLL)
        {
            return (int)MAP_EDGE_TYPE.RR;
        }
        if (bRR)
        {
            return (int)MAP_EDGE_TYPE.LL;
        }

        bool bLU = theMap.GetValue(x - 1, y + 1) == inValue;
        bool bRU = theMap.GetValue(x + 1, y + 1) == inValue;
        bool bLD = theMap.GetValue(x - 1, y - 1) == inValue;
        bool bRD = theMap.GetValue(x + 1, y - 1) == inValue;
        if (bLU)
        {
            return (int)MAP_EDGE_TYPE.RD;
        }
        else if (bRU)
        {
            return (int)MAP_EDGE_TYPE.LD;
        }
        else if (bLD)
        {
            return (int)MAP_EDGE_TYPE.RU;
        }
        else if (bRD)
        {
            return (int)MAP_EDGE_TYPE.LU;
        }
        return (int)MAP_EDGE_TYPE.NOT;
    }

    //確認這個點是否是內部邊界，必須檢查週邊是否為 out
    virtual public int GetInEdgeType(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        bool bUU = IsOut(theMap.GetValue(x, y + 1), inValue, outValue);
        bool bDD = IsOut(theMap.GetValue(x, y - 1), inValue, outValue);
        bool bLL = IsOut(theMap.GetValue(x - 1, y), inValue, outValue);
        bool bRR = IsOut(theMap.GetValue(x + 1, y), inValue, outValue);

        if (bUU)
        {
            if (bLL)
            {
                return (int)MAP_EDGE_TYPE.LU;
            }
            else if (bRR)
            {
                return (int)MAP_EDGE_TYPE.RU;
            }
            else
            {
                return (int)MAP_EDGE_TYPE.UU;
            }
        }
        if (bDD)
        {
            if (bLL)
            {
                return (int)MAP_EDGE_TYPE.LD;
            }
            else if (bRR)
            {
                return (int)MAP_EDGE_TYPE.RD;
            }
            else
            {
                return (int)MAP_EDGE_TYPE.DD;
            }
        }

        if (bLL)
        {
            return (int)MAP_EDGE_TYPE.LL;
        }
        if (bRR)
        {
            return (int)MAP_EDGE_TYPE.RR;
        }

        bool bLU = IsOut(theMap.GetValue(x - 1, y + 1), inValue, outValue);
        bool bRU = IsOut(theMap.GetValue(x + 1, y + 1), inValue, outValue);
        bool bLD = IsOut(theMap.GetValue(x - 1, y - 1), inValue, outValue);
        bool bRD = IsOut(theMap.GetValue(x + 1, y - 1), inValue, outValue);
        if (bLU)
        {
            return (int)MAP_EDGE_TYPE.LU_S;
        }
        else if (bRU)
        {
            return (int)MAP_EDGE_TYPE.RU_S;
        }
        else if (bLD)
        {
            return (int)MAP_EDGE_TYPE.LD_S;
        }
        else if (bRD)
        {
            return (int)MAP_EDGE_TYPE.RD_S;
        }
        return (int)MAP_EDGE_TYPE.NOT;
    }

    //確認這個點是否是外部邊界，必須檢查週邊是否為 in
    virtual public Tile GetOutEdgeTile(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        return GetTile((MAP_EDGE_TYPE)GetOutEdgeType(theMap, x, y, inValue, outValue));
    }

    //確認這個點是否是內部邊界，必須檢查週邊是否為 out
    virtual public Tile GetInEdgeTile(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        return GetTile((MAP_EDGE_TYPE)GetInEdgeType(theMap, x, y, inValue, outValue));
    }

    virtual public Tile GetTileByMap(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE, bool outEdge = true)
    {
        if (outEdge)
        {
            return GetOutEdgeTile(theMap, x, y, inValue, outValue);
        }
        return null;
    }

    public Tile GetTile(int type)
    {
        return GetTile((MAP_EDGE_TYPE) type);
    }

    virtual public Tile GetTile(MAP_EDGE_TYPE type)
    {
        switch (type)
        {
            case MAP_EDGE_TYPE.UU:
                return UU;
            case MAP_EDGE_TYPE.DD:
                return DD;
            case MAP_EDGE_TYPE.LL:
                return LL;
            case MAP_EDGE_TYPE.RR:
                return RR;
            //====
            case MAP_EDGE_TYPE.LU:
                return LU;
            case MAP_EDGE_TYPE.RU:
                return RU;
            case MAP_EDGE_TYPE.LD:
                return LD;
            case MAP_EDGE_TYPE.RD:
                return RD;
            //====
            case MAP_EDGE_TYPE.LU_S:
                return LU_S;
            case MAP_EDGE_TYPE.RU_S:
                return RU_S;
            case MAP_EDGE_TYPE.LD_S:
                return LD_S;
            case MAP_EDGE_TYPE.RD_S:
                return RD_S;
        }
        return null;
    }
    
    public virtual void SetTile(Tilemap tm, MAP_EDGE_TYPE edgeType, Vector3Int pos)
    {
        tm.SetTile(pos, GetTile(edgeType));
    }

}

[System.Serializable]
public class TileEdge2LGroup : TileEdgeGroup
{
    //下
    public Tile DD2;
    //左下、右下
    public Tile LD2;
    public Tile RD2;
    //左下陷、右下陷
    public Tile LD_S2;
    public Tile RD_S2;

    public override void SetTile(Tilemap tm, MAP_EDGE_TYPE edgeType, Vector3Int pos)
    {
        base.SetTile(tm, edgeType, pos);
        Vector3Int dPos = pos + new Vector3Int(0, -1, 0);
        switch (edgeType)
        {
            case MAP_EDGE_TYPE.DD:
                tm.SetTile(dPos, DD2);
                return;
            case MAP_EDGE_TYPE.LD:
                tm.SetTile(dPos, LD2);
                return;
            case MAP_EDGE_TYPE.RD:
                tm.SetTile(dPos, RD2);
                return;
            case MAP_EDGE_TYPE.LD_S:
                tm.SetTile(dPos, LD_S2);
                return;
            case MAP_EDGE_TYPE.RD_S:
                tm.SetTile(dPos, RD_S2);
                return;
        }
    }

    public override Tile GetTile(MAP_EDGE_TYPE type)
    {
        switch (type)
        {
            case MAP_EDGE_TYPE.DD2:
                return DD2;
            case MAP_EDGE_TYPE.LD2:
                return LD2;
            case MAP_EDGE_TYPE.RD2:
                return RD2;
            case MAP_EDGE_TYPE.LD_S2:
                return LD_S2;
            case MAP_EDGE_TYPE.RD_S2:
                return RD_S2;
        }
        return base.GetTile(type);
    }

    public override int GetInEdgeType(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        if (theMap.IsValid(new Vector2Int(x, y+1)))
        {
            int upEType = base.GetInEdgeType(theMap, x, y+1, inValue, outValue);
            switch (upEType)
            {
                case (int)MAP_EDGE_TYPE.DD:
                    return (int)MAP_EDGE_TYPE.DD2;
                case (int)MAP_EDGE_TYPE.LD:
                    return (int)MAP_EDGE_TYPE.LD2;
                case (int)MAP_EDGE_TYPE.RD:
                    return (int)MAP_EDGE_TYPE.RD2;
                case (int)MAP_EDGE_TYPE.LD_S:
                    return (int)MAP_EDGE_TYPE.LD_S2;
                case (int)MAP_EDGE_TYPE.RD_S:
                    return (int)MAP_EDGE_TYPE.RD_S2;
            }
        }
        return base.GetInEdgeType(theMap, x, y, inValue, outValue);
    }
}


//為了能共用 TileEdgeGtoup 的編輯，用來可獨立成一個 Component 的容器
public class TileGroupDataBase : MonoBehaviour
{
    public virtual TileGroup GetTileGroup() { return null; }
}
public class TileEdgeGroupDataBase : MonoBehaviour
{
    public virtual TileEdgeGroup GetTileEdgeGroup() { return null; }
}
