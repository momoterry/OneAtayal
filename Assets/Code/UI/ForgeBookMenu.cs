using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeBookMenu : ForgeMenu
{
    public GameObject showResultMenu;
    public BookCard resultBookCard;

    public override void OpenMenu()
    {
        base.OpenMenu();
        showResultMenu.SetActive(false);
    }

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

        //TODO: 是否集中到 ForgeManager 去處理
        PlayerData pData = GameSystem.GetPlayerData();
        if (formula.requireMoney > pData.GetMoney())
        {
            SystemUI.ShowMessageBox(null, "錢不夠喔....");
            return;
        }

        for (int i = 0; i < formula.inputs.Length; i++)
        {
            if (formula.inputs[i].num > pData.GetItemNum(formula.inputs[i].matID))
            {
                SystemUI.ShowMessageBox(null, "素材不足....");
                return;
            }
        }

        BookEquipSave equip = BookEquipManager.GetInstance().GenerateMagicBook(formula.outputID);
        if (equip == null)
        {
            SystemUI.ShowMessageBox(null, "嚴重 ERROR....");
            return;
        }

        BookEquipManager.GetInstance().AddToInventory(equip);

        //減去資源
        pData.AddMoney(-formula.requireMoney);
        for (int i = 0; i < formula.inputs.Length; i++)
        {
            pData.AddItem(formula.inputs[i].matID, -formula.inputs[i].num);
        }

        GameSystem.GetInstance().SaveData();

        ResetMenu();
        resultBookCard.SetCard(equip, false);
        showResultMenu.SetActive(true);
    }


    public void OnShowResultClose()
    {
        showResultMenu.SetActive(false);
    }

}
