using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Trailer : DollManager
{
    protected int dotNum = 50;
    protected float dotDis = 0.5f;
    protected float dotDisSqr = 0;

    protected Vector3[] dotArray = null;

    protected int currIndex = 0;

    //Debug
    public GameObject testDotRef;
    protected GameObject[] testObjArray = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        dotArray = new Vector3[dotNum];
        for (int i=0; i<dotNum; i++)
        {
            print(dotArray[i]);
            dotArray[i] = transform.position -Vector3.forward*(dotNum-i)*dotDis;
        }

        testObjArray = new GameObject[dotNum];
        for (int i = 0; i < dotNum; i++)
        {
            print(testObjArray[i]);
            //dotArray[i] = transform.position;
            testObjArray[i] = BattleSystem.GetInstance().SpawnGameplayObject(testDotRef, dotArray[i]);
        }

        dotDisSqr = dotDis * dotDis;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - dotArray[currIndex]).sqrMagnitude >= dotDisSqr)
        {
            currIndex++;
            if (currIndex >= dotNum)
            {
                currIndex = 0;
            }
            dotArray[currIndex] = transform.position;
            //TEST
            testObjArray[currIndex].transform.position = dotArray[currIndex];
        }
    }
}
