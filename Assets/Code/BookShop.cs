using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookEquipGood
{
    public BookEquipSave equip;
    public int MoneyCost;
}

public class BookShop : MonoBehaviour
{
    [System.Serializable]
    public class GoodBaseInfo
    {
        public string skillID;
        public ITEM_QUALITY quality = ITEM_QUALITY.COMMON;
        public int MoneyCost;
    }
    public GoodBaseInfo[] baseInfos;

    protected List<BookEquipGood> goodList = new List<BookEquipGood>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGoods();
    }


    protected void GenerateGoods()
    {
        print("BookShop 產生商品");
        for (int i = 0; i < baseInfos.Length; i++)
        {
            BookEquipGood good = new BookEquipGood();
            good.equip = BookEquipManager.GetInsatance().GenerateEmptyOne();
            good.equip.skillID = baseInfos[i].skillID;
            good.equip.ATK_Percent = Random.Range(100, 200);
            good.equip.HP_Percent = Random.Range(100, 200);
            good.equip.quality = baseInfos[i].quality;
            good.MoneyCost = baseInfos[i].MoneyCost;
            goodList.Add(good);
        }
    }

    public List<BookEquipGood> GetAllGoods()
    {
        return goodList;
    }

    public BookEquipGood GetGood(int _index)
    {
        if (_index < 0 || _index >= baseInfos.Length)
            return null;

        return goodList[_index];
    }

    public void RemoveGood(int _index)
    {
        if (_index < 0 || _index >= baseInfos.Length)
            return;

        print("BookShop 移除商品: " + _index);
        goodList.RemoveAt(_index);
    }
}
