using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTrigger : MonoBehaviour
{
    public float timePeriod = 2.0f;
    public bool Shuffle = false;
    public bool Loop = false;

    public GameObject[] TriggerTargetList;

    protected GameObject[] triggerSequence;
    protected int sequenceNum;
    protected int currIndex = 0;
    protected float currTime = 0;

    protected enum PHASE
    {
        NONE,
        RUNNING,
        END,
    }
    PHASE currPhase = PHASE.NONE;

    // Start is called before the first frame update
    void Start()
    {
        sequenceNum = TriggerTargetList.Length;
        triggerSequence = new GameObject[sequenceNum];

        for (int i=0; i<sequenceNum; i++)
        {
            triggerSequence[i] = TriggerTargetList[i];
        }

        if (Shuffle)
        {
            ShuffleSequence();
        }
    }

    void ShuffleSequence()
    {
        for (int i = sequenceNum-1; i>=0; i--)
        {
            int rd = Random.Range(0, i);
            GameObject tmp = triggerSequence[rd];
            triggerSequence[rd] = triggerSequence[i];
            triggerSequence[i] = tmp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currPhase == PHASE.RUNNING)
        {
            currTime += Time.deltaTime;
            if (currTime >= timePeriod)
            {
                DoOneTrigger();
                currTime = 0;
            }
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
            if (Loop) 
            {
                currIndex = 0;
                if (Shuffle)
                {
                    ShuffleSequence();
                }
            }
            else
            {
                currPhase = PHASE.END;
                enabled = false;
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        //print("GO!!");
        if (currPhase == PHASE.NONE)
        {
            currPhase = PHASE.RUNNING;
        }
    }

}
