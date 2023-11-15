using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollLayoutUIBase : MonoBehaviour
{
    public DollLayoutItem dollLayoutItemRef;
    public DollLayoutSlot slotRef;
    public RectTransform topRoot;

    protected DollManager theDM;
    protected DollLayoutItem currDragItem;
    protected DollLayoutItem touchItem;
    protected DollLayoutSlot touchSlot;

    public bool IsMenuActive() { return gameObject.activeSelf; }
    public virtual void OpenMenu()  { gameObject.SetActive(true); }
    public virtual void CloseMenu() { gameObject.SetActive(false); }
    public virtual void SetupDollManager(DollManager dm) { theDM = dm; }

    protected virtual bool MoveItemToSlot(DollLayoutItem item, DollLayoutSlot slot) { return false; }

    public virtual void RegisterDragItem(DollLayoutItem dragItem)
    {
        if (currDragItem!= null)
        {
            print("ERROR!! RegisterDragItem but currDragItem not NULL !! " + currDragItem.name);
        }
        currDragItem = dragItem;
        foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
        {
            slot.gameObject.SetActive(true);
        }
    }

    public virtual void UnRegisterDragItem(DollLayoutItem dragItem)
    {
        DollLayoutItem moveItem = null;
        DollLayoutSlot toSlot = null;
        if (currDragItem == dragItem)
        {
            if (touchItem != null)
            {
                touchItem.ShowOutline(false);
                touchItem = null;
            }
            if (touchSlot != null)
            {
                //有碰到 Slot，準進行移動 !!
                //print("TOD MOVE !!");
                moveItem = currDragItem;
                toSlot = touchSlot;
                touchSlot.ShowOutline(false);
                touchSlot = null;
            }
            currDragItem = null;
            foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
            {
                slot.gameObject.SetActive(false);
            }
        }
        else
        {
            print("ERROR!! UnRegisterDragItem not registered !! " + currDragItem.name);
        }

        if (moveItem && toSlot)     //把移動行為放到最後以確保原 Item 跟 Slot 都回復
        {
            MoveItemToSlot(moveItem, toSlot);
        }
    }

    public void OnItemPointerEnter(DollLayoutItem _item)
    {
        if (currDragItem == null || currDragItem == _item)
            return;

        if (touchItem)
        {
            touchItem.ShowOutline(false);
        }
        touchItem = _item;
        touchItem.ShowOutline(true);
        //dragTouchedItems[0]
    }

    public void OnItemPointerExit(DollLayoutItem _item)
    {
        if (currDragItem == null || currDragItem == _item)
            return;

        if (touchItem == _item)
        {
            touchItem.ShowOutline(false);
            touchItem = null;
        }
    }

    public void OnSlotPointerEnter(DollLayoutSlot slot)
    {
        if (currDragItem == null)
            return;

        if (touchSlot)
        {
            touchSlot.ShowOutline(false);
        }
        touchSlot = slot;
        touchSlot.ShowOutline(true);
    }

    public void OnSlotPointerExit(DollLayoutSlot slot)
    {
        if (currDragItem == null)
            return;

        if (touchSlot == slot)
        {
            touchSlot.ShowOutline(false);
            touchSlot = null;
        }
    }

}

public class DollLayoutDynamic : DollLayoutUIBase
{
    
    public Transform frontRoot;
    public Transform leftRoot;
    public Transform rightRoot;
    public Transform backRoot;

    //public enum GROUP_TYPE
    //{
    //    FRONT, LEFT, RIGHT, BACK
    //}

    protected DM_Dynamic dmD;

    protected List<DollLayoutItem> listFront = new List<DollLayoutItem>();
    protected List<DollLayoutItem> listBack = new List<DollLayoutItem>();
    protected List<DollLayoutItem> listLeft = new List<DollLayoutItem>();
    protected List<DollLayoutItem> listRight = new List<DollLayoutItem>();

    protected List<DollLayoutSlot> slotsFront = new List<DollLayoutSlot>();
    protected List<DollLayoutSlot> slotsBack = new List<DollLayoutSlot>();
    protected List<DollLayoutSlot> slotsLeft = new List<DollLayoutSlot>();
    protected List<DollLayoutSlot> slotsRight = new List<DollLayoutSlot>();

    public override void SetupDollManager(DollManager dm)
    {
        base.SetupDollManager(dm);
        //if (dm.)
        dmD = (DM_Dynamic)dm;
    }

