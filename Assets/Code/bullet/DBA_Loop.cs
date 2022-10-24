using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBA_Loop : DamageByAnimation
{
    public float loopTime = 2.0f;
    public Animator myAnimator;

    protected float timeToStopLoop = 0;
    // Start is called before the first frame update
    void Start()
    {
        timeToStopLoop = loopTime;
        if (myAnimator == null)
        {
            myAnimator = GetComponent<Animator>();
        }
        if (myAnimator)
        {
            myAnimator.SetBool("LOOP", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToStopLoop > 0)
        {
            timeToStopLoop -= Time.deltaTime;
            if (timeToStopLoop < 0)
            {
                if (myAnimator)
                {
                    myAnimator.SetBool("LOOP", false);
                }
                timeToStopLoop = 0;
            }
        }
    }
}
