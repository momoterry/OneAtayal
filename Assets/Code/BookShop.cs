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
            good.MoneyCost = baseInfos[i].MoneyCost;
            goodList.Add(good);
        }
    }

    public List<BookEquipGood> GetAllGoods()
    {
        return goodList;
    }

    public void RemoveGood(int goodIndex)
    {
        if (goodIndex < 0 || goodIndex >= baseInfos.Length)
            return;

        print("BookShop 移除商品: " + goodIndex);
        goodList.RemoveAt(goodIndex);
    }
}
