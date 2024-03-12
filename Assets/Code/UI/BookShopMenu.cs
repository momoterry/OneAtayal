using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    public BookShopItem ItemRef;

    protected List<BookShopItem> itemList = new List<BookShopItem>();
    protected BookItemInfo[] bookInfos;
    protected BookShopItem currSelectItem = null;

    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
    }

    public void OpenMenu(BookItemInfo[] infos)
    {
        bookInfos = infos;
        CreateItems();
        MenuRoot.gameObject.SetActive(true);
        if (SelectCursor)
            SelectCursor.SetActive(false);
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
        if (rrt){
            startX = rrt.anchoredPosition.x;
            startY = rrt.anchoredPosition.y;
        }

        for (int i=0; i< bookInfos.Length; i++)
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

            BookShopItem bi = o.GetComponent<BookShopItem>();
            bi.InitValue(i, bookInfos[i], ItemClickCB);

            itemList.Add(bi);
        }
    }

    protected void ClearItems()
    {
        foreach (BookShopItem item in itemList)
        {
            Destroy(item.gameObject);
        }
        itemList.Clear();
    }


    public void ItemClickCB(int _index)
    {
        print("Clicked " + _index);
        BookShopItem bi = itemList[_index];

        currSelectItem = bi;

        RectTransform rt = bi.GetComponent<RectTransform>();
        if (SelectCursor)
        {
            SelectCursor.transform.position = bi.transform.position;
            SelectCursor.SetActive(true);
        }
    }


}
