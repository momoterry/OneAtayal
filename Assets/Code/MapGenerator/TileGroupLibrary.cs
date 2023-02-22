using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_GROUP_ID
{
    GRASS,
    DIRT,
    DIRT_EDGE,
    HIGH_EDGE,
    ISLAND_EDGE,
}

public class TileGroupLibrary : MonoBehaviour
{
    public TileGroup grassGroup;
    public TileGroup dirtGroup;
    public TileGroup blockGroup;
    public TileEdgeGroup grassEdgeGroup;
    public TileEdgeGroup dirtEdgeGroup;
    public TileEdgeGroup highEdgeGroup;
    public TileEdge2LGroup islandEG;

    public TileGroup GetTileGroup( TILE_GROUP_ID _ID)
    {
        switch (_ID)
        {
            case TILE_GROUP_ID.GRASS:
                return grassGroup;
            case TILE_GROUP_ID.DIRT:
                return dirtGroup;
        }
        return null;
    }

    public TileEdgeGroup GetTileEdgeGroup(TILE_GROUP_ID _ID)
    {
        switch (_ID)
        {
            case TILE_GROUP_ID.DIRT_EDGE:
                return dirtEdgeGroup;
            case TILE_GROUP_ID.HIGH_EDGE:
                return highEdgeGroup;
        }
        return null;
    }

}
