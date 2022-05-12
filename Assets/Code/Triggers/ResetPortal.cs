using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPortal : MonoBehaviour
{
    // Start is called before the first frame update
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
                DoReset();
                currTime = 0;
                fadeBlocker.color = Color.white;
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

    void DoReset()
    {
        //print("Gate Opend !!");
        BattleSystem.GetInstance().GetPlayerController().ForceStop(false);
        BattleSystem.GetInstance().OnClearGateEnter();
    }
}
