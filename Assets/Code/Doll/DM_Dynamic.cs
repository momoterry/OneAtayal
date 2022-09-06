using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Dynamic : DollManager
{
    protected int MaxSlot = 20;

    //protected struct DynamicNode
    //{
    //    public Doll doll;
    //    public int index;
    //}

    protected List<Doll> frontList = new List<Doll>();

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
        int FrontWidth = 4; //TODO: 放成變數
        int frontNum = frontList.Count;

        if (frontNum <= 0)
            return;

        int nLine = ((frontNum-1) / FrontWidth ) + 1;
        int lastLineCount = (frontNum-1) % FrontWidth + 1;

        float fPos = Mathf.Max(1.0f, 2.0f - (float)(nLine-1) * 0.5f);  //前方起始
        float slotDepth = 1.0f;
        fPos += slotDepth * (float)(nLine-1);

        for (int l=0; l<nLine; l++)
        {
            int num = FrontWidth;
            if (l == nLine - 1)
                num = lastLineCount;
            print("Line: " + l + " Count: " + num);

            float slotWidth = Mathf.Max(1.0f, 1.5f - ((float)(num - 1) * 0.25f));
            float width = (float)(num - 1) * slotWidth;
            float lPos = width * -0.5f;
            for (int i= l * FrontWidth; i< l * FrontWidth + num; i++)
            {
                print("Prepare ..." + i);
                frontList[i].GetSlot().localPosition = new Vector3(lPos, 0, fPos);
                lPos += slotWidth;
            }
            fPos -= slotDepth;
        }
    }

    protected void RebuildFormation()
    {
        frontList.Clear();

        for (int i = 0; i < slotNum; i++)
        {
            if (dolls[i] && dolls[i].gameObject.activeInHierarchy && dolls[i].positionType == DOLL_POSITION_TYPE.FRONT)
            {
                frontList.Add(dolls[i]);
            }
        }
        if (frontList.Count > 0)
        {
            RebuildFrontSlots();
        }
    }


    public override Transform AddOneDoll(Doll doll, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    {
        Transform result = base.AddOneDoll(doll, positionType);

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
        }
    }
}
