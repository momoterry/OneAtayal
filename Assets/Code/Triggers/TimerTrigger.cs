using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTrigger : MonoBehaviour
{
    [System.Serializable]
    public class StepInfo
    {
        float timeToWait = 0;
        public GameObject[] TriggerTargets;
    }

    public StepInfo[] allSteps;

    public bool isLoop = false;

    protected enum Phase
    {
        NONE,
        WAIT,
        ACTIVE,
        FINISH,
    }
    protected Phase currPhase = Phase.NONE;
    protected Phase nextPhase = Phase.WAIT;

    protected int currTrigger = 0;
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
        
    }

}
