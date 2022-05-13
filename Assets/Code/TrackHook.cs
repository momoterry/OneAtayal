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
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hooking)
        {
            gameObject.transform.position = mySlot.position;
        }
    }

    private void OnDestroy()
    {
        if (myTrackObj)
        {
            Destroy(myTrackObj);
        }
    }
}