    public override void OpenMenu()
    {
        //print("OpenMenu");
        base.OpenMenu();

        CreateFrontItems(dmD.GetFrontList());
        CreateBackItems(dmD.GetBackList());
        CreateLRItems(dmD.GetLeftList(), true);
        CreateLRItems(dmD.GetRightList(), false);

        foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
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
        ClearItemAndSlotList(listFront, slotsFront);
        ClearItemAndSlotList(listLeft, slotsLeft);
        ClearItemAndSlotList(listRight, slotsRight);
        ClearItemAndSlotList(listBack, slotsBack);

        base.CloseMenu();
    }

    protected void ClearItemAndSlotList(List<DollLayoutItem> itemList, List<DollLayoutSlot> slotList)
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

    protected List<DollLayoutItem> GetItemListByGroup(int group)
    {
        switch (group)
        {
            case (int)DM_Dynamic.GROUP_TYPE.FRONT:
                return listFront;
            case (int)DM_Dynamic.GROUP_TYPE.LEFT:
                return listLeft;
            case (int)DM_Dynamic.GROUP_TYPE.RIGHT:
                return listRight;
            case (int)DM_Dynamic.GROUP_TYPE.BACK:
                return listBack;
        }
        return null;
    }
    protected List<DollLayoutSlot> GetSlotListByGroup(int group)
    {
        switch (group)
        {
            case (int)DM_Dynamic.GROUP_TYPE.FRONT:
                return slotsFront;
            case (int)DM_Dynamic.GROUP_TYPE.LEFT:
                return slotsLeft;
            case (int)DM_Dynamic.GROUP_TYPE.RIGHT:
                return slotsRight;
            case (int)DM_Dynamic.GROUP_TYPE.BACK:
                return slotsBack;
        }
        return null;
    }

    protected void RebuildGroup( int group )
    {
        ClearItemAndSlotList(GetItemListByGroup(group), GetSlotListByGroup(group));
        List<DollLayoutSlot> newSlots = null;
        switch (group)
        {
            case (int)DM_Dynamic.GROUP_TYPE.FRONT:
                CreateFrontItems(dmD.GetFrontList());
                newSlots = slotsFront;
                break;
            case (int)DM_Dynamic.GROUP_TYPE.LEFT:
                CreateLRItems(dmD.GetLeftList(), true);
                newSlots = slotsLeft;
                break;
            case (int)DM_Dynamic.GROUP_TYPE.RIGHT:
                CreateLRItems(dmD.GetRightList(), false);
                newSlots = slotsRight;
                break;
            case (int)DM_Dynamic.GROUP_TYPE.BACK:
                CreateBackItems(dmD.GetBackList());
                newSlots = slotsBack;
                break;
        }
        foreach (DollLayoutSlot s in newSlots)
        {
            s.transform.SetParent(topRoot);
            s.gameObject.SetActive(false);
        }
    }

    protected override bool MoveItemToSlot(DollLayoutItem item, DollLayoutSlot slot)
    {
        //print("MoveItemToSlot!! From: " + item.myGroup + ", " + item.myIndex + " To: " + slot.myGroup + ", " + slot.myIndex);

        bool bResult = false;
        bResult = dmD.ChangeDollPosition(item.myDoll, item.myGroup, slot.myGroup, item.myIndex, slot.myIndex);

        if (bResult)
        {
            ClearItemAndSlotList(GetItemListByGroup(item.myGroup), GetSlotListByGroup(item.myGroup));
            RebuildGroup(item.myGroup);
            if (item.myGroup != slot.myGroup)
            {
                ClearItemAndSlotList(GetItemListByGroup(slot.myGroup), GetSlotListByGroup(slot.myGroup));
                RebuildGroup(slot.myGroup);
            }

            //List<DollLayoutSlot> newSlots = new List<DollLayoutSlot>();
            //if (item.myGroup == (int)DM_Dynamic.GROUP_TYPE.FRONT || slot.myGroup == (int)DM_Dynamic.GROUP_TYPE.FRONT)
            //{
            //    CreateFrontItems(dmD.GetFrontList());
            //    newSlots.AddRange(slotsFront);
            //}
            //if (item.myGroup == (int)DM_Dynamic.GROUP_TYPE.BACK || slot.myGroup == (int)DM_Dynamic.GROUP_TYPE.BACK)
            //{
            //    CreateBackItems(dmD.GetBackList());
            //    newSlots.AddRange(slotsBack);
            //}
            //if (item.myGroup == (int)DM_Dynamic.GROUP_TYPE.LEFT || slot.myGroup == (int)DM_Dynamic.GROUP_TYPE.LEFT)
            //{
            //    CreateLRItems(dmD.GetLeftList(), true);
            //    newSlots.AddRange(slotsLeft);
            //}
            //if (item.myGroup == (int)DM_Dynamic.GROUP_TYPE.RIGHT || slot.myGroup == (int)DM_Dynamic.GROUP_TYPE.RIGHT)
            //{
            //    CreateLRItems(dmD.GetRightList(), true);
            //    newSlots.AddRange(slotsRight);
            //}

            //foreach (DollLayoutSlot s in newSlots)
            //{
            //    s.transform.SetParent(topRoot);
            //    s.gameObject.SetActive(false);
            //}

        }

        return bResult;
    }


