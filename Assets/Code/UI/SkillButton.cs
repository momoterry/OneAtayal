using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkillButton : MonoBehaviour
{
    public Text coolDownText;
    public Image icon;

    protected float coolDownLeft = 0;

    public void SetIcon(Sprite iconSprite)
    {
        if (icon)
        {
            icon.sprite = iconSprite;
        }
    }
    
    public void Bind(UnityAction act)
    {
        Button button = gameObject.GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(act);
        }
    }

    public void OnSkillRelease(float coolDown)
    {
        if (coolDown > 0)
        {
            coolDownLeft = coolDown;
            DoStartCD();
        }
    }

    public void OnSkillCoolDownFinish()
    {
        DoFinishCD();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void DoStartCD()
    {
        //print("DoStartCD.........");
        if (coolDownText)
        {
            coolDownText.gameObject.SetActive(true);
        }
        SetCDText();
    }

    void DoFinishCD()
    {
        if (coolDownText)
        {
            coolDownText.gameObject.SetActive(false);
        }
    }

    void SetCDText()
    {
        if (coolDownText)
        {
            if (coolDownLeft < 1)
                coolDownText.text = coolDownLeft.ToString("F1");
            else
                coolDownText.text = ((int)Mathf.Ceil(coolDownLeft)).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDownLeft > 0)
        {
            coolDownLeft -= Time.deltaTime;
            if (coolDownLeft <= 0)
            {
                coolDownLeft = 0;
                //DoFinishCD();     //交由 SkillBase 決定正確時機
            }

            SetCDText();
        }
    }
}
