using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEdge2LGroupData : TileEdgeGroupDataBase
{
    public TileEdge2LGroup data;

    public override TileEdgeGroup GetTileEdgeGroup()
    {
        return data;
    }
}
