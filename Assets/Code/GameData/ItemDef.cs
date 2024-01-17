using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�y�z���~���e���e��

public enum ITEM_TYPE
{
    MATERIAL,
}
[System.Serializable]
public class ItemInfo
{
    public string ID;
    public ITEM_TYPE type;
    public string Name;
    public Sprite Icon;
}

public class ItemDef : MonoBehaviour
{
    public ItemInfo[] itemInfos;
    protected Dictionary<string, ItemInfo> itemMap = new Dictionary<string, ItemInfo>();

    static protected ItemDef instance;

    public static ItemDef GetInstance() { return instance; }
    public static string GetDollMaterialID(string dollID) { return "Mat_" + dollID; }
    protected void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� ItemDef �s�b ");
        instance = this;

        for (int i = 0; i < itemInfos.Length; i++)
        {
            itemMap.Add(itemInfos[i].ID, itemInfos[i]);
        }
    }

    public ItemInfo GetItemInfo(string ID) 
    {
        return itemMap[ID];
    }

}
