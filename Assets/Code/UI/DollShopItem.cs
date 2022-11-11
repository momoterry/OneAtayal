using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class DollShopItemInfo
{
    public string ID;
    public string name;
    public string desc;
}

public class DollShopItem : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Text descText;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitInfo(DollShopItemInfo info)
    {
        if (nameText)
        {
            nameText.text = info.name;
        }
        if (descText)
        {
            descText.text = info.desc;
        }

        GameObject dObj = GameSystem.GetPlayerData().GetDollRefByID(info.ID);
        if (!dObj)
        {
            print("ERROR!! Wrong Doll ID in Shop !! : " + info.ID);
        }
        Doll d = dObj.GetComponent<Doll>();
        if (icon)
        {
            icon.sprite = d.icon;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnButtonDown()
    {
        print("¨Ó°Ú¨Ó°Ú");
    }
}
