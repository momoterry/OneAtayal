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
    public float timeBetweenLoop = 0;
    public bool triggeredAgainToStop = false;

    protected enum Phase
    {
        NONE,
        WAIT,
        ACTIVE,
        LOOP_WAIT,
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
            case Phase.LOOP_WAIT:
                UpdateLoopWait();
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
                        nextPhase = Phase.LOOP_WAIT;
                    }
                    else
                    {
                        nextPhase = Phase.FINISH; //TODO: 如果是能反覆開關?
                    }
                }
            }
        }
    }

    void UpdateLoopWait()
    {
        stepTime += Time.deltaTime;
        if (stepTime >= timeBetweenLoop)
        {
            stepTime = 0;
            nextPhase = Phase.ACTIVE;
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (currPhase == Phase.WAIT )
        {
            nextPhase = Phase.ACTIVE;
        }
        else if (triggeredAgainToStop && (currPhase == Phase.ACTIVE || currPhase == Phase.LOOP_WAIT))
        {
            nextPhase = Phase.FINISH;   //TODO: 如果是能反覆開關?
        }
    }
}
