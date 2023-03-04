using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEdgeGroupData : TileEdgeGroupDataBase
{
    public TileEdgeGroup data;

    public override TileEdgeGroup GetTileEdgeGroup()
    {
        return data;
    }
}
