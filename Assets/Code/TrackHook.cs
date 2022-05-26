using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackHook : MonoBehaviour
{
    public GameObject trackRef;
    public bool StartAtBegin = true;

    protected GameObject myTrackObj;
    protected Transform mySlot = null;
    protected bool hooking = false;

    // Start is called before the first frame update
    void Start()
    {
        if (StartAtBegin)
        {          
            myTrackObj = BattleSystem.GetInstance().SpawnGameplayObject(trackRef, transform.position, false);
            if (myTrackObj)
            {
                TrackRoot tr = myTrackObj.GetComponent<TrackRoot>();
                if (tr)
                {
                    mySlot = tr.GetSlot();
                    hooking = true;
                    StartHook();               
                }

          }
        }
    }

    virtual protected void StartHook()
    {

    }

    virtual protected void EndHook()
    {

    }

    virtual protected void UpdateHook()
    {
        gameObject.transform.position = mySlot.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (hooking)
        {
            if (mySlot == null)
            {
                //Animation Done, Delete Self.
                Destroy(this);
            }
            else
            {
                UpdateHook();
            }
        }
    }

    void OnDestroy()
    {
        EndHook();
        if (myTrackObj)
        {
            Destroy(myTrackObj);
        }
    }


}
