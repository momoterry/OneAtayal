using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG_PuzzleDisjointSet : MG_ForestRD
{
    protected override void CreateForestMap()
    {
        FillSquareInMap((int)TILE_TYPE.BLOCK, mapCenter + new Vector3Int(-4, 4, 0), 4, 4);
    }
}
