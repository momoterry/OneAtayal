using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Trailer : DollManager
{
    protected int trailSlotNum = 20;

    protected int dotNum = 0;
    protected float dotDis = 0.5f;
    protected int slotDotNum = 2;


    protected float dotDisSqr = 0;

    protected Vector3[] dotArray = null;

    protected int currIndex = 0;

    //Debug
    public GameObject testDotRef;
    public GameObject testSlotRef;
    protected GameObject[] testDotArray = null;
    protected GameObject[] testSlotArray = null;

    protected bool IsDebug = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        //base.Start();
        slotNum = trailSlotNum;
        DollSlots = new Transform[slotNum];
        for (int i=0; i< slotNum; i++)
        {
            GameObject o = new GameObject("TrailSlot_"+i);
            DollSlots[i] = o.transform;
        }
        dolls = new Doll[slotNum];


        dotNum = slotDotNum * slotNum + 10;

        dotArray = new Vector3[dotNum];
        for (int i=0; i<dotNum; i++)
        {
            //print(dotArray[i]);
            //dotArray[i] = transform.position -Vector3.forward* i *dotDis;
            dotArray[i] = transform.position;
        }
        dotDisSqr = dotDis * dotDis;

        for (int i=0; i< slotNum; i++)
        {
            DollSlots[i].parent = null; //暴力法: TODO: 直接重新生成
        }

        //Debug
        if (IsDebug)
        {
            testDotArray = new GameObject[dotNum];
            for (int i = 0; i < dotNum; i++)
            {
                //dotArray[i] = transform.position;
                testDotArray[i] = BattleSystem.GetInstance().SpawnGameplayObject(testDotRef, dotArray[i]);
            }

            testSlotArray = new GameObject[slotNum];
            for (int i = 0; i < slotNum; i++)
            {
                testSlotArray[i] = BattleSystem.GetInstance().SpawnGameplayObject(testSlotRef, transform.position);
            }
        }

        SetupSlotPosition();

    }

    void SetupSlotPosition()
    {
        int currSlotIndex = currIndex;
        for (int i = 0; i < slotNum; i++)
        {
            currSlotIndex += slotDotNum;
            if (currSlotIndex >= dotNum)
            {
                currSlotIndex -= dotNum;
            }

            DollSlots[i].position = dotArray[currSlotIndex];

            if (IsDebug)
                testSlotArray[i].transform.position = dotArray[currSlotIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - dotArray[currIndex]).sqrMagnitude >= dotDisSqr)
        {
            currIndex--;
            if (currIndex <0 )
            {
                currIndex = dotNum - 1;
            }
            dotArray[currIndex] = transform.position;

            //TEST
            if (IsDebug)
            {
                testDotArray[currIndex].transform.position = dotArray[currIndex];
            }

            SetupSlotPosition();
        }
    }
}
