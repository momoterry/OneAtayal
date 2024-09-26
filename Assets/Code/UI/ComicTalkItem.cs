using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicTalkItem : MonoBehaviour
{
    public Text textUI;
    public Image bg;

    public float vertiBorder = 2.0f;
    public float horiBorder = 4.0f;

    protected GameObject theTaker = null;
    protected Vector3 theShift;

    protected float timeLeft = 0;

    protected RectTransform myRT;
    protected RectTransform myParentRT;
    protected Canvas myCanvas;

    protected Vector2 screenShift = new();

    private void Awake()
    {
        //myRT = GetComponent<RectTransform>();
        //myParentRT = transform.parent.GetComponent<RectTransform>();
        //myCanvas = GetComponentInParent<Canvas>();
        Init();
    }

    public void Init()
    {
        myRT = GetComponent<RectTransform>();
        myParentRT = transform.parent.GetComponent<RectTransform>();
        myCanvas = GetComponentInParent<Canvas>();
    }


    public void StartTalk(string msg, GameObject talker, Vector3 posShift, float timeDuration = 2.0f)
    {
        textUI.text = msg;
        float minWidth = Mathf.Ceil(textUI.preferredWidth);
        float minHeight = Mathf.Ceil(textUI.preferredHeight);
        bg.rectTransform.sizeDelta = new Vector2(minWidth + horiBorder + horiBorder, minHeight + vertiBorder + vertiBorder);
        gameObject.SetActive(true);

        theTaker = talker;
        theShift = posShift;

        screenShift.y = minHeight / 2 + vertiBorder;

        if (theShift.x < 0)
        {
            screenShift.x = -minWidth / 2 - horiBorder;
            bg.rectTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
        {
            screenShift.x = minWidth / 2 + horiBorder;
        }

        timeLeft = timeDuration;
        UpdatePos();
    }

    protected void EndTalk()
    {
        gameObject.SetActive(false);
        theTaker = null;
    }

    protected void UpdatePos()
    {
        Vector3 worldPos = theTaker.transform.position + theShift;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        //print("screenPos: " + screenPos);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myParentRT, screenPos, myCanvas.worldCamera, out localPos);
        localPos += screenShift;
        //print("localPos: " + localPos);
        myRT.localPosition = localPos;
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
                //gameObject.SetActive(false);
                EndTalk();
            }
            else
            {
                UpdatePos();
            }
        }
    }



}
