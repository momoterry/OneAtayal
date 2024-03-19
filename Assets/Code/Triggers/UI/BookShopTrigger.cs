using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShopTrigger : MonoBehaviour
{
    public BookShopMenu theMenu;
    public BookShop theShop;
    public BookShopMenu.BookItemInfo[] items;

    void Start()
    {
        //TODO: 應該執行「真正的」商店內容處理，這邊先暴力法處理

        foreach (BookShopMenu.BookItemInfo info in items)
        {
            if (info.SkillRef)
            {
                GameObject o = Instantiate(info.SkillRef.gameObject, transform);
                SkillDollSummonEx skill = o.GetComponent<SkillDollSummonEx>();

                skill.ATK_Percent = skill.ATK_Percent * Random.Range(0.5f, 1.75f);
                skill.HP_Percent = skill.HP_Percent * Random.Range(0.5f, 1.75f);
                skill.ATK_Percent = Mathf.Round(skill.ATK_Percent / 10.0f) * 10.0f;
                skill.HP_Percent = Mathf.Round(skill.HP_Percent / 10.0f) * 10.0f;


                info.SkillRef = skill;
                o.SetActive(true);
            }
        }
    }

    public void OnTG(GameObject whoTG)
    {
        if (theMenu && !theShop)
        {
            theMenu.OpenMenu(items);
            whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
            return;
        }

        print("BookShopTrigger 採用新架構");
        //List<BookEquipGood> goods = theShop.GetAllGoods();
        //BookShopMenu.BookItemInfo[] newInfos = new BookShopMenu.BookItemInfo[goods.Count];
        //for (int i=0;  i<goods.Count; i++)
        //{
        //    newInfos[i].
        //}
        theMenu.OpenMenu(theShop);
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }
}
