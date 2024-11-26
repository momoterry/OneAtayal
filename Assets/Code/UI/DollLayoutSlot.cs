using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DollLayoutSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject outLine;
    public GameObject hint;

    protected DollLayoutUIBase myMenu;
    [System.NonSerialized]
    public int myGroup;
    public int myIndex;

    public class InitData
    {
        public DollLayoutUIBase menuDL;
        public int group;
        public int index;
    }
    public void Init(InitData _data)
    {
        myMenu = _data.menuDL;
        myGroup = _data.group;
        myIndex = _data.index;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        //print("....PointerEnter !!");
        myMenu.OnSlotPointerEnter(this);
    }

    public void OnPointerExit(PointerEventData data)
    {
        //print("....Exit !!");
        myMenu.OnSlotPointerExit(this);
    }
    public void ShowOutline(bool isOn)
    {
        outLine.gameObject.SetActive(isOn);
        outLine.transform.localScale = Vector3.one * (isOn? 1.5f: 1.0f);
    }

    public void ShowHint(bool isOn)
    {
        if (hint)
        {
            hint.gameObject.SetActive(isOn);
        }
    }
}
