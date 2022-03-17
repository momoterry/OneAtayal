using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountTR : MonoBehaviour
{
    public int count;
    public GameObject[] TriggerTargets;

    protected int currCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTG(GameObject whoTG)
    {
        currCount++;
        if (currCount == count)
        {
            foreach (GameObject o in TriggerTargets)
            {
                o.SendMessage("OnTG", gameObject);
            }
        }
    }
}
