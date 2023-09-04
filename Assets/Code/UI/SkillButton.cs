using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkillButton : MonoBehaviour
{
    public Text coolDownText;
    public Image icon;
    public Text costText;

    protected float coolDownLeft = 0;
    protected Color costColor = new Color(1.000f, 0.812f, 0.000f, 1.000f);  //TODO: 看如何去抓 costText 中一開始的值

    public void SetIcon(Sprite iconSprite)
    {
        if (icon)
        {
            icon.sprite = iconSprite;
        }
    }

    public void SetCost(int cost)
    {
        if (costText)
        {
            if (cost > 0)
                costText.text = cost.ToString();
            else
                costText.text = "";
        }
    }
    
    public void SetButtonEnable( bool bEnable)
    {
        //Button button = gameObject.GetComponent<Button>();
        //if (button)
        //    button.enabled = bEnable;
        if (bEnable)
        {
            //icon.color = Color.white;
            foreach (Image ig in icon.GetComponentsInChildren<Image>())
            {
                ig.color = Color.white;
            }
            if (costText)
                costText.color = costColor;
        }
        else
        {
            //icon.color = Color.gray;
            foreach (Image ig in icon.GetComponentsInChildren<Image>())
            {
                ig.color = Color.gray;
                if (costText)
                    costText.color = costColor * Color.gray;
            }
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
