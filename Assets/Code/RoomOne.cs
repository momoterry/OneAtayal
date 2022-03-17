using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomOne : MonoBehaviour
{
    public NavMeshSurface2d theSurface2D;
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
