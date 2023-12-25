using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public delegate void FadeBlockerDelegate();

public class FadeBlocker : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spBlocker;

    protected FadeBlockerDelegate finishCB;

    protected float fadeTime = -1;
    protected float fadeDuration = 0;
    protected float fadeRate = 0;

    protected enum FADE_PHASE
    {
        NONE,
        FADEOUT,
        FADEIN,
        DONE,
    }
    FADE_PHASE currPhase = FADE_PHASE.NONE;
    FADE_PHASE nextPhase = FADE_PHASE.NONE;

    public void StartFadeOut(float duration, FadeBlockerDelegate cb)
    {
        if (currPhase != FADE_PHASE.NONE)
        {
            print("ERROR!!! 不能 Fade Out 在這狀態: " + currPhase);
            return;
        }
        finishCB = cb;
        fadeDuration = duration;
        fadeTime = 0;
        SetBlocekRate(0);
        nextPhase = FADE_PHASE.FADEOUT;
    }
    public void StartFadeIn(float duration, FadeBlockerDelegate cb)
    {
        if (currPhase != FADE_PHASE.NONE)
        {
            print("ERROR!!! 不能 Fade In 在這狀態: " + currPhase);
            return;
        }
        finishCB = cb;
        fadeDuration = duration;
        fadeTime = 0;
        SetBlocekRate(1.0f);
        nextPhase = FADE_PHASE.FADEIN;
    }

    protected void SetBlocekRate(float alpha)
    {
        if (!spBlocker)
            return;
        if (alpha <=0)
        {
            alpha = 0;
            spBlocker.gameObject.SetActive(false);
        }
        else 
        {
            if (alpha >= 1.0f)
                alpha = 1.0f;
            spBlocker.gameObject.SetActive(true);
        }
        spBlocker.color = new Color(0, 0, 0, alpha);
    }

    // Update is called once per frame
    void Update()
    {
        if (currPhase != nextPhase)
        {
            if (nextPhase == FADE_PHASE.DONE)
            { 
                if (finishCB!=null)
                    finishCB();
            }
            currPhase = nextPhase;
        }

        switch (currPhase)
        {
            case FADE_PHASE.FADEOUT:
                fadeTime += Time.deltaTime;
                SetBlocekRate(fadeTime / fadeDuration);
                if (fadeTime >= fadeDuration)
                {
                    nextPhase = FADE_PHASE.DONE;
                }
                break;
            case FADE_PHASE.FADEIN:
                fadeTime += Time.deltaTime;
                SetBlocekRate(1.0f - (fadeTime / fadeDuration));
                if (fadeTime >= fadeDuration)
                {
                    nextPhase = FADE_PHASE.DONE;
                }
                break;
            case FADE_PHASE.DONE:
                nextPhase = FADE_PHASE.NONE;
                break;
        }
    }
}
