using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        base.OpenMenu();
    }

    public override void CloseMenu()
    {
        ClearAll();
        base.CloseMenu();
    }

    protected void CreateAll()
    {

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
