using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

public class DollLayoutHex : DollLayoutUIBase
{

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
        foreach (DollLayoutSlot slot in slotList)
        {
            slot.transform.SetParent(topRoot);
            slot.gameObject.SetActive(false);
            //測試
            //slot.gameObject.SetActive(true);
            //slot.ShowOutline(true);
        }
    }

    public override void CloseMenu()
    {
        ClearAll();
        base.CloseMenu();
    }

    protected void CreateAll()
    {
        List<DM_Hex.Node> nodes = dmH.GetValidNodes();
        Transform root = transform;
        for (int i = 0; i < nodes.Count; i++)
        {
            //GameObject o = Instantiate(slotRef.gameObject, root.position, root.rotation, root);
            //o.SetActive(true);
            //RectTransform rt = o.GetComponent<RectTransform>();
            //rt.localPosition = new Vector3(nodes[i].x * 16.0f, nodes[i].y * 16.0f);
            //DollLayoutSlot ds = o.GetComponent<DollLayoutSlot>();
            //DollLayoutSlot.InitData _data = new DollLayoutSlot.InitData();
            //_data.menuDL = this;
            //_data.group = 0;
            //_data.index = nodes[i].slotIndex;
            //ds.Init(_data);

            Vector3 rPos = new Vector3(nodes[i].x * 16.0f, nodes[i].y * 16.0f);
            if (nodes[i].doll != null)
            {
                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, nodes[i].doll, root, rPos, 0, nodes[i].slotIndex);
                itemList.Add(di);
            }
            else
            {
                DollLayoutSlot ds = CreateOneSlotAndLink(slotList, slotRef, root, rPos, 0, nodes[i].slotIndex);
            }
            //slotList.Add(ds);
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
