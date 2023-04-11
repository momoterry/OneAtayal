using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimation4D : SPAnimatorOne
{
    public SPAnimationClip IdleLeft;
    public SPAnimationClip IdleRight;
    public SPAnimationClip RunLeft;
    public SPAnimationClip RunRight;


    public override void SetIsRun(bool run)
    {
        if (isRun && !run)
        {
            RunLeft.Reset();
            RunRight.Reset();
        }
        else if (!isRun && run)
        {
            IdleLeft.Reset();
            IdleRight.Reset();
        }
        base.SetIsRun(run);
    }
    protected override void Init()
    {
        RunLeft.Init();
        RunRight.Init();
        IdleLeft.Init();
        IdleRight.Init();
        base.Init();
    }

    protected override void UpdateLoop()
    {
        SPAnimationClip currLoop;
        if (isRun)
        {
            Run.Update();
            RunUp.Update();
            RunLeft.Update();
            RunRight.Update();

            if (Mathf.Abs(X) > Mathf.Abs(Y))
            {
                if (X > 0)
                    currLoop = RunRight;
                else
                    currLoop = RunLeft;
            }
            else
            {
                if (Y > 0)
                    currLoop = RunUp;
                else
                    currLoop = Run;
            }
        }
        else
        {
            Idle.Update();
            IdleUp.Update();
            IdleLeft.Update();
            IdleRight.Update();
            if (Mathf.Abs(X) > Mathf.Abs(Y))
            {
                if (X > 0)
                    currLoop = IdleRight;
                else
                    currLoop = IdleLeft;
            }
            else
            {
                if (Y > 0)
                    currLoop = IdleUp;
                else
                    currLoop = Idle;
            }
        }

        target.sprite = currLoop.GetCurrSprite();
    }

}
