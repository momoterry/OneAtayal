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
    //protected List<DollLayoutItem> dragTouchedItems;
    public bool IsMenuActive() { return gameObject.activeSelf; }
    public virtual void OpenMenu()  { gameObject.SetActive(true); }
    public virtual void CloseMenu() { gameObject.SetActive(false); }
    public virtual void SetupDollManager(DollManager dm) { theDM = dm; }

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
        if (currDragItem == dragItem)
        {
            currDragItem = null;
            //dragTouchedItems.Clear();
            if (touchItem != null)
            {
                touchItem.ShowOutline(false);
                touchItem = null;
            }
            foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
            {
                slot.gameObject.SetActive(false);
            }
        }
        else
        {
            print("ERROR!! UnRegisterDragItem not registered !! " + currDragItem.name);
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

}

public class DollLayoutDynamic : DollLayoutUIBase
{
    
    public Transform frontRoot;
    public Transform leftRoot;
    public Transform rightRoot;
    public Transform backRoot;

    protected DM_Dynamic dmD;

    protected List<DollLayoutItem> listFront = new List<DollLayoutItem>();
    protected List<DollLayoutItem> listBack = new List<DollLayoutItem>();
    protected List<DollLayoutItem> listLeft = new List<DollLayoutItem>();
    protected List<DollLayoutItem> listRight = new List<DollLayoutItem>();

    protected List<DollLayoutSlot> slotsFront = new List<DollLayoutSlot>();
    protected List<DollLayoutSlot> slotsBack = new List<DollLayoutSlot>();
    protected List<DollLayoutSlot> slotsLeft = new List<DollLayoutSlot>();
    protected List<DollLayoutSlot> slotsRight = new List<DollLayoutSlot>();


    private void Awake()
    {
        //gameObject.SetActive(false);
    }

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
        CreateMiddleItems(dmD.GetMiddleList());

        foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
        {
            slot.transform.SetParent(topRoot);
            slot.gameObject.SetActive(false);
        }
    }

    public override void CloseMenu()
    {
        //print("CloseMenu");

        foreach (DollLayoutItem di in listFront)
        {
            Destroy(di.gameObject);
        }
        foreach (DollLayoutItem di in listBack)
        {
            Destroy(di.gameObject);
        }
        foreach (DollLayoutItem di in listLeft)
        {
            Destroy(di.gameObject);
        }
        foreach (DollLayoutItem di in listRight)
        {
            Destroy(di.gameObject);
        }
        listFront.Clear();
        listBack.Clear();
        listLeft.Clear();
        listRight.Clear();


        foreach (DollLayoutSlot slot in gameObject.GetComponentsInChildren<DollLayoutSlot>(true))
        {
            Destroy(slot.gameObject);
        }

        slotsFront.Clear();
        slotsLeft.Clear();
        slotsRight.Clear();
        slotsBack.Clear();

        base.CloseMenu();
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
            CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(x - hsWidth, y));  //先建立最左邊的欄位
            //print(h + " .... " + wMax);
            for (int w = 0; w<wMax; w++)
            {
                Doll d = dList[i];
                //print(i + " .... " + d);
                if (!d)
                    continue;

                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], frontRoot, new Vector2(x, y));
                CreateOneSlotAndLink(slotsFront, slotRef, frontRoot, new Vector2(x + hsWidth, y));
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

                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], backRoot, new Vector2(x, y));

                listBack.Add(di);
                i--;
                x += slotWidth;
            }
            y += slotHeight;
        }

    }

    protected void CreateMiddleItems( List<Doll> dList )
    {
        if (!dollLayoutItemRef || dList.Count == 0)
            return;

        int middleNum = dList.Count;

        int circleNum = dmD.MiddleDepth + dmD.MiddleDepth;
        int nCircle = (middleNum - 1) / circleNum + 1;
        int lastCircleCount = (middleNum - 1) % circleNum + 1;

        float slotWidth = 16.0f;
        float slotHeight = 16.0f;

        float x = (nCircle-1) * 0.5f * slotWidth;
        for (int c = 0; c < nCircle; c++)
        {
            int num = circleNum;
            if (c == nCircle - 1)
                num = lastCircleCount;
            int nLine = (num - 1) / 2 + 1;
            float y = (nLine-1) * 0.5f * slotHeight;

            for (int l = 0; l < nLine; l++)
            {
                int i = c * circleNum + l * 2;
                DollLayoutItem di = CreateOneItem(dollLayoutItemRef, dList[i], leftRoot, new Vector2(x, y));
                listLeft.Add(di);

                i++;
                if (i < middleNum)
                {
                    DollLayoutItem di2 = CreateOneItem(dollLayoutItemRef, dList[i], rightRoot, new Vector2(-x, y));
                    listRight.Add(di2);
                }

                y -= slotHeight;
            }
            x -= slotWidth;
        }
    }

    protected DollLayoutItem CreateOneItem(DollLayoutItem iRef, Doll d, Transform root, Vector2 lPos)
    {
        GameObject o = Instantiate(iRef.gameObject, root.position, root.rotation, root);
        o.SetActive(true);
        RectTransform rt = o.GetComponent<RectTransform>();
        rt.localPosition = lPos;
        DollLayoutItem di = o.GetComponent<DollLayoutItem>();
        DollLayoutItem.InitData _data;
        _data.doll = d;
        _data.menuDL = this;
        di.Init(_data);

        return di;
    }

    protected DollLayoutSlot CreateOneSlotAndLink(List<DollLayoutSlot> _list, DollLayoutSlot sRef, Transform root, Vector2 lPos)
    {
        GameObject o = Instantiate(sRef.gameObject, root.position, root.rotation, root);
        o.SetActive(true);
        RectTransform rt = o.GetComponent<RectTransform>();
        rt.localPosition = lPos;
        DollLayoutSlot ds = o.GetComponent<DollLayoutSlot>();
        _list.Add(ds);
        return ds;
    }

}
