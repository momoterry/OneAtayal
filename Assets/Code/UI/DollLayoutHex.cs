using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

public class DollLayoutHex : DollLayoutUIBase
{
    public RectTransform defaultRoot;
    protected List<DollLayoutItem> itemList = new();
    protected List<DollLayoutSlot> slotList = new();

    protected DM_Hex dmH;

    public override void SetupDollManager(DollManager dm)
    {
        base.SetupDollManager(dm);

        dmH = (DM_Hex)dm;
    }

    public override void OpenMenu()
    {
        base.OpenMenu();    //必須先呼叫 Base 以確保創建 Item/Slot 時能走完 Awake

        CreateAll();
    }

    public override void CloseMenu()
    {
        ClearAll();
        base.CloseMenu();
    }


    protected override bool MoveItemToSlot(DollLayoutItem item, DollLayoutSlot slot)
    {
        bool result = dmH.ChangeDollPosition(item.myDoll, item.myIndex, slot.myIndex);
        if (result) 
        {
            //print("移動成功 !!");
            //RectTransform rItem = item.GetComponent<RectTransform>();
            //RectTransform rSlot = slot.GetComponent<RectTransform>();
            //Vector3 tv = rItem.localPosition;
            //rItem.localPosition = rSlot.localPosition;
            //rSlot.localPosition = tv;

            //int tempIndex = item.myIndex;
            //item.myIndex = slot.myIndex;
            //slot.myIndex = tempIndex;
            ClearAll();
            CreateAll();
        }
        return result;
    }


    protected void CreateAll()
    {
        List<DM_Hex.Node> nodes = dmH.GetValidNodes();
        Transform root = defaultRoot ? defaultRoot : transform;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 rPos = new Vector3(nodes[i].x * 16.0f, nodes[i].y * 16.0f);
            if (nodes[i].doll != null)
            {
                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, nodes[i].doll, root, rPos, 0, nodes[i].slotIndex);
                itemList.Add(di);
            }
            //else  //無論如何都加入 Slot
            {
                DollLayoutSlot ds = CreateOneSlot(slotRef, root, rPos, 0, nodes[i].slotIndex);
                slotList.Add(ds);
            }
        }

        foreach (DollLayoutSlot slot in slotList)
        {
            slot.transform.SetParent(topRoot);
            slot.gameObject.SetActive(false);
        }
    }

    protected void ClearAll()
    {
        foreach (DollLayoutItem di in itemList)
        {
            Destroy(di.gameObject);
        }
        foreach (DollLayoutSlot ds in slotList)
        {
            Destroy(ds.gameObject);
        }
        itemList.Clear();
        slotList.Clear();
    }

}
