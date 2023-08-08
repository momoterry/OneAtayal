using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VPad : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum ALIGN_TYPE
    {
        CENTER_LEFT,
        LEFT,
        RIGHT,
    }
    public Image vCenter;
    public Image vStick;

    public GameObject actionButton;

    public ALIGN_TYPE align = ALIGN_TYPE.CENTER_LEFT;

    protected Vector2 defaultCenter;
    protected Vector2 currVector = Vector2.zero;

    protected bool defaultShowPad = true;

    protected Vector2 vCenterDefaultSize;
    protected Vector2 vStickDefaultSize;
    protected Vector2 myDefaultSize;

    public Vector2 GetCurrVector() { return currVector; }

    private void Awake()
    {
        if (vCenter)
        {
            vCenterDefaultSize = vCenter.rectTransform.sizeDelta;
        }
        if (vStick)
        {
            vStickDefaultSize = vStick.rectTransform.sizeDelta;
        }

        RectTransform rt = GetComponent<RectTransform>();
        myDefaultSize = rt.sizeDelta;
    }

    void Start()
    {
        defaultCenter = vCenter.rectTransform.anchoredPosition;
        if (actionButton)
            actionButton.SetActive(false);

        vCenter.enabled = defaultShowPad;
        vStick.enabled = defaultShowPad;
    }

    public void OnScreenResolution(int width, int height)
    {
        //print("OnScreenResolution!!!! " + width + " : " + height);
        float ratio = (float)width / (float)height;
        float adjustRatio = 0;
        float scaleRatio = 1.0f;
        float x_shift = 0;
        if (ratio > 1)
        {
            adjustRatio = (ratio - 1.0f) * 9.0f / 7.0f;   //�H 16:9 ���̤j���
            scaleRatio = Mathf.Min(2.0f, (1.0f + adjustRatio));
            //x_shift = -160.0f * ratio + ( vCenterDefaultSize.x * scaleRatio * 0.5f ) + 32; ;
            switch (align)
            {
                case ALIGN_TYPE.CENTER_LEFT:
                    x_shift = -160.0f * ratio + (vCenterDefaultSize.x * scaleRatio * 0.5f) + 32; ;
                    break;
                case ALIGN_TYPE.LEFT:
                    x_shift = 48.0f * (scaleRatio - 1.0f);
                    break;
                case ALIGN_TYPE.RIGHT:
                    x_shift = -48.0f * (scaleRatio - 1.0f);
                    break;
            }
        }
        else if (ratio < 9.0f / 16.1f)
        {
            //print("�Ӫ����ե� !!");
            float shiftRatio = (9.0f / 16.0f) - ratio;
            switch (align)
            {
                case ALIGN_TYPE.LEFT:
                    x_shift = -48.0f * shiftRatio;
                    break;
                case ALIGN_TYPE.RIGHT:
                    x_shift = 48.0f * shiftRatio;
                    break;
            }
        }

        RectTransform rt = GetComponent<RectTransform>();
        Vector3 pos = rt.anchoredPosition;
        //pos.x = -180.0f * Mathf.Min(1.0f, adjustRatio);

        rt.sizeDelta = myDefaultSize * scaleRatio;

        if (vCenter)
        {
            vCenter.rectTransform.sizeDelta = vCenterDefaultSize * scaleRatio;
        }
        if (vStick)
        {
            vStick.rectTransform.sizeDelta = vStickDefaultSize * scaleRatio;
        }

        pos.x = x_shift;
        rt.anchoredPosition = pos;

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
