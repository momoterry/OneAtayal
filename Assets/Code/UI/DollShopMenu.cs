using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DollShopMenu : MonoBehaviour
{
    public GameObject DollShopItemRef;
    public Transform DollShopRoot;
    public Text msgText;

    public DollShopItemInfo[] allItemInfo;


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


    public void OpenMenu()
    {
        if (msgText)
            msgText.text = "��ܭn�ʶR�����F....";
        if (DollShopRoot)
        {
            DollShopRoot.gameObject.SetActive(true);
            ClearAllItems(); //�H�K���_�}�ҳy�����D
            CreateShopItems();
        }
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
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