    protected void CreateFrontItems(List<Doll> dList)
    {
        if (!dollLayoutItemRef)
            return;

        int iCount = dList.Count;
        if (iCount == 0)
        {
            //至少建立空欄位
            CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(0, 0), (int)DM_Dynamic.GROUP_TYPE.FRONT, 0);
            return;
        }
        int iWidth = dmD.FrontWidth;
        int iHeight = iCount / iWidth + 1;
        int iLast = iCount % iWidth;
        if (iLast == 0)
        {
            iLast = iWidth;
            iHeight--;
        }
        //print("Height" + iHeight + "Last" + iLast);

        float slotWidth = 16.0f;
        float slotHeight = 16.0f;
        float hsWidth = 8.0f;

        int i = 0;
        float y = (iHeight-1) * 0.5f * slotHeight;
        for (int h = 0; h<iHeight; h++)
        {
            int wMax = ((h == (iHeight - 1)) ? iLast : iWidth);
            float x = ( -wMax + 1) * 0.5f * slotWidth;
            CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(x - hsWidth, y), (int)DM_Dynamic.GROUP_TYPE.FRONT, i);  //先建立最左邊的欄位
            //print(h + " .... " + wMax);
            for (int w = 0; w<wMax; w++)
            {
                Doll d = dList[i];
                //print(i + " .... " + d);
                if (!d)
                    continue;

                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], frontRoot, new Vector2(x, y), (int)DM_Dynamic.GROUP_TYPE.FRONT, i);
                CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(x + hsWidth, y), (int)DM_Dynamic.GROUP_TYPE.FRONT, i+1);
                listFront.Add(di);
                i++;
                x += slotWidth;
            }
            y -= slotHeight;
        }

    }

    protected void CreateBackItems(List<Doll> dList)
    {
        if (!dollLayoutItemRef)
            return;

        int iCount = dList.Count;
        if (iCount == 0)
        {
            //至少建立空欄位
            CreateOneSlotAndLink(slotsBack, slotRef, backRoot, new Vector2(0, 0), (int)DM_Dynamic.GROUP_TYPE.BACK, 0);
            return;
        }
        int iWidth = dmD.BackWidth;
        int iHeight = iCount / iWidth + 1;
        int iLast = iCount % iWidth;
        if (iLast == 0)
        {
            iLast = iWidth;
            iHeight--;
        }
        //print("Height" + iHeight + "Last" + iLast);


        float slotWidth = 16.0f;
        float slotHeight = 16.0f;
        float hsWidth = 8.0f;

        int i = iCount-1;
        float y = (iHeight - 1) * -0.5f * slotHeight;
        for (int h = 0; h < iHeight; h++)
        {
            int wMax = ((h == (iHeight - 1)) ? iLast : iWidth);
            float x = (-wMax + 1) * 0.5f * slotWidth;
            CreateOneSlotAndLink(slotsBack, slotRef, backRoot, new Vector2(x - hsWidth, y), (int)DM_Dynamic.GROUP_TYPE.BACK, i+1);  //先建立最左邊的欄位
            for (int w = 0; w < wMax; w++)
            {
                Doll d = dList[i];
                if (!d)
                    continue;

                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], backRoot, new Vector2(x, y), (int)DM_Dynamic.GROUP_TYPE.BACK, i);
                CreateOneSlotAndLink(slotsBack, slotRef, backRoot, new Vector2(x + hsWidth, y), (int)DM_Dynamic.GROUP_TYPE.BACK, i);
                listBack.Add(di);
                i--;
                x += slotWidth;
            }
            y += slotHeight;
        }

    }

    //protected void CreateMiddleItems( List<Doll> dList )
    //{
    //    if (!dollLayoutItemRef || dList.Count == 0)
    //        return;

    //    int middleNum = dList.Count;

    //    int circleNum = dmD.MiddleDepth + dmD.MiddleDepth;
    //    int nCircle = (middleNum - 1) / circleNum + 1;
    //    int lastCircleCount = (middleNum - 1) % circleNum + 1;

    //    float slotWidth = 16.0f;
    //    float slotHeight = 16.0f;

    //    int _index = 0;
    //    float x = (nCircle-1) * 0.5f * slotWidth;
    //    for (int c = 0; c < nCircle; c++)
    //    {
    //        int num = circleNum;
    //        if (c == nCircle - 1)
    //            num = lastCircleCount;
    //        int nLine = (num - 1) / 2 + 1;
    //        float y = (nLine-1) * 0.5f * slotHeight;

    //        for (int l = 0; l < nLine; l++)
    //        {
    //            int i = c * circleNum + l * 2;
    //            DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], leftRoot, new Vector2(x, y), (int)GROUP_TYPE.LEFT, _index);
    //            listLeft.Add(di);

    //            i++;
    //            if (i < middleNum)
    //            {
    //                DollLayoutItem di2 = CreateOneItem(dollLayoutItemRef, dList[i], rightRoot, new Vector2(-x, y), (int)GROUP_TYPE.RIGHT, _index);
    //                listRight.Add(di2);
    //            }

    //            y -= slotHeight;
    //            _index++;
    //        }
    //        x -= slotWidth;
    //    }
    //}

    protected void CreateLRItems(List<Doll> dList, bool isLeft)
    {
        if (!dollLayoutItemRef)
            return;

        int totalNum = dList.Count;
        List<DollLayoutItem> itemList = isLeft ? listLeft : listRight;
        List<DollLayoutSlot> slotList = isLeft ? slotsLeft : slotsRight;
        Transform root = isLeft ? leftRoot : rightRoot;
        int iType = isLeft ? (int)DM_Dynamic.GROUP_TYPE.LEFT : (int)DM_Dynamic.GROUP_TYPE.RIGHT;

        if (totalNum == 0)
        {
            CreateOneSlotAndLink(slotList, slotRef, root, new Vector2(0, 0), iType, 0);  //先建立最上面的欄位
            return;
        }

        int MiddleDepth = dmD.MiddleDepth;
        int nCols = ((totalNum - 1) / MiddleDepth) + 1;
        int lastColCount = (totalNum - 1) % MiddleDepth + 1;

        float slotWidth = 16.0f;
        float slotHeight = 16.0f;
        float hsHeight = 8.0f;

        int i = 0;
        float x = (nCols - 1) * 0.5f * slotWidth;
        for (int c = 0; c < nCols; c++)
        {
            int nLine = MiddleDepth;
            if (c == nCols - 1)
                nLine = lastColCount;

            float y = (nLine - 1) * 0.5f * slotHeight;
            CreateOneSlotAndLink(slotList, slotRef, root, new Vector2((isLeft ? x : -x), y + hsHeight), iType, i);  //先建立最上面的欄位
            for (int l = 0; l < nLine; l++)
            {
                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], root, new Vector2((isLeft? x:-x), y), iType, i);
                itemList.Add(di);
                CreateOneSlotAndLink(slotList, slotRef, root, new Vector2((isLeft ? x : -x), y - hsHeight), iType, i+1);  //建立下方欄位
                y -= slotHeight;
                i++;
            }
            x -= slotWidth;
        }
    }

    protected DollLayoutItem CreateOneItem(DollLayoutItem iRef, Doll d, Transform root, Vector2 lPos, int group, int index)
    {
        GameObject o = Instantiate(iRef.gameObject, root.position, root.rotation, root);
        o.SetActive(true);
        RectTransform rt = o.GetComponent<RectTransform>();
        rt.localPosition = lPos;
        DollLayoutItem di = o.GetComponent<DollLayoutItem>();
        DollLayoutItem.InitData _data = new DollLayoutItem.InitData();
        _data.doll = d;
        _data.menuDL = this;
        _data.group = group;
        _data.index = index;
        di.Init(_data);

        return di;
    }

    protected DollLayoutSlot CreateOneSlotAndLink(List<DollLayoutSlot> _list, DollLayoutSlot sRef, Transform root, Vector2 lPos, int group, int index)
    {
        GameObject o = Instantiate(sRef.gameObject, root.position, root.rotation, root);
        o.SetActive(true);
        RectTransform rt = o.GetComponent<RectTransform>();
        rt.localPosition = lPos;
        DollLayoutSlot ds = o.GetComponent<DollLayoutSlot>();
        DollLayoutSlot.InitData _data = new DollLayoutSlot.InitData();
        _data.menuDL = this;
        _data.group = group;
        _data.index = index;
        ds.Init(_data);
        _list.Add(ds);
        return ds;
    }

}
