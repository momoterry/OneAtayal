using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimator : MonoBehaviour
{
    public SpriteRenderer target;
    public SPAnimationClip Idle;


    protected SPAnimationClip currClip = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    virtual protected void Init()
    {
        Idle.Init();
        currClip = Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (currClip != null)
        {
            currClip.Update();
            if (target)
            {
                target.sprite = currClip.GetCurrSprite();
            }
        }
    }
}
