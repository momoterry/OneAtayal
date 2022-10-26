using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp_BarDynamic : HpBar_PA
{
    public GameObject theSpriteRoot;

    protected float hideTime = 1.0f;
    protected float timeToHide = 0;
    protected bool toHide = true;

    public override void SetValue(float hp, float hpMax)
    {
        base.SetValue(hp, hpMax);
        if (toHide && hp < hpMax)
        {
            toHide = false;
            timeToHide = 0;
            ShowSprite(true);
        }
        else if (!toHide && hp == hpMax)
        {
            timeToHide = hideTime;
            toHide = true;
        }
    }

    protected void ShowSprite(bool isShow)
    {
        if (theSpriteRoot)
        {
            theSpriteRoot.SetActive(isShow);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowSprite(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToHide > 0)
        {
            timeToHide -= Time.deltaTime;
            if (timeToHide <= 0)
            {
                ShowSprite(false);
                timeToHide = 0;
            }
        }
    }
}
