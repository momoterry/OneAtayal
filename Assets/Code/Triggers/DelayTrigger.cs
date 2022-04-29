using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTrigger : MonoBehaviour
{
    public float delayTime;
    // Start is called before the first frame update
    public GameObject[] TriggerTargets;

    protected float timeToTrigger = -1.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timeToTrigger >= 0)
        {
            timeToTrigger -= Time.deltaTime;
            if (timeToTrigger <= 0)
            {
                foreach (GameObject o in TriggerTargets)
                {
                    o.SendMessage("OnTG", gameObject);
                }
                timeToTrigger = -1.0f;
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (timeToTrigger <= 0)
        {
            timeToTrigger = delayTime;
        }
    }
}
