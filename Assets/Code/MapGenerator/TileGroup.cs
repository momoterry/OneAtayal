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
    NOT = 0,
    UU = 11,
    DD = 12,
    LL = 13,
    RR = 14,
    LU = 21,
    RU = 22,
    LD = 23,
    RD = 24,
    LU_S = 31,
    RU_S = 32,
    LD_S = 33,
    RD_S = 34,
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

    virtual public Tile GetOutEdgeTile(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        return null;
    }

    virtual public Tile GetInEdgeTile(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        return null;
    }

    virtual public Tile GetTileByMap(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE, bool outEdge = true)
    {
        if (outEdge)
        {
            return GetOutEdgeTile(theMap, x, y, inValue, outValue);
        }
        return null;
    }

    //TODO: 舊方法，希望拿掉
    public Tile GetTile(MAP_EDGE_TYPE type)
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
}

public class TileGroupDataBase : MonoBehaviour
{
    public virtual TileGroup GetTileGroup() { return null; }
}
public class TileEdgeGroupDataBase : MonoBehaviour
{
    public virtual TileEdgeGroup GetTileEdgeGroup() { return null; }
}
