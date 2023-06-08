using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TimeSequenceTrigger : MonoBehaviour
{
    [System.Serializable]
    public class Clip
    {
        public GameObject triggerTarget;
        public float waitTime;
    }
    public Clip[] clips;

    protected int currClipIndex = -1;
    protected float waitTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currClipIndex >= 0)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                clips[currClipIndex].triggerTarget.SendMessage("OnTG", gameObject, SendMessageOptions.DontRequireReceiver);

                currClipIndex++;
                if (currClipIndex >= clips.Length)
                {
                    currClipIndex = -1; //µ²§ô
                }
                else
                {
                    waitTime += clips[currClipIndex].waitTime;
                }
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (currClipIndex < 0 && clips.Length > 0)
        {
            currClipIndex = 0;
            waitTime = clips[0].waitTime;
        }
    }
}
