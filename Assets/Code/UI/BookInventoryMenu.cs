using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInventoryMenu : MonoBehaviour
{

    public Transform MenuRoot;
    public BookInventoryItem ItemRef;
    public BookInventoryItem EquippedRef;
    public BookCard bookCard;

    public GameObject inventoryCursor;
    public GameObject equippedCursor;

    protected List<BookInventoryItem> itemList = new List<BookInventoryItem>();
    protected BookInventoryItem[] equippedArray = new BookInventoryItem[BookEquipManager.MAX_BOOKEQUIP];

    protected enum SELECT_PHASE
    {
        NONE,
        INVENTORY,
        EQUIP,
    }
    protected SELECT_PHASE selectPhase = SELECT_PHASE.NONE;
    protected BookEquipSave lastSelect;

    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        CreateItems();
        MenuRoot.gameObject.SetActive(true);

        selectPhase = SELECT_PHASE.NONE;
        lastSelect = null;
        inventoryCursor.SetActive(false);
        equippedCursor.SetActive(false);

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

            equippedArray[i] = bi;
        }
    }

    protected void ClearItems()
    {
        foreach(BookInventoryItem bi in itemList)
        {
            Destroy(bi.gameObject);
        }
        itemList.Clear();
        for (int i=0; i < equippedArray.Length; i++)
        {
            if (equippedArray[i] != null)
            {
                Destroy(equippedArray[i].gameObject);
            }
            equippedArray[i] = null;
        }
    }

    protected void ResetItems()
    {
        ClearItems();
        CreateItems();
    }


    public void ItemClickCB(int _index)
    {
        equippedCursor.SetActive(false);
        BookEquipSave equip = BookEquipManager.GetInsatance().GetInventoryByIndex(_index);
        if (selectPhase == SELECT_PHASE.INVENTORY && lastSelect == equip) 
        {
            //雙擊
            print("雙擊 Inventory!!");
            bookCard.gameObject.SetActive(false);
            lastSelect = null;
            inventoryCursor.SetActive(false);
            selectPhase = SELECT_PHASE.NONE;

            //穿裝操作
            int validSlot = -1;
            for (int i=0; i<BookEquipManager.MAX_BOOKEQUIP; i++)
            {
                if (BookEquipManager.GetInsatance().GetCurrEquip(i) == null)
                {
                    validSlot = i;
                    break;
                }
            }
            if (validSlot >= 0)
            {
                BookEquipManager.GetInsatance().RemoveFromInventoryByIndex(_index);
                BookEquipManager.GetInsatance().Equip(equip, validSlot);
                ResetItems();
            }
            else
            {
                print("裝備己滿，無法再裝................");
            }
        }
        else
        {
            bookCard.SetCard(equip);
            bookCard.gameObject.SetActive(true);
            lastSelect = equip;
            inventoryCursor.transform.position = itemList[_index].transform.position;
            inventoryCursor.SetActive(true);
            selectPhase = SELECT_PHASE.INVENTORY;

        }
    }

    public void EquippedItemClickCB(int _index)
    {
        inventoryCursor.SetActive(false);
        BookEquipSave equip = BookEquipManager.GetInsatance().GetCurrEquip(_index);
        if (selectPhase == SELECT_PHASE.EQUIP &&　lastSelect == equip)
        {
            //雙擊
            print("雙擊 Equip !!");
            bookCard.gameObject.SetActive(false);
            lastSelect = null;
            equippedCursor.SetActive(false);
            selectPhase = SELECT_PHASE.NONE;

            //脫裝操作
            BookEquipManager.GetInsatance().Equip(null, _index);
            BookEquipManager.GetInsatance().AddToInventory(equip);
            ResetItems();
        }
        else
        {
            bookCard.SetCard(equip);
            bookCard.gameObject.SetActive(true);
            lastSelect = equip;
            equippedCursor.transform.position = equippedArray[_index].transform.position;
            equippedCursor.SetActive(true);
            selectPhase = SELECT_PHASE.EQUIP;
        }
    }
}
