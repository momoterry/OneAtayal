using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileReplaceTool : MonoBehaviour
{
    public Tile tileFrom;
    public Tile tileTo;

    public GameObject mapRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReplaceTarget()
    {
        if (!mapRoot)
            return;

        Tilemap[] allMaps = mapRoot.GetComponentsInChildren<Tilemap>(true);
        print("==== ReplaceTarget : ====");
        foreach ( Tilemap map in allMaps)
        {
            print(map.name);
            map.SwapTile(tileFrom, tileTo);
        }
        print("==== Done ====");
    }

}
