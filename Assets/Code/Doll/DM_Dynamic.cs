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
            RebuilFormation();
            needRebuild = false;
        }
    }

    protected void RebuildFrontSlots()
    {
        int frontNum = frontList.Count;

        float slotWidth = 1.5f;
        float width = (float)(frontNum - 1) * slotWidth;

        float lPos = width * -0.5f;
        foreach (Doll d in frontList)
        {
            // 有問題, 這時新 Assign 的 Doll 還沒好
            d.GetSlot().localPosition = new Vector3(lPos, 0, 2.0f);
            lPos += slotWidth;
        }
    }

    protected void RebuilFormation()
    {
        print("RebuildFormation !! " +  frontList.Count);
        frontList.Clear();
        print("RebuildFormation Clear !! " +  frontList.Count);

        for (int i = 0; i < slotNum; i++)
        {
            if (dolls[i] && dolls[i].positionType == DOLL_POSITION_TYPE.FRONT)
            {
                frontList.Add(dolls[i]);
            }
        }
        if (frontList.Count > 0)
        {
            RebuildFrontSlots();
        }
    }

    //public override bool HasEmpltySlot(DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    //{
    //    return false;
    //}

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
}
