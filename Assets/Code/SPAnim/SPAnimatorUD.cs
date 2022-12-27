using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SPAnimatorUD : SPAnimator
{
    //public SpriteRenderer target;

    public SPAnimationClip IdleUp;
    //public SPAnimationClip Idle;

    //protected float X = 0;
    //protected float Y = -1.0f;

    //protected SPAnimationClip currClip = null;

    //¥~³¡©I¥s
    //override public void SetXY( float x, float y)
    //{
    //    X = x;
    //    Y = y;
    //    //CheckCurrClip();
    //}

    //// Start is called before the first frame update
    //void Start()
    //{
    //    Init();
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    if (currClip != null)
    //    {
    //        currClip.Update();
    //        if (target)
    //        {
    //            target.sprite = currClip.GetCurrSprite();
    //        }
    //    }
    //}

    //protected void CheckCurrClip()
    //{
    //    if (Y > 0)
    //    {
    //        currClip = IdleUp;
    //    }
    //    else
    //    {
    //        currClip = Idle;
    //    }
    //}

    protected override void UpdateLoop()
    {
        //base.UpdateLoop();
        Idle.Update();
        IdleUp.Update();
        if (target)
        {
            if (Y > 0)
            {
                target.sprite = IdleUp.GetCurrSprite();
            }
            else
            {
                target.sprite = Idle.GetCurrSprite();
            }
        }
    }

    override protected void Init()
    {
        IdleUp.Init();
        base.Init();
    }

    protected override void SetupInitSprite()
    {
        if (InitAnim.IsValid())
            target.sprite = InitAnim.sprites[0];
        else
        {
            if (Y > 0)
            {
                target.sprite = IdleUp.sprites[0];
            }
            else
            {
                target.sprite = Idle.sprites[0];
            }
        }
    }
}
