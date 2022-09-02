using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DM_Dynamic : DollManager
{
    protected int MaxSlot = 20;

    protected struct DynamicNode
    {
        public Doll doll;
        public int index;
    }

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

    }


    //public override bool HasEmpltySlot(DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    //{
    //    return false;
    //}

    //public override Transform AddOneDoll(Doll doll, DOLL_POSITION_TYPE positionType = DOLL_POSITION_TYPE.FRONT)
    //{
    //    return null;
    //}
}
