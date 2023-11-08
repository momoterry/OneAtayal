using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DollLayoutSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject outLine;
    // Start is called before the first frame update

    protected DollLayoutUIBase myMenu;

    public struct InitData
    {
        public DollLayoutUIBase menuDL;
    }
    public void Init(InitData _data)
    {
        myMenu = _data.menuDL;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData data)
    {
        print("....PointerEnter !!");
        myMenu.OnSlotPointerEnter(this);
    }

    public void OnPointerExit(PointerEventData data)
    {
        print("....Exit !!");
        myMenu.OnSlotPointerExit(this);
    }
    public void ShowOutline(bool isOn)
    {
        outLine.gameObject.SetActive(isOn);
    }
}
