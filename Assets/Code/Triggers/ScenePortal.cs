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

    void OnTG(GameObject whoTG)
    {
        currTime = fadeTime;
        if (fadeBlocker)
            fadeBlocker.gameObject.SetActive(true);
        BattleSystem.GetInstance().GetPlayerController().ForceStop(true);
    }

    void DoLoadScene()
    {
        //SceneManager.LoadScene(sceneName);
        BattleSystem.GetInstance().OnGotoScene(sceneName);
    }
}
