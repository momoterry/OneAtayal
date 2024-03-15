using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInventoryMenu : MonoBehaviour
{

    public Transform MenuRoot;

    public BookCard bookCard;


    private void Awake()
    {
        MenuRoot.gameObject.SetActive(false);
    }

    public void OpenMenu()
    {
        //CreateItems();
        MenuRoot.gameObject.SetActive(true);
        //if (SelectCursor)
        //    SelectCursor.SetActive(false);
        bookCard.gameObject.SetActive(false);
        BattleSystem.GetPC().ForceStop(true);
    }

    public void CloseMenu()
    {
        MenuRoot.gameObject.SetActive(false);
        //ClearItems();
        BattleSystem.GetPC().ForceStop(false);
    }

    protected void CreateItems()
    {

    }

    protected void ClearItems()
    {

    }


    public void ItemClickCB(int _index)
    {

    }
}
