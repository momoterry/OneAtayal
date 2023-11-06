using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class VDirPad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image vCenter;

    protected Vector2 touchPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(vCenter.rectTransform, data.position, data.enterEventCamera, out pos);
        //print("OnDrag!! " + pos);
    }

}
