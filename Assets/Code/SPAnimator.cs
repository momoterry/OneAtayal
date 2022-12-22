using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SPAnimationClip
{
    public Sprite[] sprites;
    public float FPS = 4.0f;
    public bool Loop = true;

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
    public void Init()
    {
        currIndex = 0;
        currTime = 0;
        totalIndex = sprites.Length;
        stepTime = 1.0f / FPS;
        isValid = totalIndex > 0 ? true : false;
        isDone = false;
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

[System.Serializable]
public class SpecificAnimation
{
    public string name;
    public SPAnimationClip anim;
}

public class SPAnimator : MonoBehaviour
{
    public SpriteRenderer target;
    public SPAnimationClip Idle;

    public SpecificAnimation[] specificAnimations;
    public string initSpecific;

    protected SPAnimationClip currClip = null;
    protected SPAnimationClip specificClip = null;
    protected Dictionary<string, SPAnimationClip> specificMaps = new Dictionary<string, SPAnimationClip>();

    private void Awake()
    {
        for (int i=0; i<specificAnimations.Length; i++)
        {
            specificMaps.Add(specificAnimations[i].name, specificAnimations[i].anim);
        }

        Init();

        if (initSpecific != "")
        {
            PlaySpecific(initSpecific);
            if (target && specificClip != null)
            {
                target.sprite = specificClip.GetCurrSprite();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
    }

    virtual protected void Init()
    {
        Idle.Init();
        for (int i=0; i<specificAnimations.Length; i++)
        {
            specificAnimations[i].anim.Init();
        }
        currClip = Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (specificClip != null)
        {
            specificClip.Update();
            if (target) //TODO: 應該整合 currClip
            {
                target.sprite = specificClip.GetCurrSprite();
            }
            if (specificClip.IsDone())
            {
                StopSpecific();
            }
        }
        else if (currClip != null)
        {
            currClip.Update();
            if (target)
            {
                target.sprite = currClip.GetCurrSprite();
            }
        }
    }

    public bool PlaySpecific(string name)
    {
        specificClip = specificMaps[name];
        if (specificClip != null)
        {
            specificClip.Init();
            return true;
        }
        return false;
    }

    public void StopSpecific()
    {
        specificClip = null;
    }
}
