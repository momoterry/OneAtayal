using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPortal : MonoBehaviour
{
    protected SpriteRenderer fadeBlocker;
    public Vector2Int toWorldZoneIndex;
    public Vector3 enterPosition;
    public float enterFaceAngle;
    public bool enterWithCurrX = false;
    public bool enterWithCurrZ = false;

    public bool messageHint = false;

    protected float fadeTime = 0.25f;

    protected enum PHASE
    {
        NONE,
        NORMAL,
        FADEOUT,
    }
    protected PHASE currPhase = PHASE.NONE;
    protected PHASE nextPhase = PHASE.NONE;

    //protected float stateTimeLeft = 0;

    protected void Start()
    {
        //if (fadeBlocker)
        //    fadeBlocker.gameObject.SetActive(false);
        GameObject o = new GameObject("WorldPortal_FadeBlocker");
        fadeBlocker = o.AddComponent<SpriteRenderer>();
        fadeBlocker.color = Color.black;
        fadeBlocker.sprite = SystemUI.GetWitheSprite();
        fadeBlocker.transform.rotation = Quaternion.Euler(90, 0, 0);
        fadeBlocker.transform.localScale = Vector3.one * 1000.0f;
        fadeBlocker.sortingLayerName = "UI_Block";
        fadeBlocker.gameObject.SetActive(false);
        o.transform.SetParent(transform);
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

        //if (stateTimeLeft > 0)
        //{
        //    stateTimeLeft -= Time.deltaTime;
        //}

        //switch (currPhase)
        //{
        //    case PHASE.FADEOUT:
        //        if (stateTimeLeft <= 0)
        //        {
        //            stateTimeLeft = 0;
        //            DoLoadScene();
        //        }
        //        else if (fadeBlocker)
        //        {
        //            fadeBlocker.color = new Color(0, 0, 0, 1.0f - (stateTimeLeft / fadeTime));
        //        }
        //        break;
        //}
    }

    //virtual protected void DoTeleport()
    //{
    //    nextPhase = PHASE.FADEOUT;
    //    stateTimeLeft = fadeTime;
    //    if (fadeBlocker)
    //        fadeBlocker.gameObject.SetActive(true);
    //    BattleSystem.GetInstance().GetPlayerController().ForceStop(true);

    //}
    protected void DoTeleport()
    {
        nextPhase = PHASE.FADEOUT;
        BattleSystem.GetInstance().StartFadeOut(fadeTime, DoLoadScene);
        BattleSystem.GetInstance().GetPlayerController().ForceStop(true);

    }


    public void OnTG(GameObject whoTG)
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
        Vector3 pPos = BattleSystem.GetPC().transform.position;
        Vector3 fixEnterPos = new Vector3(enterWithCurrX ? pPos.x : enterPosition.x, enterPosition.y, enterWithCurrZ ? pPos.z : enterPosition.z);
        print("Ready to GotoZone: " + enterPosition  + "/" + fixEnterPos + " -- " + enterFaceAngle);
        GameSystem.GetWorldMap().GotoZone(toWorldZoneIndex, fixEnterPos, enterFaceAngle);
    }
}
