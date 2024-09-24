using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicTalkItem : MonoBehaviour
{
    public Text textUI;
    public Image bg;

    protected float timeLeft = 0;

    public void StartTalk(string msg, GameObject talker, float timeDuration = 2.0f)
    {
        textUI.text = msg;
        float minWidth = Mathf.Ceil(textUI.preferredWidth) + 8.0f;
        float minHeight = Mathf.Ceil(textUI.preferredHeight) + 8.0f;
        bg.rectTransform.sizeDelta = new Vector2(minWidth, minHeight);
        gameObject.SetActive(true);

        timeLeft = timeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0) 
        {
            timeLeft -= Time.deltaTime;
            if ( timeLeft < 0)
            {
                timeLeft = 0;
                gameObject.SetActive(false);
            }
        }
    }
}
