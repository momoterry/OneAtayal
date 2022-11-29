using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SystemUI : MonoBehaviour
{
    public MessageBox theMessageBox;

    static private SystemUI instance;

    //MessageBox
    protected GameObject messageBoxOwner;

    public SystemUI() : base()
    {
        print("SystemUI 創建完成");
    }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 SystemUI 存在 ");
        instance = this;        
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (theMessageBox)
        {
            theMessageBox.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //==== Message Box 相關

    static public void ShowMessageBox( GameObject owner, string msg)
    {
        instance._ShowMessageBox(owner, msg);
    }

    static public void OnMessageBoxFinish(MessageBox.RESULT result)
    {
        instance._OnMessageBoxFinish(result);
    }

    protected void _ShowMessageBox(GameObject owner, string msg)
    {
        if (messageBoxOwner != null)
        {
            print("ERROR!!!! ShowMessageBox without close previous one !!");
            messageBoxOwner.SendMessage("OnMessageBoxResult", MessageBox.RESULT.CLOSE);
        }
        messageBoxOwner = owner;
        theMessageBox.theText.text = msg;
        theMessageBox.gameObject.SetActive(true);
    }

    protected void _OnMessageBoxFinish(MessageBox.RESULT result)
    {
       // print("_OnMessageBoxFinish: " + result);

        theMessageBox.gameObject.SetActive(false);
        if (messageBoxOwner)
        {
            messageBoxOwner.SendMessage("OnMessageBoxResult", result);
        }
        messageBoxOwner = null;
    }

}
