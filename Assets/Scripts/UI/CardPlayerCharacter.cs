using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardPlayerCharacter : MonoBehaviour, IPointerDownHandler
{
    public Image selectedOutline;

    private StartMenu theStartMenu;
    private int myIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected(bool isSelect)
    {
        if (selectedOutline)
            selectedOutline.enabled = isSelect;
    }

    public void SetupByStartmenu(StartMenu startMenu, int idx)
    {
        theStartMenu = startMenu;
        myIndex = idx;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (theStartMenu)
            theStartMenu.OnPlayerCharacterCardSelected( myIndex );
    }
}
