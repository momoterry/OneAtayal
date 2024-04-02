using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInventoryMenu : MonoBehaviour
{
    protected const int defaultSellValue = 100; //�ȥN�A�Ҧ��˳ƩT�w���

    public Transform MenuRoot;
    public RectTransform scrollContentRoot;

    public BookInventoryItem ItemRef;
    public BookInventoryItem EquippedRef;
    public BookCard bookCard;

    public GameObject sellArea;
    public Text sellValueText;

    public GameObject inventoryCursor;
    public GameObject equippedCursor;

    public Transform[] equippedSlots;

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
    protected int lastSelectIndex = -1;

    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
        ItemRef.gameObject.SetActive(false);
        EquippedRef.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        CreateItems();
        MenuRoot.gameObject.SetActive(true);

        selectPhase = SELECT_PHASE.NONE;
        lastSelect = null;
        lastSelectIndex = -1;
        inventoryCursor.SetActive(false);
        equippedCursor.SetActive(false);

        bookCard.gameObject.SetActive(false);
        sellArea.SetActive(false);
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        MenuRoot.gameObject.SetActive(false);
        ClearItems();
        BattleSystem.GetPC().ForceStop(false);
        GameSystem.GetInstance().SaveData();
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

        for (int i = 0; i < BookEquipManager.GetInstance().GetInventorySize(); i++)
        {
            int row = i / numPerRow;
            int col = i % numPerRow;
            GameObject o = Instantiate(ItemRef.gameObject, ItemRef.transform.parent);
            RectTransform rt = o.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(startX + (stepWidth * col), startY - (stepHeight * row));
            }
            o.SetActive(true);

            BookInventoryItem bi = o.GetComponent<BookInventoryItem>();
            bi.InitValue(i, BookEquipManager.GetInstance().GetInventoryByIndex(i), ItemClickCB);

            itemList.Add(bi);
        }

        //ScrollRest
        int rowMax = (BookEquipManager.GetInstance().GetInventorySize()-1) / numPerRow + 1;
        scrollContentRoot.sizeDelta = new Vector2(scrollContentRoot.sizeDelta.x, stepHeight * rowMax + 8);

        //==================== �w�˳Ƥ���
        //RectTransform ert = EquippedRef.GetComponent<RectTransform>();
        //if (ert)
        //{
        //    startX = ert.anchoredPosition.x;
        //    startY = ert.anchoredPosition.y;
        //}
        for (int i=0; i<BookEquipManager.MAX_BOOKEQUIP; i++)
        {
            BookEquipSave equip = BookEquipManager.GetInstance().GetCurrEquip(i);
            if (equip == null)
                continue;
            GameObject o = Instantiate(EquippedRef.gameObject, EquippedRef.transform.parent);
            //RectTransform rt = o.GetComponent<RectTransform>();
            //if (rt)
            //{
            //    rt.anchoredPosition = new Vector2(startX + (stepWidth * i), startY);
            //}
            o.transform.position = equippedSlots[i].position;
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
        BookEquipSave equip = BookEquipManager.GetInstance().GetInventoryByIndex(_index);
        if (selectPhase == SELECT_PHASE.INVENTORY && lastSelect == equip) 
        {
            //����
            //print("���� Inventory!!");
            bookCard.gameObject.SetActive(false);
            sellArea.SetActive(false);
            lastSelect = null;
            lastSelectIndex = -1;
            inventoryCursor.SetActive(false);
            selectPhase = SELECT_PHASE.NONE;

            //��˾ާ@
            string errMsg = "";
            int validSlot = -1;
            if (!BookEquipManager.GetInstance().CheckIfEquipPossible(equip, ref errMsg, ref validSlot))
            {
                SystemUI.ShowMessageBox(null, errMsg);
                return;
            }
            //for (int i=0; i<BookEquipManager.MAX_BOOKEQUIP; i++)
            //{
            //    if (BookEquipManager.GetInstance().GetCurrEquip(i) == null)
            //    {
            //        validSlot = i;
            //        break;
            //    }
            //}
            if (validSlot >= 0)
            {
                BookEquipManager.GetInstance().RemoveFromInventoryByIndex(_index);
                BookEquipManager.GetInstance().Equip(equip, validSlot);
                ResetItems();
            }
            else
            {
                //�����ӷ|����o�F
                print("ERROR!! �����Ө���o��................");
            }
        }
        else
        {
            bookCard.SetCard(equip);
            sellValueText.text = defaultSellValue.ToString();
            sellArea.gameObject.SetActive(true);
            bookCard.gameObject.SetActive(true);
            lastSelect = equip;
            lastSelectIndex = _index;
            inventoryCursor.transform.position = itemList[_index].transform.position;
            inventoryCursor.SetActive(true);
            selectPhase = SELECT_PHASE.INVENTORY;

        }
    }

    public void EquippedItemClickCB(int _index)
    {
        inventoryCursor.SetActive(false);
        BookEquipSave equip = BookEquipManager.GetInstance().GetCurrEquip(_index);
        if (selectPhase == SELECT_PHASE.EQUIP &&�@lastSelect == equip)
        {
            //����
            //print("���� Equip !!");
            bookCard.gameObject.SetActive(false);
            sellArea.SetActive(false);
            lastSelect = null;
            lastSelectIndex = -1;
            equippedCursor.SetActive(false);
            selectPhase = SELECT_PHASE.NONE;

            //��˾ާ@
            BookEquipManager.GetInstance().Equip(null, _index);
            BookEquipManager.GetInstance().AddToInventory(equip);
            ResetItems();
        }
        else
        {
            bookCard.SetCard(equip);
            sellArea.gameObject.SetActive(false);       //����c��˳Ƥ���
            bookCard.gameObject.SetActive(true);
            lastSelect = equip;
            lastSelectIndex = _index;
            equippedCursor.transform.position = equippedArray[_index].transform.position;
            equippedCursor.SetActive(true);
            selectPhase = SELECT_PHASE.EQUIP;
        }
    }

    public void OnSellBookEquipCB()
    {
        if (selectPhase != SELECT_PHASE.INVENTORY)
        {
            print("ERROR!!!! OnSellBookEquipCB() �����Ө���o��.....");
            return; //�����Ө���o��
        }

        SystemUI.ShowYesNoMessageBox(OnSellBookEquipConfirm, "�T�w�n��X?");
    }


    public void OnSellBookEquipConfirm(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            //print("�u���T�a�n��F ..... " + lastSelect.quality);
            BookEquipSave equip = BookEquipManager.GetInstance().RemoveFromInventoryByIndex(lastSelectIndex);
            BookEquipManager.GetInstance().DestroyOne(equip);
            GameSystem.GetPlayerData().AddMoney(defaultSellValue);

            bookCard.gameObject.SetActive(false);
            sellArea.SetActive(false);
            lastSelect = null;
            lastSelectIndex = -1;
            inventoryCursor.SetActive(false);

            selectPhase = SELECT_PHASE.NONE;
            ResetItems();
        }
    }

}
