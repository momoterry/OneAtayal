using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestMap : MapGeneratorBase
{
    public bool useMiniMap = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void BuildAll(int buildLevel = 1)
    {
        if (useMiniMap)
        {

        }
    }
}
