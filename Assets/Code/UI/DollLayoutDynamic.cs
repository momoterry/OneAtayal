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
                print("TOD MOVE !!");
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

    public enum GROUP_TYPE
    {
        FRONT, LEFT, RIGHT, BACK
    }

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
        //CreateMiddleItems(dmD.GetMiddleList());
        CreateLRItems(dmD.GetLeftList(), true);
        CreateLRItems(dmD.GetRightList(), false);

        foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
        {
            slot.transform.SetParent(topRoot);
            slot.gameObject.SetActive(false);
        }
    }

    public override void CloseMenu()
    {
        //foreach (DollLayoutItem di in listFront)
        //{
        //    Destroy(di.gameObject);
        //}
        //foreach (DollLayoutItem di in listBack)
        //{
        //    Destroy(di.gameObject);
        //}
        //foreach (DollLayoutItem di in listLeft)
        //{
        //    Destroy(di.gameObject);
        //}
        //foreach (DollLayoutItem di in listRight)
        //{
        //    Destroy(di.gameObject);
        //}
        //listFront.Clear();
        //listBack.Clear();
        //listLeft.Clear();
        //listRight.Clear();

        //foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
        //{
        //    Destroy(slot.gameObject);
        //}

        //slotsFront.Clear();
        //slotsLeft.Clear();
        //slotsRight.Clear();
        //slotsBack.Clear();
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


    protected override bool MoveItemToSlot(DollLayoutItem item, DollLayoutSlot slot)
    {
        print("MoveItemToSlot!! From: " + item.myGroup + ", " + item.myIndex + " To: " + slot.myGroup + ", " + slot.myIndex);
        //List<DollLayoutItem> fromList, toList;

        return false;
    }


    protected void CreateFrontItems(List<Doll> dList)
    {
        if (!dollLayoutItemRef)
            return;

        int iCount = dList.Count;
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
            CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(x - hsWidth, y), (int)GROUP_TYPE.FRONT, i);  //先建立最左邊的欄位
            //print(h + " .... " + wMax);
            for (int w = 0; w<wMax; w++)
            {
                Doll d = dList[i];
                //print(i + " .... " + d);
                if (!d)
                    continue;

                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], frontRoot, new Vector2(x, y), (int)GROUP_TYPE.FRONT, i);
                CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(x + hsWidth, y), (int)GROUP_TYPE.FRONT, i+1);
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

        int i = iCount-1;
        float y = (iHeight - 1) * -0.5f * slotHeight;
        for (int h = 0; h < iHeight; h++)
        {
            int wMax = ((h == (iHeight - 1)) ? iLast : iWidth);
            float x = (-wMax + 1) * 0.5f * slotWidth;
            for (int w = 0; w < wMax; w++)
            {
                Doll d = dList[i];
                if (!d)
                    continue;

                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], backRoot, new Vector2(x, y), (int)GROUP_TYPE.BACK, i);

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
        if (!dollLayoutItemRef || dList.Count == 0)
            return;

        int totalNum = dList.Count;

        if (totalNum <= 0)
            return;

        int MiddleDepth = dmD.MiddleDepth;
        int nCols = ((totalNum - 1) / MiddleDepth) + 1;
        int lastColCount = (totalNum - 1) % MiddleDepth + 1;

        float slotWidth = 16.0f;
        float slotHeight = 16.0f;

        int i = 0;
        float x = (nCols - 1) * 0.5f * slotWidth;
        for (int c = 0; c < nCols; c++)
        {
            int nLine = MiddleDepth;
            if (c == nCols - 1)
                nLine = lastColCount;

            float y = (nLine - 1) * 0.5f * slotHeight;
            for (int l = 0; l < nLine; l++)
            {
                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], (isLeft?leftRoot:rightRoot), 
                    new Vector2((isLeft? x:-x), y), (int)(isLeft?GROUP_TYPE.LEFT:GROUP_TYPE.RIGHT), i);
                (isLeft?listLeft:listRight).Add(di);

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
