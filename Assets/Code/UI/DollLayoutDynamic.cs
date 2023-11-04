using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollLayoutUIBase : MonoBehaviour
{
    public DollLayoutItem dollLayoutItemRef;
    protected DollManager theDM;
    public bool IsMenuActive() { return gameObject.activeSelf; }
    public virtual void OpenMenu()  { gameObject.SetActive(true); }
    public virtual void CloseMenu() { gameObject.SetActive(false); }
    public virtual void SetupDollManager(DollManager dm) { theDM = dm; }
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

        CreateFrontItems(dmD.GetFrontList());
        CreateBackItems(dmD.GetBackList());

        base.OpenMenu();

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
        listFront.Clear();
        listBack.Clear();
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

        int i = 0;
        float y = (iHeight-1) * 0.5f * slotHeight;
        for (int h = 0; h<iHeight; h++)
        {
            int wMax = ((h == (iHeight - 1)) ? iLast : iWidth);
            float x = ( -wMax + 1) * 0.5f * slotWidth;
            //print(h + " .... " + wMax);
            for (int w = 0; w<wMax; w++)
            {
                Doll d = dList[i];
                //print(i + " .... " + d);
                if (!d)
                    continue;
                GameObject o = Instantiate(dollLayoutItemRef.gameObject, frontRoot.position, frontRoot.rotation, frontRoot);
                o.SetActive(true);
                RectTransform rt = o.GetComponent<RectTransform>();
                rt.localPosition = new Vector2(x, y);
                DollLayoutItem di = o.GetComponent<DollLayoutItem>();
                di.dollIcon.sprite = d.icon;

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
                GameObject o = Instantiate(dollLayoutItemRef.gameObject, backRoot.position, backRoot.rotation, backRoot);
                o.SetActive(true);
                RectTransform rt = o.GetComponent<RectTransform>();
                rt.localPosition = new Vector2(x, y);
                DollLayoutItem di = o.GetComponent<DollLayoutItem>();
                di.dollIcon.sprite = d.icon;

                listBack.Add(di);
                i--;
                x += slotWidth;
            }
            y += slotHeight;
        }

    }

    protected void CreateMiddleItems( List<Doll> dList )
    {

    }

}
