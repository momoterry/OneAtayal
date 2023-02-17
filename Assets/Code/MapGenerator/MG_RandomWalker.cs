using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MG_RandomWalker : MapGeneratorBase
{
    public Tilemap groundTM;

    public TileGroup grassTG;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected IEnumerator BuildMapIterator()
    {
        float stepTime = 0.025f;
        for ( int x=-5; x<5; x++)
        {
            for (int y=-5; y<5; y++)
            {
                groundTM.SetTile(new Vector3Int(x, y, 0), grassTG.GetOneTile());
                yield return new WaitForSeconds(stepTime);
            }
        }

        theSurface2D.BuildNavMesh();
    }

    public override void BuildAll(int buildLevel = 1)
    {
        IEnumerator theC = BuildMapIterator();
        StartCoroutine(theC);
    }

}
