using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecificAnimationClip
{
    public string name;
    public SPAnimationClip clip;
    public bool loop = true;
}

public class SPAnimatorSpecific : SPAnimator
{

    public SpecificAnimationClip[] specificClips;
    //protected string currSpecific = null;
    SpecificAnimationClip currSpecific = null;

    public override void PlaySpecific(string specificName)
    {
        //print("PlaySpecific !!!! " + specificName);
        if (currPhase != PHASE.LOOP) 
        {
            nextPhase = PHASE.LOOP;
        }
        for (int i = 0; i < specificClips.Length; i++)
        {
            if (specificClips[i].name == specificName)
            {
                //print("¶}©l Play: " + specificName);
                specificClips[i].clip.Init(specificClips[i].loop);
                currSpecific = specificClips[i];
            }
        }
    }

    protected override void UpdateLoop()
    {
        if (currSpecific != null)
        {
            currSpecific.clip.Update();
            if (target)
            {
                target.sprite = currSpecific.clip.GetCurrSprite();
            }
            if (currSpecific.clip.IsDone())
            {
                currSpecific = null;
                //print("µ²§ôspefific ");
            }
        }
        else
        {
            base.UpdateLoop();
        }
    }
}
