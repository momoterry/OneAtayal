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

    public StepInfo allSteps;

    public bool isLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
