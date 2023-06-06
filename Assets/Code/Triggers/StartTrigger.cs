using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    public GameObject[] triggerTargets;
    public float startTime = 0.1f;

    protected float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                DoIt();
            }
        }
    }

    void DoIt()
    {
        print("Do It !!");
        for (int i = 0; i < triggerTargets.Length; i++)
        {
            if (triggerTargets[i])
            {
                triggerTargets[i].SendMessage("OnTG", gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
