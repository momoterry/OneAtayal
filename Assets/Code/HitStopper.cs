using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopper : MonoBehaviour
{
    private float hitStopTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hitStopTime > 0)
        {
            hitStopTime -= Time.unscaledDeltaTime;
            if (hitStopTime <= 0)
            {
                Time.timeScale = 1.0f;
            }
        }
    }

    public void DoHitStop( float duration)
    {
        hitStopTime = duration;
        Time.timeScale = 0;
    }
}
