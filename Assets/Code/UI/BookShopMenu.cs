using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

//巫靈書商店介面
//Book 表示 Doll Summon Skill ，就算改用別的方式包裝

public class BookShopMenu : MonoBehaviour
{
    [System.Serializable]
    public class BookItemInfo
    {
        public SkillDollSummonEx SkillRef;
        public int MoneyCost = 1000;
    }

    public Transform MenuRoot;
    public GameObject SelectCursor;
    public BookInventoryItem ItemRef;
    public BookCard bookCard;
    public Text costText;

    public GameObject showResultMenu;
    public BookCard resultBookCard;

    protected BookShop theShop;

    protected List<BookInventoryItem> itemList = new List<BookInventoryItem>();
    protected BookItemInfo[] bookInfos;
    protected BookInventoryItem currSelectItem = null;
    protected int currSelectIndex = -1;

    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
        ItemRef.gameObject.SetActive(false);
    }

    public void OpenMenu(BookShop shop)
    {
        theShop = shop;

        CreateItems();
        MenuRoot.gameObject.SetActive(true);
        SelectCursor.SetActive(false);
        currSelectIndex = -1;
        bookCard.gameObject.SetActive(false);
        showResultMenu.SetActive(false);
        BattleSystem.GetPC().ForceStop(true);
    }

    //public void OpenMenu(BookItemInfo[] infos)
    //{
    //    bookInfos = infos;
    //    CreateItems();
    //    MenuRoot.gameObject.SetActive(true);
    //    if (SelectCursor)
    //        SelectCursor.SetActive(false);
    //    bookCard.gameObject.SetActive(false);
    //    BattleSystem.GetPC().ForceStop(true);
    //}

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
        if (rrt){
            startX = rrt.anchoredPosition.x;
            startY = rrt.anchoredPosition.y;
        }

        List<BookEquipGood> goods = theShop.GetAllGoods();
        for (int i=0; i< goods.Count; i++)
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
            bi.InitValue(i, goods[i].equip, ItemClickCB, goods[i].hideValue);

            itemList.Add(bi);
        }
    }

    protected void ClearItems()
    {
        foreach (BookInventoryItem item in itemList)
        {
            Destroy(item.gameObject);
        }
        itemList.Clear();
    }


    protected void ResetItems()
    {
        ClearItems();
        CreateItems();
        SelectCursor.SetActive(false);
        currSelectIndex = -1;
        bookCard.gameObject.SetActive(false);
    }

    public void ItemClickCB(int _index)
    {
        //print("Clicked " + _index);
        BookInventoryItem bi = itemList[_index];

        currSelectItem = bi;

        SelectCursor.transform.position = bi.transform.position;
        SelectCursor.SetActive(true);

        currSelectIndex = _index;

        //bookCard.SetCard(bookInfos[_index].SkillRef);
        bookCard.SetCard(theShop.GetGood(_index).equip, theShop.GetGood(_index).hideValue);
        costText.text = theShop.GetGood(_index).MoneyCost.ToString();
        bookCard.gameObject.SetActive(true);
    }


    public void OnBuyCB()
    {
        BookEquipGood good = theShop.GetGood(currSelectIndex);
        if (good.MoneyCost > GameSystem.GetPlayerData().GetMoney())
        {
            SystemUI.ShowMessageBox(null, "金錢不足!!");
            return;
        }

        BookEquipManager.GetInsatance().AddToInventory(good.equip);
        GameSystem.GetPlayerData().AddMoney(-good.MoneyCost);
        theShop.RemoveGood(currSelectIndex);

        resultBookCard.SetCard(good.equip, false);
        showResultMenu.SetActive(true);

        ResetItems();
    }


    public void OnShowResultCB()
    {
        showResultMenu.SetActive(false);
    }
}
