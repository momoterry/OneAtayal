using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp_BarDynamic : HpBar_PA
{
    public GameObject theSpriteRoot;

    protected float hideTime = 1.0f;
    protected float timeToHide = 0;
    //protected bool toHide = true;

    protected enum PHASE
    {
        HIDE,
        SHOW,
        TO_HIDE,
    }
    protected PHASE currPhase = PHASE.HIDE;
    protected PHASE nextPhase = PHASE.HIDE;

    protected bool hpFull = true;
    protected bool mpFull = true;

    public override void SetValue(float hp, float hpMax)
    {
        base.SetValue(hp, hpMax);
        //if (toHide && hp < hpMax)
        //{
        //    toHide = false;
        //    timeToHide = 0;
        //    ShowSprite(true);
        //}
        //else if (!toHide && hp == hpMax)
        //{
        //    timeToHide = hideTime;
        //    toHide = true;
        //}
        hpFull = (hp == hpMax);
        if (!hpFull)
        {
            ForceShow();
        }
    }

    public override void SetMPValue(float mp, float mpMax)
    {
        base.SetMPValue(mp, mpMax);
        mpFull = (mp == mpMax);
        if (!mpFull)
        {
            //print("MP ¥¼º¡ !!");
            ForceShow();
        }    
    }

    protected void ForceShow()
    {
        if (currPhase == PHASE.HIDE)
        {
            ShowSprite(true);
        }
        nextPhase = PHASE.SHOW;
    }

    protected void ShowSprite(bool isShow)
    {
        if (theSpriteRoot)
        {
            theSpriteRoot.SetActive(isShow);
        }
    }

    private void Awake()
    {
        ShowSprite(false);
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    ShowSprite(false);
    //}

    // Update is called once per frame
    void Update()
    {
        if (nextPhase != currPhase)
        {
            switch (nextPhase)
            {
                case PHASE.TO_HIDE:
                    timeToHide = hideTime;
                    break;
                case PHASE.HIDE:
                    ShowSprite(false);
                    break;
            }
        }
        currPhase = nextPhase;
        switch (currPhase)
        {
            case PHASE.SHOW:
                if (hpFull && mpFull)
                {
                    nextPhase = PHASE.TO_HIDE;
                }
                break;
            case PHASE.TO_HIDE:
                timeToHide -= Time.deltaTime;
                if (timeToHide <= 0)
                {
                    timeToHide = 0;
                    nextPhase = PHASE.HIDE;
                }
                break;
        }
        //if (timeToHide > 0)
        //{
        //    timeToHide -= Time.deltaTime;
        //    if (timeToHide <= 0)
        //    {
        //        ShowSprite(false);
        //        timeToHide = 0;
        //    }
        //}
    }
}
