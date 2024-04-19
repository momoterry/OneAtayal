using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookEquipGood
{
    public BookEquipSave equip;
    public int MoneyCost;
    public bool hideValue;
}

public class BookShop : MonoBehaviour
{
    [System.Serializable]
    public class GoodBaseInfo
    {
        public string skillID;
        public ITEM_QUALITY quality = ITEM_QUALITY.COMMON;
        public int MoneyCost;
        public bool hideValue;
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
    //void Start()
    //{
    //    //print("BookShop Start.......");
    //    //GenerateGoods();
    //}

    bool isGoodGenerated = false;
    private void Update()
    {
        if (!isGoodGenerated)
        {
            //print("BookShop GenerateGoods.......");
            GenerateGoods();
            isGoodGenerated = true;
        }
    }


    public void ApplyBookEquipEnhance(ref BookEquipSave equip, ITEM_QUALITY quality)
    {
        int atkAdd = 0;
        int hpAdd = 0;
        string prefix = "";
        string suffix = "";
        EnhanceInfo preEnhance = null;
        EnhanceInfo sufEnhance = null;
        int prefixCount = 0;
        int suffixCount = 0;
        switch (quality)
        {
            case ITEM_QUALITY.UNCOMMON:
                prefixCount = 1;
                break;
            case ITEM_QUALITY.RARE:
                prefixCount = 1;
                suffixCount = 1;
                break;
            case ITEM_QUALITY.EPIC:
                prefixCount = 2;
                suffixCount = 2;
                break;
        }
        //float twoEnhanceRate = 40.0f;
        //float hRate = twoEnhanceRate / 2.0f;
        //if (quality == ITEM_QUALITY.UNCOMMON || quality == ITEM_QUALITY.RARE)
        //{
        //    float rd = Random.Range(0, 100.0f);
        //    if (rd < 50.0f + hRate)
        //    {
        //        //取字首
        //        //preEnhance = enhancePrefixs[Random.Range(0, enhancePrefixs.Length)];
        //        //switch (preEnhance.type)
        //        //{
        //        //    case DOLL_BUFF_TYPE.DAMAGE:
        //        //        atkAdd += preEnhance.value;
        //        //        break;
        //        //    case DOLL_BUFF_TYPE.HP:
        //        //        hpAdd += preEnhance.value;
        //        //        break;
        //        //}
        //        //prefix = preEnhance.text + "的";
        //        prefixCount++;
        //    }
        //    if (rd > 50.0f - hRate)
        //    {
        //        //sufEnhance = enhanceSuffixs[Random.Range(0, enhanceSuffixs.Length)];
        //        //switch (sufEnhance.type)
        //        //{
        //        //    case DOLL_BUFF_TYPE.DAMAGE:
        //        //        atkAdd += sufEnhance.value;
        //        //        break;
        //        //    case DOLL_BUFF_TYPE.HP:
        //        //        hpAdd += sufEnhance.value;
        //        //        break;
        //        //}
        //        //suffix = sufEnhance.text;
        //        suffixCount++;
        //    }
        //}
        if (quality == ITEM_QUALITY.RARE)
        {
            prefixCount++;
            suffixCount++;
        }
        for (int i=0; i < prefixCount; i++)
        {
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
        for (int i=0; i<suffixCount; i++)
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
        //if (quality == ITEM_QUALITY.RARE)
        //{
        //    prefix = "超級";
        //    suffix = "";
        //}

        equip.HP_Percent = hpAdd + 100;
        equip.ATK_Percent = atkAdd + 100;
        equip.quality = quality;

        SkillDollSummonEx skill = BookEquipManager.GetInstance().GetSkillByID(equip.skillID);
        DollInfo dInfo = GameSystem.GetDollData().GetDollInfoByID(skill.dollID);
        equip.bookName = prefix + dInfo.dollName + suffix;
    }

    protected void GenerateGoods()
    {
        //print("BookShop 產生商品");
        for (int i = 0; i < baseInfos.Length; i++)
        {
            BookEquipGood good = new BookEquipGood();
            good.equip = BookEquipManager.GetInstance().GenerateEmptyOne();
            good.equip.skillID = baseInfos[i].skillID;
            //good.equip.ATK_Percent = Random.Range(100, 200);
            //good.equip.HP_Percent = Random.Range(100, 200);
            //good.equip.quality = baseInfos[i].quality;
            ApplyBookEquipEnhance(ref good.equip, baseInfos[i].quality);
            good.MoneyCost = baseInfos[i].MoneyCost;
            good.hideValue = baseInfos[i].hideValue;
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

        //print("BookShop 移除商品: " + _index);
        goodList.RemoveAt(_index);
    }
}
