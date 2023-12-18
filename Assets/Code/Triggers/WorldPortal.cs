using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPortal : MonoBehaviour
{
    public SpriteRenderer fadeBlocker;
    public Vector2Int toWorldZoneIndex;

    public bool messageHint = false;
    protected float fadeTime = 0.5f;

    protected enum PHASE
    {
        NONE,
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
        nextPhase = PHASE.NORMAL;
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
    }

    virtual protected void DoTeleport()
    {
        nextPhase = PHASE.FADEOUT;
        stateTimeLeft = fadeTime;
        if (fadeBlocker)
            fadeBlocker.gameObject.SetActive(true);
        BattleSystem.GetInstance().GetPlayerController().ForceStop(true);

    }

    void OnTG(GameObject whoTG)
    {
        if (currPhase != PHASE.NORMAL)
            return;

        if (messageHint)
        {
            BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
            SystemUI.ShowYesNoMessageBox(gameObject, "傳送到世界地圖嗎"+ toWorldZoneIndex + "?");
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

    void DoLoadScene()
    {
        GameSystem.GetWorldMap().GotoZone(toWorldZoneIndex, Vector2.zero);
    }
}
