using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollHouse : MonoBehaviour
{
    public DollBackpackUI theUI;

    bool isPlayerIn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
 

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && isPlayerIn == false)
        {
            isPlayerIn = true;
            if (theUI)
            {
                theUI.OpenBackpackUI();
            }
        }

    }


    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && isPlayerIn == true)
        {
            isPlayerIn = false;
            if (theUI)
            {
                theUI.CloseBackpackUI();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (isPlayerIn)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 2.0f);
    }
}
