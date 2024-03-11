using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���F�Ѱө�����
//Book ��� Doll Summon Skill �A�N���ΧO���覡�]��

public class BookShopMenu : MonoBehaviour
{
    [System.Serializable]
    public class BookItemInfo
    {
        public SkillDollSummonEx SkillRef;
        public int MoneyCost = 1000;
    }

    public Transform MenuRoot;

    public void OpenMenu(BookItemInfo[] items)
    {
        MenuRoot.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        MenuRoot.gameObject.SetActive(false);
    }
}
