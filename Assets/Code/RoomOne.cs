using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class RoomOne : MonoBehaviour
{
    public NavMeshSurface theSurface2D;
    // Start is called before the first frame update
    void Start()
    {
        if (theSurface2D)
        {
            theSurface2D.BuildNavMeshAsync();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
