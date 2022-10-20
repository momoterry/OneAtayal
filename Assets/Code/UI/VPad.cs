using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VPad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image vCenter;
    public Image vStick;

    public GameObject actionButton;

    protected Vector2 defaultCenter;
    protected Vector2 currVector = Vector2.zero;

    protected bool defaultShowPad = true;

    public Vector2 GetCurrVector() { return currVector; }

    void Start()
    {
        defaultCenter = vCenter.rectTransform.anchoredPosition;
        if (actionButton)
            actionButton.SetActive(false);

        vCenter.enabled = defaultShowPad;
        vStick.enabled = defaultShowPad;
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

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
        if (data.button != PointerEventData.InputButton.Left)
            return;

        Vector2 pos;
        Vector2 oldPos = vCenter.rectTransform.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(vCenter.rectTransform, data.position, data.enterEventCamera, out pos);
        vCenter.rectTransform.anchoredPosition = oldPos + pos;

        vCenter.enabled = true;
        vStick.enabled = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        vCenter.rectTransform.anchoredPosition = defaultCenter;
        vStick.rectTransform.anchoredPosition = Vector2.zero;
        currVector = Vector2.zero;

        vCenter.enabled = defaultShowPad;
        vStick.enabled = defaultShowPad;
    }


    public void OnAttack()
    {
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnAttack();
        }
    }

    public void OnShoot()
    {
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnShoot();
        }
    }

    public void OnAction()
    {
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnActionKey();
        }
    }

    public void OnActionOn()
    {
        if (actionButton)
            actionButton.SetActive(true);
    }

    public void OnActionOff()
    {
        if (actionButton)
            actionButton.SetActive(false);
    }

    public void OnSkillOne()
    {
        PlayerControllerBase pc = BattleSystem.GetInstance().GetPlayerController();
        if (pc)
        {
            pc.OnSkill(0);
        }
    }
}
