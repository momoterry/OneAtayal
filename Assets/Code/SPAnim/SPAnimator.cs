using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SPAnimationClip
{
    public Sprite[] sprites;
    public float FPS = 4.0f;
    protected bool Loop = true;

    protected float currTime = 0;
    protected int currIndex = 0;

    protected int totalIndex;
    protected float stepTime;

    protected bool isValid = false;
    protected bool isDone = false;

    public bool IsValid() { return isValid; }
    public bool IsDone() { return isDone; }
    public Sprite GetCurrSprite()
    {
        if (!isValid)
            return null;
        return sprites[currIndex];
    }

    public void Reset()
    {
        currIndex = 0;
        currTime = 0;
    }

    public void Init(bool isLoop = true)
    {
        currIndex = 0;
        currTime = 0;
        totalIndex = sprites.Length;
        stepTime = 1.0f / FPS;
        isValid = totalIndex > 0 ? true : false;
        isDone = false;
        Loop = isLoop;
    }

    public void Update()
    {
        if (!isValid || isDone)
            return;

        currTime += Time.deltaTime;
        if (currTime > stepTime)
        {
            currTime -= stepTime;
            if (currIndex >= totalIndex-1)
            {
                if (Loop)
                {
                    currIndex = 0;
                }
                else
                {
                    //currIndex = totalIndex - 1;
                    isDone = true;
                }
            }
            else
            {
                currIndex++;
            }
        }
    }
}

//[System.Serializable]
//public class SpecificAnimation
//{
//    public string name;
//    public SPAnimationClip anim;
//}


//============================================================================================
//          SPAnimator
//============================================================================================
public class SPAnimator : MonoBehaviour
{
    public SpriteRenderer target;
    public SPAnimationClip InitAnim;
    public SPAnimationClip Idle;

    //public SpecificAnimation[] specificAnimations;
    //public string initSpecific;

    protected enum PHASE
    {
        NONE,
        INIT,
        LOOP,
        //SPECIFIC,
        FINISH,
    }

    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;

    //protected SPAnimationClip currClip = null;
    //protected SPAnimationClip specificClip = null;
    //protected Dictionary<string, SPAnimationClip> specificMaps = new Dictionary<string, SPAnimationClip>();

    protected float X = 0;
    protected float Y = -1.0f;
    protected bool isRun = false;

    virtual public void SetXY(float x, float y) 
    { 
        X = x; 
        Y = y;
        if (currPhase == PHASE.NONE)
        {
            //暴力法解決角色一開始就被轉向的設定
            SetupInitSprite();
        }
    }
    virtual public void SetIsRun(bool run) { isRun = run; }

    private void Awake()
    {
        //for (int i=0; i<specificAnimations.Length; i++)
        //{
        //    specificMaps.Add(specificAnimations[i].name, specificAnimations[i].anim);
        //}

        Init();
        SetupInitSprite();
    }


    virtual protected void Init()
    {
        Idle.Init();
        InitAnim.Init(false);
        //for (int i=0; i<specificAnimations.Length; i++)
        //{
        //    specificAnimations[i].anim.Init(false);
        //}
        if (InitAnim.IsValid())
        {
            //currClip = InitAnim;
            nextPhase = PHASE.INIT;
            //if (target)
            //{
            //    target.sprite = InitAnim.sprites[0];    //初始化
            //}
        }
        else
        {
            //currClip = Idle;
            nextPhase = PHASE.LOOP;
            //SetupFirstLoopSprite();
        }

        //if (currClip.IsValid() && target)
        //{
        //    target.sprite = currClip.sprites[0];    //初始化
        //}
    }


    virtual protected void SetupInitSprite()
    {
        if (InitAnim.IsValid())
            target.sprite = InitAnim.sprites[0];
        else if (Idle.IsValid())
            target.sprite = InitAnim.sprites[0];
    }

    virtual protected void SetupFirstLoopSprite()
    {
        if (Idle.IsValid() && target)
        {
            target.sprite = Idle.sprites[0];
        }
    } 

    // Update is called once per frame
    void Update()
    {
        currPhase = nextPhase;

        //if (currPhase == PHASE.FINISH || !currClip.IsValid())
        //{
        //    return;
        //}

        //currClip.Update();
        //if (target)
        //{
        //    target.sprite = currClip.GetCurrSprite();
        //}
        //if (currClip.IsDone())
        //{
        //    switch (currPhase)
        //    {
        //        case PHASE.INIT:
        //        //case PHASE.SPECIFIC:
        //            currClip = Idle;
        //            nextPhase = PHASE.LOOP;
        //            break;
        //    }
        //}

        if (currPhase == PHASE.FINISH)
        {
            return;
        }
        switch (currPhase)
        {
            case PHASE.INIT:
                UpdateInit();
                break;
            case PHASE.LOOP:
                UpdateLoop();
                break;
        }

    }

    virtual protected void UpdateInit()
    {
        if (!InitAnim.IsValid())
        {
            nextPhase = PHASE.LOOP;
            return;
        }
        InitAnim.Update();
        if (InitAnim.IsDone())
        {
            nextPhase = PHASE.LOOP;
        }
        if (target)
        {
            target.sprite = InitAnim.GetCurrSprite();
        }
    }
    
    virtual protected void UpdateLoop()
    {
        if (!Idle.IsValid())
        {
            nextPhase = PHASE.FINISH;
            return;
        }

        Idle.Update();
        if (target)
        {
            target.sprite = Idle.GetCurrSprite();
        }
    }

    //public bool PlaySpecific(string name)
    //{
    //    SPAnimationClip specificClip = specificMaps[name];
    //    if (specificClip != null)
    //    {
    //        //specificClip.Init();
    //        currClip = specificClip;
    //        nextPhase = PHASE.SPECIFIC;
            
    //        return true;
    //    }
    //    return false;
    //}

    //public void StopSpecific()
    //{
    //    //specificClip = null;
    //    if (currPhase == PHASE.SPECIFIC)
    //    {
    //        nextPhase = PHASE.LOOP;
    //    }
    //}
}
