using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTG : MonoBehaviour
{
    public GameObject[] TriggerTargets;

    public bool triggerOnce = true;

    private bool isTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && isTriggered == false)
        {
            //print("Player In !!");
            foreach (GameObject o in TriggerTargets)
            {
                o.SendMessage("OnTG", col.gameObject);
            }
            if (triggerOnce)
            {
                isTriggered = true;
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTriggered == false)
        {
            //print("Player In !!");
            foreach (GameObject o in TriggerTargets)
            {
                if (o)
                    o.SendMessage("OnTG", other.gameObject);
            }
            if (triggerOnce)
            {
                isTriggered = true;
                enabled = false;
            }
        }
    }
}
