using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] EnableTargets;
    public GameObject[] DisableTargets;

    void OnTG(GameObject whoTG)
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
}
