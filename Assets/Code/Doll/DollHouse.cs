using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollHouse : MonoBehaviour
{
    //protected enum Phase
    //{
    //    NONE,
    //    WAIT,
    //    ACTIVE,
    //}
    //Phase currPhase = Phase.NONE;
    //Phase nextPhase = Phase.NONE;

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
        }

    }


    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && isPlayerIn == true)
        {
            isPlayerIn = false;
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
