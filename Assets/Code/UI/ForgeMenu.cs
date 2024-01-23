using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ثe�u���B�z Doll ����y

public class ForgeMenu : MonoBehaviour
{
    public GameObject itemRef;
    public Transform MenuRoot;

    protected List<GameObject> itemList = new List<GameObject>();

    void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(true);
            ClearAllItems(); //�H�K���_�}�ҳy�����D
            CreateAllItems();
        }
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        ClearAllItems();
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(false);
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

    protected void CreateAllItems()
    {
        float itemStep = 50.0f;
        List<ForgeFormula> list = ForgeManager.GetInstance().GetValidFormulas();
        Vector2 pos2 = itemRef.GetComponent<RectTransform>().anchoredPosition;
        itemRef.SetActive(false);
        int i = 0;
        foreach (ForgeFormula f in list)
        {
            GameObject o = Instantiate(itemRef, MenuRoot);
            RectTransform rt = o.GetComponent<RectTransform>();
            rt.anchoredPosition = pos2;

            ForgeMenuItem item = o.GetComponent<ForgeMenuItem>();
            item.InitValue(this, f);

            o.SetActive(true);
            pos2.y -= itemStep;
            i++;
        }
    }

    public void OnTryToForge( ForgeMenuItem item, ForgeFormula formula)
    {
        print("�յۥ��y: " + formula.outputID);

        FORGE_RESULT result = ForgeManager.ForgeOneDoll(formula);

        if (result == FORGE_RESULT.OK)
        {
            CloseMenu();
            return;
        }

        switch (result)
        {
            case FORGE_RESULT.NO_MONEY:
                SystemUI.ShowMessageBox(null, "��������");
                break;
            case FORGE_RESULT.NO_MATERIAL:
                SystemUI.ShowMessageBox(null, "��������");
                break;
        }

    }
}
