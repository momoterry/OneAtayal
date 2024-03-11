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
    public GameObject ItemRef;

    protected List<GameObject> itemList = new List<GameObject>();
    protected BookItemInfo[] bookInfos;


    public void OpenMenu(BookItemInfo[] infos)
    {
        bookInfos = infos;
        CreateItems();
        MenuRoot.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        MenuRoot.gameObject.SetActive(false);
        ClearItems();
    }

    protected void CreateItems()
    {
        const int numPerRow = 4;
        const float stepWidth = 36.0f;
        const float stepHeight = 36.0f;
        const float startX = -54.0f;
        const float startY = 92.0f;
        for (int i=0; i< bookInfos.Length; i++)
        {
            int row = i / numPerRow;
            int col = i % numPerRow;
            GameObject o = Instantiate(ItemRef, MenuRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(startX + (stepWidth * col), startY - (stepHeight * row));
            }
            o.SetActive(true);

            BookShopItem bi = o.GetComponent<BookShopItem>();
            bi.InitValue(this, bookInfos[i]);
        }
    }

    protected void ClearItems()
    {
        foreach (GameObject item in itemList)
        {
            Destroy(item);
        }
        itemList.Clear();
    }

}
