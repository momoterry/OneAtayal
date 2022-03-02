using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class DenGen_Bata : MapGeneratorBase
{
    //TODO: 以下應該被視為 Dungeon Generator 的 Base
    public GameObject gridRoot;
    public NavMeshSurface2d theSurface2D;

    public GameObject roomRefTest;

    int toBuild = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (toBuild > 0)
        {
            toBuild--;
        }
        else
        {
            theSurface2D.BuildNavMesh();
        }
    }

    public override void BuildAll(int buildLevel = 1)
    {
        base.BuildAll(buildLevel);

        GameObject rObj = null;

        rObj = Instantiate(roomRefTest, transform.position, Quaternion.identity, gridRoot.transform);
        //Grid grid = rObj.GetComponent<Grid>();
        //grid.enabled = false;

        rObj = Instantiate(roomRefTest, transform.position + new Vector3(-20.0f, 0, 0), Quaternion.identity, gridRoot.transform);
        //grid = rObj.GetComponent<Grid>();
        //grid.enabled = false;

        //theSurface2D.BuildNavMesh();
    }
}
