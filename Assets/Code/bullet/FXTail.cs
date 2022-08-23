using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class FXTail : MonoBehaviour
{
    public float TailStepTime = 0.05f;
    public int ParticleNum = 6;

    public float SlotStepTime = 0.2f;

    public Sprite ParticleImange;

    protected GameObject[] particleObjList = null;
    protected int slotNum = 0;
    protected Vector3[] tailSlots = null;

    protected float currTime = 0;
    protected int currSlotStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        particleObjList = new GameObject[ParticleNum];
        slotNum = (int)Mathf.Round((float)ParticleNum*TailStepTime/SlotStepTime) + 1;
        tailSlots = new Vector3[slotNum];

        for (int i=0; i<ParticleNum; i++)
        {
            GameObject o = new GameObject("FXTail_Child");
            SpriteRenderer sr = o.AddComponent<SpriteRenderer>();
            sr.sprite = ParticleImange;
            o.AddComponent<OrderAdjust>();

            o.transform.rotation = Quaternion.Euler(90, 0, 0);
            o.transform.position = transform.position + Vector3.up * (float)i * -0.01f;
            //ParticleObj.transform.parent = transform;
            particleObjList[i] = o;
        }

        for (int i=0; i<slotNum; i++)
        {
            tailSlots[i] = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime > SlotStepTime)
        {
            currTime -= SlotStepTime;
            currSlotStart--;
            if (currSlotStart < 0)
            {
                currSlotStart = slotNum - 1;
            }
            tailSlots[currSlotStart] = transform.position;
        }

        //TEST
        //for (int i = 0; i < ParticleNum; i++)
        //{
        //    particleObjList[i].transform.position = tailSlots[i];
        //}

        //float tLength = SlotStepTime - currTime;
        //for (int i = 0; i < ParticleNum; i++)
        //{
        //    int lengthInSlot = (int)(tLength / SlotStepTime);
        //    float ratio = (tLength - ((float)lengthInSlot * SlotStepTime)) / SlotStepTime;
        //    int fmSlot = currSlotStart + lengthInSlot;
        //    int toSlot = fmSlot + 1;
        //    if (toSlot >= slotNum)
        //    {
        //        toSlot -= slotNum;
        //    }
        //    if (fmSlot >= slotNum)
        //    {
        //        fmSlot -= slotNum;
        //    }

        //    Vector3 pos = tailSlots[fmSlot] + (tailSlots[toSlot] - tailSlots[fmSlot]) * ratio;
        //    particleObjList[i].transform.position = pos;

        //    tLength += TailStepTime;
        //}

        float tLength = 0;
        for (int i = 0; i < ParticleNum; i++)
        {
            if (tLength < currTime)
            {
                particleObjList[i].transform.position = transform.position + (tailSlots[currSlotStart] - transform.position) * (tLength / currTime);
            }
            else
            {
                float tLengthLeft = tLength - currTime;
                int lengthInSlot = (int)(tLengthLeft / SlotStepTime);
                float ratio = (tLengthLeft - ((float)lengthInSlot * SlotStepTime)) / SlotStepTime;
                int fmSlot = currSlotStart + lengthInSlot;
                if (fmSlot >= slotNum)
                {
                    fmSlot -= slotNum;
                }                
                int toSlot = fmSlot + 1;
                if (toSlot >= slotNum)
                {
                    toSlot -= slotNum;
                }

                particleObjList[i].transform.position = tailSlots[fmSlot] + (tailSlots[toSlot] - tailSlots[fmSlot]) * ratio; ;
            }

            tLength += TailStepTime;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < ParticleNum; i++)
        {
            Destroy(particleObjList[i]);
        }
        particleObjList = null;
        tailSlots = null;
    }
}
