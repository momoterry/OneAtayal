using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogUI : MonoBehaviour
{
    public Text totalText;
    public RectTransform textRoot;

    public void Open()
    {
        totalText.text = One.GetLogs();
        textRoot.sizeDelta = new Vector2(textRoot.sizeDelta.x, totalText.preferredHeight);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        print("Close....");
        gameObject.SetActive(false);
    }

    //// Start is called before the first frame update
    //void Start()
    //{
    //    string allMsg = "";
    //    for (int i = 1; i <= 50; i++)
    //    {
    //        allMsg += ("²Ä " + i + "¦æ¤å¦r³á ....\n");
    //    }
    //    totalText.text = allMsg;

    //    textRoot.sizeDelta = new Vector2(textRoot.sizeDelta.x, totalText.preferredHeight);
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
