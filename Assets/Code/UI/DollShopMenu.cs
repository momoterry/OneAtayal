using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DollShopItemInfo
{
    public string ID;
    public int cost;
    public string name;
    public string desc;
}

public class DollShopMenu : MonoBehaviour
{
    public GameObject DollShopItemRef;
    public Transform DollShopRoot;
    public Text msgText;

    protected DollShopItemInfo[] allItemInfo;


    protected List<GameObject> itemList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //CreateShopItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTempMessage( string msg )
    {
        msgText.text = msg;
    }


    public void OpenMenu(DollShopItemInfo[] infos)
    {
        if (msgText)
            msgText.text = "選擇要購買的巫靈....";

        allItemInfo = infos;

        if (DollShopRoot)
        {
            DollShopRoot.gameObject.SetActive(true);
            ClearAllItems(); //以免重復開啟造成問題
            CreateShopItems();
            //print("DollShopRoot 啟動 ...." + DollShopRoot.gameObject.activeInHierarchy);
        }
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        //print("DollShop CloseMenu !!");
        ClearAllItems();
        if (DollShopRoot)
        {
            DollShopRoot.gameObject.SetActive(false);
        }
        BattleSystem.GetPC().ForceStop(false);
    }

    protected void ClearAllItems()
    {
        foreach (GameObject item in itemList)
        {
            Destroy(item);
        }
        itemList.Clear();
    }

    protected void CreateShopItems()
    {

        for (int i=0; i< allItemInfo.Length; i++)
        {
            GameObject itemObj = Instantiate(DollShopItemRef, DollShopRoot);
            RectTransform rt = itemObj.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(8.0f, -40.0f - (26.0f * i));
            }
            itemList.Add(itemObj);

            DollShopItem shopItem = itemObj.GetComponent<DollShopItem>();
            shopItem.InitInfo(allItemInfo[i], this);
        }
    }
}
