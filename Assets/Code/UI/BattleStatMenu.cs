using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject itemRef;
    protected List<GameObject> itemList = new List<GameObject>();


    private void Awake()
    {
        print("BattleStatMenu: 我來啦 !!!!");
        ClearItems();
        CreateItems();
    }

    protected class ItemDataComparer : IComparer<ItemData>
    {
        public int Compare(ItemData A, ItemData B)
        {
            // 使用字串比較的方式進行排序
            return (int)((B.totalDamage - A.totalDamage) * 100.0f);
        }
    }
    public class ItemData
    {
        public string dollID;
        public float totalDamage;
        public bool isMax = false;
    }

    protected void CreateItems()
    {
        List<ItemData> dataList = new List<ItemData>();

        RectTransform rrt = itemRef.GetComponent<RectTransform>();
        Vector2 pos2 = rrt.anchoredPosition;
        float itemStep = rrt.sizeDelta.y;
        Transform listRoot = itemRef.transform.parent;
        itemRef.SetActive(false);

        float totalDamage = 0;
        float maxDamage = -1;
        
        Dictionary<string, float> allData = BattleStat.GetInstance().GetDollDamageTotal();
        foreach (KeyValuePair<string, float> p in allData)
        {
            ItemData data = new ItemData();
            data.dollID = p.Key;
            data.totalDamage = p.Value;
            dataList.Add(data);

            if (data.totalDamage > maxDamage)
                maxDamage = data.totalDamage;
            totalDamage += data.totalDamage;
        }

        dataList.Sort(new ItemDataComparer());
        dataList[0].isMax = true;

        for (int i=0; i<dataList.Count; i++)
        {
            GameObject o = Instantiate(itemRef, listRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            rt.anchoredPosition = pos2;

            BattleStatItem item = o.GetComponent<BattleStatItem>();
            item.InitValue(dataList[i], maxDamage, totalDamage);

            o.SetActive(true);
            pos2.y -= itemStep;
        }
    }

    protected void ClearItems()
    {
        foreach (GameObject o in itemList)
        {
            Destroy(o);
        }
        itemList.Clear();
    }
}
