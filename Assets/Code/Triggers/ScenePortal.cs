using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    // Start is called before the first frame update
    public string sceneName;
    public SpriteRenderer fadeBlocker;
    public float fadeTime = 0.5f;

    public bool messageHint = false;
    public string hintLevelName = "";

    protected float currTime = 0;
    void Start()
    {
        if (fadeBlocker)
            fadeBlocker.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currTime > 0)
        {
            currTime -= Time.deltaTime;
            if (currTime <= 0)
            {
                DoLoadScene();
                currTime = 0;
            }
            
            if (fadeBlocker)
            {
                fadeBlocker.color = new Color(0, 0, 0, 1.0f - (currTime / fadeTime));
            }
        }
    }

    protected void DoTeleport()
    {
        currTime = fadeTime;
        if (fadeBlocker)
            fadeBlocker.gameObject.SetActive(true);
        BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
    }

    void OnTG(GameObject whoTG)
    {
        //currTime = fadeTime;
        //if (fadeBlocker)
        //    fadeBlocker.gameObject.SetActive(true);
        //BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
        if (messageHint)
        {
            BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
            SystemUI.ShowMessageBox(gameObject, "¶Ç°e¨ì " + hintLevelName + " ¶Ü?");
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
        if (sceneName == "")
        {
            BattleSystem.GetInstance().OnBackPrevScene();
        }
        else
        {
            BattleSystem.GetInstance().OnGotoScene(sceneName);
        }
    }
}
