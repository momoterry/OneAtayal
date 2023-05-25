using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroupData : TileGroupDataBase
{
    public TileGroup data;


    public override TileGroupBase GetTileGroup()
    {
        return data;
    }
}
