using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    [System.Serializable]
    public class StepInfo
    {
        public float timeToWait = 0;
        public GameObject TriggerTarget;
    }

    public StepInfo[] allSteps;

    public bool isLoop = false;
    public float timeAfterLoop = 0;

    protected enum Phase
    {
        NONE,
        WAIT,
        ACTIVE,
        FINISH,
    }
    protected Phase currPhase = Phase.NONE;
    protected Phase nextPhase = Phase.WAIT;

    protected int currStep = 0;
    protected float stepTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (currPhase)
        {
            case Phase.ACTIVE:
                UpdateActive();
                break;
        }
        currPhase = nextPhase;
    }

    void UpdateActive()
    {
        stepTime += Time.deltaTime;
        if (stepTime >= allSteps[currStep].timeToWait)
        {
            if (allSteps[currStep].TriggerTarget)
            {
                allSteps[currStep].TriggerTarget.SendMessage("OnTG", gameObject, SendMessageOptions.DontRequireReceiver);
                currStep++;
                stepTime = 0;
                if (currStep >= allSteps.Length)
                {
                    if (isLoop)
                    {
                        currStep = 0;
                    }
                    else
                    {
                        nextPhase = Phase.FINISH;
                    }
                }
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (currPhase == Phase.WAIT)
        {
            nextPhase = Phase.ACTIVE;
        }
    }
}
