using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SPAnimationClip
{
    public Sprite[] sprites;
    public float FPS = 4.0f;

    protected float currTime = 0;
    protected int currIndex = 0;

    protected int totalIndex;
    protected float stepTime;

    protected bool isValid = false;

    public bool IsValid() { return isValid; }
    public Sprite GetCurrSprite() 
    {
        if (!isValid)
            return null;
        return sprites[currIndex]; 
    }
    public void Init()
    {
        currIndex = 0;
        currTime = 0;
        totalIndex = sprites.Length;
        stepTime = 1.0f / FPS;
        isValid = totalIndex > 0 ? true : false;
    }

    public void Update()
    {
        if (!isValid)
            return;

        currTime += Time.deltaTime;
        if (currTime > stepTime)
        {
            currTime -= stepTime;
            currIndex++;
            if (currIndex >= totalIndex)
            {
                currIndex = 0;
            }
        }
    }
}

public class SPAnimatorUD : SPAnimator
{
    //public SpriteRenderer target;

    public SPAnimationClip IdleUp;
    //public SPAnimationClip Idle;

    protected float X = 0;
    protected float Y = -1.0f;

    //protected SPAnimationClip currClip = null;

    //¥~³¡©I¥s
    public void SetXY( float x, float y)
    {
        X = x;
        Y = y;
        CheckCurrClip();
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
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

    protected void CheckCurrClip()
    {
        if (Y > 0)
        {
            currClip = IdleUp;
        }
        else
        {
            currClip = Idle;
        }
    }

    override protected void Init()
    {
        base.Init();
        IdleUp.Init();
        CheckCurrClip();
    }

}
