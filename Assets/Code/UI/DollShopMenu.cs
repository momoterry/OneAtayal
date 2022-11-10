using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollShopMenu : MonoBehaviour
{
    public GameObject DollShopItemRef;

    public Transform DollShopRoot;
    // Start is called before the first frame update
    void Start()
    {
        CreateShopItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void CreateShopItems()
    {

        for (int i=0; i<7; i++)
        {
            GameObject item = Instantiate(DollShopItemRef, DollShopRoot.transform);
            RectTransform rt = item.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchoredPosition = new Vector2(8.0f, -40.0f - (26.0f * i));
            }
        }
    }
}
