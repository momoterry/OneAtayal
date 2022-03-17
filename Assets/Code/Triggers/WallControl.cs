using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl : MonoBehaviour
{
    public GameObject[] walls;

    // Start is called before the first frame update
    protected enum Phase
    {
        NONE,
        WAIT,
        BATTLE,
        CLEAN_WALL,
        FINISH,
    }
    protected Phase currPhase = Phase.NONE;
    protected Phase nextPhase = Phase.NONE;

    protected float phaseTime = 0;

    void Start()
    {
        DoPrepareWalls();
        nextPhase = Phase.WAIT;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextPhase != currPhase)
        {
            switch (nextPhase)
            {
                case Phase.BATTLE:
                    DoStartWalls();
                    break;
                case Phase.CLEAN_WALL:
                    DoStopWalls();
                    break;
            }
            currPhase = nextPhase;
        }
        else
        {
            switch (currPhase)
            {
                case Phase.CLEAN_WALL:
                    phaseTime += Time.deltaTime;
                    if (phaseTime > 1.0f)
                    {
                        nextPhase = Phase.FINISH;
                    }
                    break;
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        switch (currPhase)
        {
            case Phase.WAIT:
                nextPhase = Phase.BATTLE;
                break;
            case Phase.BATTLE:
                nextPhase = Phase.CLEAN_WALL;
                break;
        }
    }

    protected void DoPrepareWalls( )
    {
        foreach (GameObject o in walls)
        {
            o.SetActive(false);
        }
    }

    protected void DoStartWalls()
    {
        foreach (GameObject o in walls)
        {
            o.SetActive(true);
        }
    }

    protected void DoStopWalls()
    {
        //¼È¥N
        foreach (GameObject o in walls)
        {
            o.SetActive(false);
        }
    }

    protected void DoDestroyWalls()
    {
        foreach (GameObject o in walls)
        {
            Destroy(o);
        }
    }
}
