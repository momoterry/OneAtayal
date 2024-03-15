using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInventoryMenu : MonoBehaviour
{

    public Transform MenuRoot;
    public BookInventoryItem ItemRef;
    public BookCard bookCard;

    protected List<BookInventoryItem> itemList = new List<BookInventoryItem>();

    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
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


        BookEquipManager bm = BookEquipManager.GetInsatance();
        for (int i = 0; i < bm.GetInventorySize(); i++)
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
            bi.InitValue(i, bm.GetInventoryByIndex(i), ItemClickCB);

            itemList.Add(bi);
        }
    }

    protected void ClearItems()
    {
        itemList.Clear();
    }


    public void ItemClickCB(int _index)
    {
        print("Iventory ..... " + _index);
    }
}
