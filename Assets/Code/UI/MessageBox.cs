using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public enum TYPE
    {
        YES_NO,
        SIMPLE,
    }

    public enum RESULT
    {
        YES,
        NO,
        CLOSE,  //Ãö³¬ (¸õ¥X)
    }

    public Text theText;
    public GameObject buttonYes;
    public GameObject buttonNo;
    public GameObject buttonOK;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetType(TYPE t)
    {
        switch (t)
        {
            case TYPE.YES_NO:
                buttonYes.SetActive(true);
                buttonNo.SetActive(true);
                buttonOK.SetActive(false);
                break;
            case TYPE.SIMPLE:
                buttonYes.SetActive(false);
                buttonNo.SetActive(false);
                buttonOK.SetActive(true);
                break;
        }
    }

    public void OnYes()
    {
        SystemUI.OnMessageBoxFinish(RESULT.YES);
    }

    public void OnNo()
    {
        SystemUI.OnMessageBoxFinish(RESULT.NO);
    }

    public void OnClose()
    {
        SystemUI.OnMessageBoxFinish(RESULT.CLOSE);
    }
}
