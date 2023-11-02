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
    
    public GameObject frontRoot;

    protected DM_Dynamic dmD;

    protected List<DollLayoutItem> listFront = new List<DollLayoutItem>();


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public override void SetupDollManager(DollManager dm)
    {
        base.SetupDollManager(dm);
        //if (dm.)
        dmD = (DM_Dynamic)dm;
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        //List<Doll> dollFrontList, dollMiddleList, dollBackList;
        //CreateFrontItems();
        //dmD.GetAllList(ref dollFrontList, ref dollMiddleList, ref dollBackList);
        CreateFrontItems(dmD.GetFrontList());
    }

    public override void CloseMenu()
    {
        foreach (DollLayoutItem di in listFront)
        {
            Destroy(di.gameObject);
        }
        listFront.Clear();
        base.CloseMenu();
    }

    protected void CreateFrontItems(List<Doll> dList)
    {
        if (!dollLayoutItemRef)
            return;

        float slotWidth = 16.0f;
        //float slotHeight = 16.0f;

        float totalWidth = dList.Count * slotWidth;
        float x = (-totalWidth + slotWidth) * 0.5f;

        foreach (Doll d in dList)
        {
            if (!d)
                continue;
            GameObject o = Instantiate(dollLayoutItemRef.gameObject, frontRoot.transform.position, frontRoot.transform.rotation, frontRoot.transform);
            o.SetActive(true);
            RectTransform rt = o.GetComponent<RectTransform>();
            rt.localPosition = new Vector2(x, 0);
            x += slotWidth;
            DollLayoutItem di = o.GetComponent<DollLayoutItem>();
            di.dollIcon.sprite = d.icon;

            listFront.Add(di);
        }
    }
}
