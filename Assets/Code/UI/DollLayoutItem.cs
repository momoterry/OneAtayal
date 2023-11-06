using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DollLayoutItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image dollIcon;

    public void OnPointerDown(PointerEventData data)
    {
        print("..DollLayoutItem Point Down!!");
    }

    public void OnPointerUp(PointerEventData data)
    {
        print("..DollLayoutItem Point Up!!");
        dollIcon.rectTransform.localPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dollIcon.rectTransform, data.position, data.enterEventCamera, out pos);
        //print("..Drag " + data.position);
        //print("..Drag Rect Pos " + pos);
        dollIcon.rectTransform.localPosition = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
