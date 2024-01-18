using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : MonoBehaviour
{
    public GameObject ItemRef;
    public Transform MenuRoot;

    protected List<GameObject> itemList = new List<GameObject>();

    public void OpenMenu()
    {
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(true);
            ClearAllItems(); //以免重復開啟造成問題
            CreateAllItems();
        }
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        ClearAllItems();
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(false);
        }
        BattleSystem.GetPC().ForceStop(false);
        //GameSystem.GetInstance().SaveData();
    }

    protected void ClearAllItems()
    {
        foreach (GameObject item in itemList)
        {
            Destroy(item);
        }
        itemList.Clear();
    }

    protected void CreateAllItems()
    {
        Dictionary<string, int> items = GameSystem.GetPlayerData().GetItemInventory();
        int i = 0;
        foreach ( KeyValuePair<string, int> p in items)
        {
            ItemInfo iInfo = ItemDef.GetInstance().GetItemInfo(p.Key);
            if (iInfo == null)
            {
                print("ERROR!! No suck Item ID: " + p.Key);
                continue;
            }
            GameObject itemObj = Instantiate(ItemRef, MenuRoot);
            itemObj.SetActive(true);
            RectTransform rt = itemObj.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(8.0f, -40.0f - (26.0f * i));
            }
            itemList.Add(itemObj);

            ItemMenuItem item = itemObj.GetComponent<ItemMenuItem>();
            item.InitValue(iInfo, p.Value);

            i++;
        }
    }
}
