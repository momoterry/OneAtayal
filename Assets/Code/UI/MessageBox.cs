using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public enum RESULT
    {
        YES,
        NO,
        CLOSE,  //Ãö³¬ (¸õ¥X)
    }

    public Text theText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
