using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DM_Trailer : DollManager
{
    protected int trailSlotNum = 40;

    protected int dotNum = 0;
    protected float dotDis = 0.5f;
    protected int slotDotNum = 2;


    protected float dotDisSqr = 0;

    protected Vector3[] dotArray = null;

    protected int currIndex = 0;        //Index 暫增時，越遠離玩家，直到

    //Debug
    public GameObject testDotRef;
    public GameObject testSlotRef;
    protected GameObject[] testDotArray = null;
    protected GameObject[] testSlotArray = null;

    public bool IsDebug = false;

    protected List<Doll> dollList = new List<Doll>();

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

        //初始化所有 Dot 的位置以免一開始重疊
        InitDotPosition();

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

    protected void InitDotPosition()
    {
        int halfWidth = slotDotNum * 3;  //可參數化

        float step = dotDis * 1.5f;
        Vector3 vDir = Vector3.back * step;
        Vector3 hDir = Vector3.right * step;
        dotArray[0] = transform.position;
        dotArray[1] = transform.position + vDir;
        
        Vector3 currPos = dotArray[1];
        int i = 2;
        int width = halfWidth + halfWidth;
        int currStep = halfWidth;
        while (i < dotNum)
        {
            currPos += hDir * step;
            dotArray[i] = currPos;
            i++;
            currStep++;
            if (currStep >= width && i < dotNum)
            {
                currPos += vDir;
                dotArray[i] = currPos;
                i++;
                currStep = 0;
                hDir = -hDir;
            }
        }

        //for (i = 0; i < dotNum; i++)
        //{
        //    //print(dotArray[i]);
        //    //dotArray[i] = transform.position -Vector3.forward* i *dotDis;
        //    dotArray[i] = transform.position + Vector3.back * 0.5f * i;
        //}
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
    protected override void Update()
    {
        base.Update();

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

    public override bool AddOneDoll(Doll doll, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    {

        //for (int i = 0; i < slotNum; i++)
        //{
        //    if (dolls[i] == null && DollSlots[i] != null)
        //    {
        //        dolls[i] = doll;
        //        doll.SetSlot(DollSlots[i]);
        //        //return DollSlots[i];

        //        dollList.Add(doll);
        //        ResortDollList();

        //        return true;
        //    }
        //}
        if (dollList.Count < slotNum)
        {
            doll.SetSlot(DollSlots[dollList.Count]);
            dollList.Add(doll);
            ResortDollList();
            return true;
        }

        return false;
    }

    public override void OnDollTempDeath(Doll doll)
    {
        //dollList.Remove(doll);
        ResortDollList();
    }

    public override void OnDollRevive(Doll doll)
    {
        //dollList.Add(doll);
        ResortDollList();
    }

    public override void OnDollDestroy(Doll doll)
    {
        dollList.Remove(doll);
        ResortDollList();
    }


    protected class DollComparer : IComparer<Doll>
    {
        public int Compare(Doll x, Doll y)
        {
            // 使用字串比較的方式進行排序
            return (int)x.positionType - (int)y.positionType 
                + (x.isActiveAndEnabled ? -1000:0) - (y.isActiveAndEnabled? -1000:0);
        }
    }

    protected void ResortDollList()
    {
        dollList.Sort(new DollComparer());
        for (int i=0;i<dollList.Count; i++)
        {
            dollList[i].SetSlot(DollSlots[i]);
            dolls[i] = dollList[i];
        }
        for (int i=dollList.Count; i<slotNum; i++)
        {
            dolls[i] = null;
        }
    }

}
