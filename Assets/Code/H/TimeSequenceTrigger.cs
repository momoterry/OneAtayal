using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TimeSequenceTrigger : MonoBehaviour
{
    [System.Serializable]
    public class Clip
    {
        public GameObject triggerTarget;
        public float waitTimeAfter;     //觸發後的等待
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
                if (currClipIndex >= clips.Length)
                {
                    currClipIndex = -1; //結束
                    //print("結束.........");
                }
                else
                {
                    clips[currClipIndex].triggerTarget.SendMessage("OnTG", gameObject, SendMessageOptions.DontRequireReceiver);
                    waitTime += clips[currClipIndex].waitTimeAfter;
                    currClipIndex++;
                }
            }
        }
    }

    void OnTG(GameObject whoTG)
    {
        if (currClipIndex < 0 && clips.Length > 0)
        {
            currClipIndex = 0;
            waitTime = 0;
        }
    }
}
