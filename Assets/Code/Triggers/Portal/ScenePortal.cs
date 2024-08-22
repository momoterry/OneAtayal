using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    // Start is called before the first frame update
    public string sceneName;
    public string entraceName;              //跳到下個場景的指定入口
    public SpriteRenderer fadeBlocker;
    public float fadeTime = 0.5f;
    public float showupTime = -1.0f;
    public SpriteRenderer[] showupSprites;  // Portal 出現時淡入效果的目標 SpriteRenders 

    public bool messageHint = false;
    public string hintLevelName = "";
    public TextMesh hintTextMesh;
    public bool isClearLevel = false;
    public bool showWinUI = false;

    public string backScene = "";
    public string backEntrance = "";

    protected enum PHASE
    {
        NONE,
        SHOWUP,
        NORMAL,
        FADEOUT,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;

    protected float stateTimeLeft = 0;

    protected virtual void Awake()
    {
        if (fadeBlocker)
            fadeBlocker.gameObject.SetActive(false);
        if (showupTime > 0)
        {
            nextPhase = PHASE.SHOWUP;
            stateTimeLeft = showupTime;
            SetShowupAlpha(0);
        }
        else
            nextPhase = PHASE.NORMAL;
        if (hintTextMesh)
            hintTextMesh.text = hintLevelName;
    }

    //void Start()
    //{
    //    if (fadeBlocker)
    //        fadeBlocker.gameObject.SetActive(false);
    //}
    protected void SetShowupAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in showupSprites)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currPhase != nextPhase)
        {
            currPhase = nextPhase;
        }

        if (stateTimeLeft > 0)
        {
            stateTimeLeft -= Time.deltaTime;
        }

        switch (currPhase)
        {
            case PHASE.SHOWUP:
                if (stateTimeLeft > 0)
                {
                    SetShowupAlpha(1.0f - (stateTimeLeft / showupTime));
                }
                else
                {
                    SetShowupAlpha(1.0f);
                    nextPhase = PHASE.NORMAL;
                }
                break;
            case PHASE.FADEOUT:
                if (stateTimeLeft <= 0)
                {
                    stateTimeLeft = 0;
                    DoLoadScene();
                }
                else if (fadeBlocker)
                {
                    fadeBlocker.color = new Color(0, 0, 0, 1.0f - (stateTimeLeft / fadeTime));
                }
                break;
        }

        //if (stateTimeLeft > 0)
        //{
        //    stateTimeLeft -= Time.deltaTime;
        //    if (stateTimeLeft <= 0)
        //    {
        //        DoLoadScene();
        //        stateTimeLeft = 0;
        //    }
            
        //    if (fadeBlocker)
        //    {
        //        fadeBlocker.color = new Color(0, 0, 0, 1.0f - (stateTimeLeft / fadeTime));
        //    }
        //}
    }

    virtual protected void DoTeleport()
    {
        nextPhase = PHASE.FADEOUT;
        stateTimeLeft = fadeTime;
        if (fadeBlocker)
            fadeBlocker.gameObject.SetActive(true);
        BattleSystem.GetInstance().GetPlayerController().ForceStop(true);

        if (isClearLevel)
        {
            string currLevelID = GameSystem.GetLevelManager().GetCurrLevelID();
            if (currLevelID != "")
            {
                //print("關卡完成 !! " + currLevelID);
                GameSystem.GetLevelManager().SetLevelClear(currLevelID);
            }
        }
    }

    public void OnWinMenuDone()
    {
        DoTeleport();
    }

    public virtual void OnTG(GameObject whoTG)
    {
        if (currPhase != PHASE.NORMAL)
            return;
        //stateTimeLeft = fadeTime;
        //if (fadeBlocker)
        //    fadeBlocker.gameObject.SetActive(true);
        //BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
        if (messageHint)
        {
            BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
            SystemUI.ShowYesNoMessageBox(gameObject, "傳送到 " + hintLevelName + " 嗎?");
        }
        else if (showWinUI)
        {
            BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
            BattleSystem.GetInstance().theBattleHUD.OnOpenWinMenu(OnWinMenuDone);
        }
        else
        {
            DoTeleport();
        }
    }

    public void OnMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            DoTeleport();
        }
        else
        {
            BattleSystem.GetInstance().GetPlayerController().ForceStop(false);
        }
    }

    virtual protected void DoLoadScene()
    {
        if (sceneName == "")
        {
            BattleSystem.GetInstance().OnBackPrevScene();
        }
        else
        {
            if (backScene != "")
            {
                BattleSystem.GetInstance().OnGotoSceneWithBack(sceneName, entraceName, backScene, backEntrance);
                //BattleSystem.GetInstance().OnGotoScene(sceneName, backEntrance);
            }
            else
            {
                BattleSystem.GetInstance().OnGotoScene(sceneName, entraceName);
            }
        }
    }
}
