using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTrigger : MonoBehaviour
{
    public GameObject[] TriggerTargetList;
    public float timePeriod = 2.0f;

    public bool Shuffle = false;
    public bool Loop = false;

    protected GameObject[] triggerSequence;
    protected int sequenceNum;
    protected int currIndex = 0;
    protected float currTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        sequenceNum = TriggerTargetList.Length;
        triggerSequence = new GameObject[sequenceNum];

        for (int i=0; i<sequenceNum; i++)
        {
            triggerSequence[i] = TriggerTargetList[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime >= timePeriod)
        {
            currTime = 0;
        }
    }

    void DoOneTrigger()
    {
        GameObject target = triggerSequence[currIndex];
        if (target)
        {
            target.SendMessage("OnTG", gameObject);
        }

        currIndex++;
        if (currIndex >= sequenceNum)
        {
            currIndex = 0;
            enabled = false;    //TODO: Loop ?
        }
    }

}
