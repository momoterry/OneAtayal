using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEnabler : MonoBehaviour
{
    // Start is called before the first frame update
    public string EventName;

    public GameObject[] EnableTargets;
    public GameObject[] DisableTargets;

    protected bool isPending = false;

    void Start()
    {
        //print("PlaeyrData Ready ? " + GameSystem.GetPlayerData().IsReady());

        if (GameSystem.GetPlayerData().IsReady())
        {
            DoIt();
        }
        else
        {
            isPending = true;
        }
    }

    void Update()
    {
        if (isPending)
        {
            if (GameSystem.GetPlayerData().IsReady())
            {
                DoIt();
                isPending = false;
            }
        }
    }

    protected void DoIt()
    {
        //print("DoIt()");
        if (GameSystem.GetPlayerData().GetEvent(EventName))
        {
            foreach (GameObject o in EnableTargets)
            {
                o.SetActive(true);
            }
            foreach (GameObject o in DisableTargets)
            {
                o.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject o in DisableTargets)
            {
                o.SetActive(true);
            }
            foreach (GameObject o in EnableTargets)
            {
                o.SetActive(false);
            }
        }

        enabled = false;
    }
}
