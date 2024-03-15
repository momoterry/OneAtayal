using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInventoryMenu : MonoBehaviour
{

    public Transform MenuRoot;
    public BookInventoryItem ItemRef;
    public BookCard bookCard;

    public BookInventoryItem EquippedRef;

    protected List<BookInventoryItem> itemList = new List<BookInventoryItem>();
    //protected BookEquipManager bm;

    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        //bm = BookEquipManager.GetInsatance();

        CreateItems();
        MenuRoot.gameObject.SetActive(true);
        //if (SelectCursor)
        //    SelectCursor.SetActive(false);
        bookCard.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        MenuRoot.gameObject.SetActive(false);
        ClearItems();
        BattleSystem.GetPC().ForceStop(false);
    }

    protected void CreateItems()
    {
        const int numPerRow = 4;
        const float stepWidth = 36.0f;
        const float stepHeight = 36.0f;
        float startX = -54.0f;
        float startY = 60.0f;
        RectTransform rrt = ItemRef.GetComponent<RectTransform>();
        if (rrt)
        {
            startX = rrt.anchoredPosition.x;
            startY = rrt.anchoredPosition.y;
        }

        for (int i = 0; i < BookEquipManager.GetInsatance().GetInventorySize(); i++)
        {
            int row = i / numPerRow;
            int col = i % numPerRow;
            GameObject o = Instantiate(ItemRef.gameObject, MenuRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(startX + (stepWidth * col), startY - (stepHeight * row));
            }
            o.SetActive(true);

            BookInventoryItem bi = o.GetComponent<BookInventoryItem>();
            bi.InitValue(i, BookEquipManager.GetInsatance().GetInventoryByIndex(i), ItemClickCB);

            itemList.Add(bi);
        }

        //==================== 已裝備介面
        RectTransform ert = EquippedRef.GetComponent<RectTransform>();
        if (ert)
        {
            startX = ert.anchoredPosition.x;
            startY = ert.anchoredPosition.y;
        }
        for (int i=0; i<BookEquipManager.MAX_BOOKEQUIP; i++)
        {
            BookEquipSave equip = BookEquipManager.GetInsatance().GetCurrEquip(i);
            if (equip == null)
                continue;
            GameObject o = Instantiate(ItemRef.gameObject, MenuRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(startX + (stepWidth * i), startY);
            }
            o.SetActive(true);

            BookInventoryItem bi = o.GetComponent<BookInventoryItem>();
            bi.InitValue(i, equip, EquippedItemClickCB);

        }
    }

    protected void ClearItems()
    {
        itemList.Clear();
    }


    public void ItemClickCB(int _index)
    {
        BookEquipSave equip = BookEquipManager.GetInsatance().GetInventoryByIndex(_index);
        //SkillDollSummonEx skill = BookEquipManager.GetInsatance().GetSkillByID(equip.skillID);
        if (bookCard)
        {
            bookCard.SetCard(equip);
            bookCard.gameObject.SetActive(true);
        }
    }

    public void EquippedItemClickCB(int _index)
    {
        BookEquipSave equip = BookEquipManager.GetInsatance().GetCurrEquip(_index);
        //SkillDollSummonEx skill = BookEquipManager.GetInsatance().GetSkillByID(equip.skillID);
        if (bookCard)
        {
            bookCard.SetCard(equip);
            bookCard.gameObject.SetActive(true);
        }
    }
}
