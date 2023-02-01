using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

//[System.Serializable]
//public struct TileInfo
//{
//    public Tile t;
//    public float percent; //¾÷²v
//}

[System.Serializable]
public class TileGroup
{
    public Tile baseTile;
    public Tile[] decorateTiles;
    public float decorateRate;
}

public class MG_ForestRD : MapGeneratorBase
{
    public Tilemap groundTM;
    public TileGroup grassGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void BuildSquareArea( TileGroup tg, Vector3Int center, int width, int height)
    {
        int hWidth = width / 2;
        int hHeight = height / 2;
        Vector3Int cd = Vector3Int.zero;
        //theSurface2D.BuildNavMesh();
        for (int x = -hWidth; x < hWidth; x++)
        {
            for (int y = -hHeight; y < hHeight; y++)
            {
                cd.x = center.x + x;
                cd.y = center.y + y;
                Tile t = tg.baseTile;
                if (tg.decorateTiles.Length > 0 && tg.decorateRate > 0)
                {
                    float rd = Random.Range(0, 1.0f);
                    if (rd < tg.decorateRate)
                    {
                        t = tg.decorateTiles[Random.Range(0, tg.decorateTiles.Length)];
                    }
                }
                groundTM.SetTile(cd, t);
            }
        }
    }

    public override void BuildAll(int buildLevel = 1)
    {
        BuildSquareArea(grassGroup, new Vector3Int(0, 1, 0), 16, 40);

        theSurface2D.BuildNavMesh();
    }
}
