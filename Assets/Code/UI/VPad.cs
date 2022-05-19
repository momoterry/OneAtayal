using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VPad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image vCenter;
    public Image vStick;

    protected Vector2 defaultCenter;
    protected Vector2 currVector = Vector2.zero;

    public Vector2 GetCurrVector() { return currVector; }

    void Start()
    {
        defaultCenter = vCenter.rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData data)
    {
        float maxLength = 40.0f;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(vCenter.rectTransform, data.position, data.enterEventCamera, out pos);

        if (pos.magnitude >= maxLength)
        {
            currVector = pos.normalized;
            pos = currVector * maxLength;
        }
        else
        {
            currVector = pos / maxLength;
        }

        vStick.rectTransform.anchoredPosition = pos;
    }

    public void OnPointerDown(PointerEventData data)
    {
        Vector2 pos;
        Vector2 oldPos = vCenter.rectTransform.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(vCenter.rectTransform, data.position, data.enterEventCamera, out pos);
        vCenter.rectTransform.anchoredPosition = oldPos + pos;
    }

    public void OnPointerUp(PointerEventData data)
    {
        vCenter.rectTransform.anchoredPosition = defaultCenter;
        vStick.rectTransform.anchoredPosition = Vector2.zero;
        currVector = Vector2.zero;
    }

    public void OnAttack()
    {
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnAttack();
        }
    }

    public void OnShoot()
    {
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnShoot();
        }
    }

    public void OnAction()
    {
        PlayerController pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnActionKey();
        }
    }
}
