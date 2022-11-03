using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSwitch : MonoBehaviour
{
    public GameObject[] EachTriggerTargets;
    public bool loop;

    protected int currIndex = 0;

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
        if (EachTriggerTargets.Length == 0 && currIndex < 0)
            return;

        if (EachTriggerTargets[currIndex])
        {
            EachTriggerTargets[currIndex].SendMessage("OnTG", gameObject);
            currIndex++;
            if (currIndex == EachTriggerTargets.Length)
            {
                currIndex = loop ? 0 : -1;
            }
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
        }
    }
}
