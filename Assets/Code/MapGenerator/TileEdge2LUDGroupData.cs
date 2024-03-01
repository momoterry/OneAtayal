using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileEdge2LUDGroup : TileEdge2LGroup
{
    //上
    public Tile UU2;
    //左上、右上
    public Tile LU2;
    public Tile RU2;
    //左上陷、右上陷
    public Tile LU_S2;
    public Tile RU_S2;
    public override Tile GetTile(MAP_EDGE_TYPE type)
    {
        switch (type)
        {
            case MAP_EDGE_TYPE.UU2:
                return UU2;
            case MAP_EDGE_TYPE.LU2:
                return LU2;
            case MAP_EDGE_TYPE.RU2:
                return RU2;
            case MAP_EDGE_TYPE.LU_S2:
                return LU_S2;
            case MAP_EDGE_TYPE.RU_S2:
                return RU_S2;
        }
        return base.GetTile(type);
    }

    public override int GetInEdgeType(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        //if (theMap.IsValid(new Vector2Int(x, y - 1)))
        //{
        //    int upEType = base.GetInEdgeType(theMap, x, y - 1, inValue, outValue);
        //    switch (upEType)
        //    {
        //        case (int)MAP_EDGE_TYPE.UU:
        //            return (int)MAP_EDGE_TYPE.UU2;
        //        case (int)MAP_EDGE_TYPE.LU:
        //            return (int)MAP_EDGE_TYPE.LU2;
        //        case (int)MAP_EDGE_TYPE.RU:
        //            return (int)MAP_EDGE_TYPE.RU2;
        //        case (int)MAP_EDGE_TYPE.LU_S:
        //            return (int)MAP_EDGE_TYPE.LU_S2;
        //        case (int)MAP_EDGE_TYPE.RU_S:
        //            return (int)MAP_EDGE_TYPE.RU_S2;
        //    }
        //}
        //return base.GetInEdgeType(theMap, x, y, inValue, outValue);

        int baseEType = base.GetInEdgeType(theMap, x, y, inValue, outValue);
        switch (baseEType)
        {
            case (int)MAP_EDGE_TYPE.UU:
                return (int)MAP_EDGE_TYPE.UU2;
            case (int)MAP_EDGE_TYPE.LU:
                return (int)MAP_EDGE_TYPE.LU2;
            case (int)MAP_EDGE_TYPE.RU:
                return (int)MAP_EDGE_TYPE.RU2;
            case (int)MAP_EDGE_TYPE.LU_S:
                return (int)MAP_EDGE_TYPE.LU_S2;
            case (int)MAP_EDGE_TYPE.RU_S:
                return (int)MAP_EDGE_TYPE.RU_S2;
        }
        //如果上面那格是上方 Edge ，自己就是
        if (theMap.IsValid(new Vector2Int(x, y + 1)))
        {
            int upEType = base.GetInEdgeType(theMap, x, y + 1, inValue, outValue);
            switch (upEType)
            {
                case (int)MAP_EDGE_TYPE.UU:
                    return (int)MAP_EDGE_TYPE.UU;
                case (int)MAP_EDGE_TYPE.LU:
                    return (int)MAP_EDGE_TYPE.LU;
                case (int)MAP_EDGE_TYPE.RU:
                    return (int)MAP_EDGE_TYPE.RU;
                case (int)MAP_EDGE_TYPE.LU_S:
                    return (int)MAP_EDGE_TYPE.LU_S;
                case (int)MAP_EDGE_TYPE.RU_S:
                    return (int)MAP_EDGE_TYPE.RU_S;
            }
        }
        return baseEType;
    }

    public override int GetOutEdgeType(OneMap theMap, int x, int y, int inValue, int outValue = OneMap.INVALID_VALUE)
    {
        if (theMap.IsValid(new Vector2Int(x, y - 1)))
        {
            int upEType = base.GetOutEdgeType(theMap, x, y - 1, inValue, outValue);
            switch (upEType)
            {
                case (int)MAP_EDGE_TYPE.UU:
                    return (int)MAP_EDGE_TYPE.UU2;
                case (int)MAP_EDGE_TYPE.LU:
                    return (int)MAP_EDGE_TYPE.LU2;
                case (int)MAP_EDGE_TYPE.RU:
                    return (int)MAP_EDGE_TYPE.RU2;
                case (int)MAP_EDGE_TYPE.LU_S:
                    return (int)MAP_EDGE_TYPE.LU_S2;
                case (int)MAP_EDGE_TYPE.RU_S:
                    return (int)MAP_EDGE_TYPE.RU_S2;
            }
        }
        return base.GetOutEdgeType(theMap, x, y, inValue, outValue);
    }
}

public class TileEdge2LUDGroupData : TileEdgeGroupDataBase
{
    public TileEdge2LUDGroup data;
    // Start is called before the first frame update
    public override TileEdgeGroup GetTileEdgeGroup()
    {
        return data;
    }
}
