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

    //字首字尾清單 (TODO: 應該要定義在別處，先測試)
    [System.Serializable]
    public class EnhanceInfo
    {
        public DOLL_BUFF_TYPE type;
        public int value;
        public string text;
    }
    public EnhanceInfo[] enhancePrefixs;
    public EnhanceInfo[] enhanceSuffixs;

    protected List<BookEquipGood> goodList = new List<BookEquipGood>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGoods();
    }


    public void ApplyBookEquipEnhance(ref BookEquipSave equip, ITEM_QUALITY quality)
    {
        int atkAdd = 0;
        int hpAdd = 0;
        string prefix = "";
        string suffix = "";
        EnhanceInfo preEnhance = null;
        EnhanceInfo sufEnhance = null;
        float twoEnhanceRate = 40.0f;
        float hRate = twoEnhanceRate / 2.0f;
        if (quality == ITEM_QUALITY.RARE)
        {
            float rd = Random.Range(0, 100.0f);
            if (rd < 50.0f + hRate)
            {
                //取字首
                preEnhance = enhancePrefixs[Random.Range(0, enhancePrefixs.Length)];
                switch (preEnhance.type)
                {
                    case DOLL_BUFF_TYPE.DAMAGE:
                        atkAdd += preEnhance.value;
                        break;
                    case DOLL_BUFF_TYPE.HP:
                        hpAdd += preEnhance.value;
                        break;
                }
                prefix = preEnhance.text + "的";
            }
            if (rd > 50.0f - hRate)
            {
                sufEnhance = enhanceSuffixs[Random.Range(0, enhanceSuffixs.Length)];
                switch (sufEnhance.type)
                {
                    case DOLL_BUFF_TYPE.DAMAGE:
                        atkAdd += sufEnhance.value;
                        break;
                    case DOLL_BUFF_TYPE.HP:
                        hpAdd += sufEnhance.value;
                        break;
                }
                suffix = sufEnhance.text;
            }
        }
        equip.HP_Percent = hpAdd + 100;
        equip.ATK_Percent = atkAdd + 100;
        equip.quality = quality;

        SkillDollSummonEx skill = BookEquipManager.GetInsatance().GetSkillByID(equip.skillID);
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        equip.bookName = prefix + dInfo.dollName + suffix;
    }

    protected void GenerateGoods()
    {
        print("BookShop 產生商品");
        for (int i = 0; i < baseInfos.Length; i++)
        {
            BookEquipGood good = new BookEquipGood();
            good.equip = BookEquipManager.GetInsatance().GenerateEmptyOne();
            good.equip.skillID = baseInfos[i].skillID;
            //good.equip.ATK_Percent = Random.Range(100, 200);
            //good.equip.HP_Percent = Random.Range(100, 200);
            //good.equip.quality = baseInfos[i].quality;
            ApplyBookEquipEnhance(ref good.equip, baseInfos[i].quality);
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
