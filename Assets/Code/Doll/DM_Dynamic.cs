using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Dynamic : DollManager
{
    public int MaxSlot = 40;

    public int FrontWidth = 4;
    public int MiddleDepth = 3;
    public int BackWidth = 4;

    protected float allShift = 0.0f;

    protected List<Doll> frontList = new List<Doll>();
    protected List<Doll> middleList = new List<Doll>();
    protected List<Doll> backList = new List<Doll>();

    protected bool needRebuild = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        slotNum = MaxSlot;
        DollSlots = new Transform[slotNum];
        for (int i = 0; i < slotNum; i++)
        {
            GameObject o = new GameObject("DynaSlot_" + i);
            o.transform.position = transform.position;
            o.transform.parent = transform;
            DollSlots[i] = o.transform;
        }
        dolls = new Doll[slotNum];
    }


    // Update is called once per frame
    void Update()
    {
        if (needRebuild)
        {
            RebuildFormation();
            needRebuild = false;
        }
    }

    protected void RebuildFrontSlots()
    {
        //int FrontWidth = 4; //TODO: 放成變數
        int frontNum = frontList.Count;

        if (frontNum <= 0)
            return;

        int nLine = ((frontNum-1) / FrontWidth ) + 1;
        int lastLineCount = (frontNum-1) % FrontWidth + 1;

        float fPos = Mathf.Max(1.0f, 2.0f - (float)(nLine-1) * 0.5f) + allShift;  //前方起始
        float slotDepth = 1.0f;
        fPos += slotDepth * (float)(nLine-1);

        for (int l=0; l<nLine; l++)
        {
            int num = FrontWidth;
            if (l == nLine - 1)
                num = lastLineCount;
            //print("Line: " + l + " Count: " + num);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i= l * FrontWidth; i< l * FrontWidth + num; i++)
            {
                //print("Prepare ..." + i);
                frontList[i].GetSlot().localPosition = new Vector3(lPos, 0, fPos);
                lPos += slotWidth;
            }
            fPos -= slotDepth;
        }
    }

    protected void RebuildMiddleSlots()
    {
        //int MiddleDepth = 3;    //TODO: 放成變數
        int middleNum = middleList.Count;

        if (middleNum <= 0)
            return;

        int circleNum = MiddleDepth + MiddleDepth;
        int nCircle = (middleNum - 1) / circleNum + 1;
        int lastCircleCount = (middleNum - 1) % circleNum + 1;

        float slotWidth = 1.0f;
        float innerWidth = 1.0f;    //最內圈距離

        float width = innerWidth;
        for (int c=0; c<nCircle; c++)
        {
            int num = circleNum;
            if (c == nCircle - 1)
                num = lastCircleCount;
            int nLine = (num - 1) / 2 + 1;
            float slotDepth = Mathf.Max(1.0f, 1.5f - (nLine - 1) * 0.25f);
            float totalDepth = (float)(nLine - 1) * slotDepth;
            float fPos = totalDepth * 0.5f + allShift;

            for (int l=0; l<nLine; l++)
            {
                int i = c * circleNum + l * 2;
                middleList[i].GetSlot().localPosition = new Vector3(-width, 0, fPos);  //左
                //print("Prepare... " + i);

                i++;
                if (i < middleNum)
                {
                    middleList[i].GetSlot().localPosition = new Vector3(width, 0, fPos);   //右
                    //print("Prepare... " + i);
                }

                fPos -= slotDepth;
            }
            width += slotWidth;
        }

    }

    protected void RebuildBackSlots()
    {
        //int BackWidth = 5; //TODO: 放成變數
        int backNum = backList.Count;

        if (backNum <= 0)
            return;

        int nLine = ((backNum - 1) / BackWidth) + 1;
        int lastLineCount = (backNum - 1) % BackWidth + 1;

        float bkPos = Mathf.Max(1.0f, 2.0f - (float)(nLine - 1) * 0.5f) - allShift;  //後方起始
        float slotDepth = 1.0f;
        bkPos += slotDepth * (float)(nLine - 1);

        for (int l = 0; l < nLine; l++)
        {
            int num = BackWidth;
            if (l == nLine - 1)
                num = lastLineCount;
            //print("Line: " + l + " Count: " + num);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i = l * BackWidth; i < l * BackWidth + num; i++)
            {
                //print("Prepare ..." + (backNum - i));
                backList[backNum-i-1].GetSlot().localPosition = new Vector3(lPos, 0, -bkPos);
                lPos += slotWidth;
            }
            bkPos -= slotDepth;
        }
    }

    protected void RebuildFormation()
    {
        frontList.Clear();
        middleList.Clear();
        backList.Clear();

        for (int i = 0; i < slotNum; i++)
        {
            if (dolls[i] && dolls[i].gameObject.activeInHierarchy)
            {
                switch(dolls[i].positionType)
                {
                    case DOLL_POSITION_TYPE.FRONT:
                        frontList.Add(dolls[i]);
                        break;
                    case DOLL_POSITION_TYPE.MIDDLE:
                        middleList.Add(dolls[i]);
                        break;
                    case DOLL_POSITION_TYPE.BACK:
                        backList.Add(dolls[i]);
                        break;
                }
            }
        }
        if (frontList.Count > 0)
        {
            RebuildFrontSlots();
        }
        if (middleList.Count > 0)
        {
            RebuildMiddleSlots();
        }
        if (backList.Count > 0)
        {
            RebuildBackSlots();
        }
    }


    public override Transform AddOneDoll(Doll doll, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    {
        //Transform result = base.AddOneDoll(doll, positionType);
        Transform result = null;
        for (int i=0; i<slotNum; i++)
        {
            if (dolls[i] == null && DollSlots[i] != null)
            {
                dolls[i] = doll;
                result = DollSlots[i];
                break;
            }
        }

        if (result)
        {
            //RebuilFormation();
            needRebuild = true;
        }

        return result;
    }

    public override void OnDollTempDeath(Doll doll)
    {
        DoRemoveDollFronList(doll);
    }

    public override void OnDollRevive(Doll doll)
    {
        needRebuild = true;
    }

    public override void OnDollDestroy(Doll doll)
    {
        DoRemoveDollFronList(doll);
    }

    protected void DoRemoveDollFronList(Doll doll)
    {
        switch (doll.positionType)
        {
            case DOLL_POSITION_TYPE.FRONT:
                frontList.Remove(doll);
                RebuildFrontSlots();
                break;
            case DOLL_POSITION_TYPE.MIDDLE:
                middleList.Remove(doll);
                RebuildMiddleSlots();
                break;
            case DOLL_POSITION_TYPE.BACK:
                backList.Remove(doll);
                RebuildBackSlots();
                break;
        }
    }
}
