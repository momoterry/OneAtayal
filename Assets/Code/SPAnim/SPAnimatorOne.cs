using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimatorOne : SPAnimatorUD
{
    public SPAnimationClip Run;
    public SPAnimationClip RunUp;

    public override void SetIsRun(bool run)
    {
        if (isRun && !run)
        {
            Run.Reset();
            RunUp.Reset();
        }
        else if (!isRun && run)
        {
            Idle.Reset();
            IdleUp.Reset();
        }
        base.SetIsRun(run);
    }

    protected override void Init()
    {
        Run.Init();
        RunUp.Init();        
        base.Init();
    }
    protected override void UpdateLoop()
    {
        SPAnimationClip currLoop;
        if (isRun)
        {
            Run.Update();
            RunUp.Update();

            if (Y > 0)
                currLoop = RunUp;
            else
                currLoop = Run;
        }
        else
        {
            Idle.Update();
            IdleUp.Update();
            if (Y > 0)
                currLoop = IdleUp;
            else
                currLoop = Idle;
        }

        target.sprite = currLoop.GetCurrSprite();
    }

    protected override void SetupFirstLoopSprite()
    {
        base.SetupFirstLoopSprite();
        //print("Y = " + Y);
    }
}
