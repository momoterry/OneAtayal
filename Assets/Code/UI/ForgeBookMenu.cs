using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeBookMenu : ForgeMenu
{
    protected override void CreateAllItems()
    {
        float itemStep = 50.0f;
        List<ForgeFormula> list = ForgeManager.GetInstance().GetValidFormulas();
        Vector2 pos2 = itemRef.GetComponent<RectTransform>().anchoredPosition;
        Transform listRoot = itemRef.transform.parent;
        itemRef.SetActive(false);
        int i = 0;
        foreach (ForgeFormula f in list)
        {
            if (f.outputType != ITEM_TYPE.BOOKEQUIP)
                continue;
            GameObject o = Instantiate(itemRef, listRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            rt.anchoredPosition = pos2;

            ForgeMenuItem item = o.GetComponent<ForgeMenuItem>();
            item.InitValue(this, f);

            o.SetActive(true);
            pos2.y -= itemStep;
            i++;
        }

        scrollContentRoot.sizeDelta = new Vector2(scrollContentRoot.sizeDelta.x, itemStep * i + 8);
    }

    public override void OnTryToForge(ForgeMenuItem item, ForgeFormula formula)
    {
        print("試著打造: " + formula.outputID);
    }
}
