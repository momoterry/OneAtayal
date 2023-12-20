using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SystemUI : MonoBehaviour
{
    public MessageBox theMessageBox;
    [SerializeField]
    protected Sprite whiteSprite;

    static private SystemUI instance;

    //MessageBox
    protected GameObject messageBoxOwner;
    protected System.Action<MessageBox.RESULT> messageCB = null;

    public SystemUI() : base()
    {
        //print("SystemUI 創建完成");
    }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 SystemUI 存在 ");
        instance = this;

        DontDestroyOnLoad(gameObject);
        if (theMessageBox)
        {
            theMessageBox.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //==== 常用資源相關
    static public Sprite GetWitheSprite() { return instance.whiteSprite; }

    //==== Message Box 相關

    static public void ShowYesNoMessageBox( GameObject owner, string msg)
    {
        instance._ShowYesNoMessageBox(owner, msg);
    }

    static public void ShowYesNoMessageBox(System.Action<MessageBox.RESULT> cb, string msg)
    {
        instance._ShowYesNoMessageBox(cb, msg);
    }

    static public void ShowMessageBox(System.Action<MessageBox.RESULT> cb, string msg)
    {
        instance._ShowMessageBox(cb, msg);
    }

    static public void OnMessageBoxFinish(MessageBox.RESULT result)
    {
        instance._OnMessageBoxFinish(result);
    }


    protected void _ShowYesNoMessageBox(GameObject owner, string msg)
    {
        if (messageBoxOwner != null)
        {
            print("ERROR!!!! ShowYesNoMessageBox without close previous one !!");
            messageBoxOwner.SendMessage("OnMessageBoxResult", MessageBox.RESULT.CLOSE);
        }
        if (messageCB != null)
        {
            print("ERROR!!!! ShowYesNoMessageBox without close previous one !!");
            messageCB(MessageBox.RESULT.CLOSE);
        }
        messageBoxOwner = owner;
        messageCB = null;
        theMessageBox.theText.text = msg;
        theMessageBox.SetType(MessageBox.TYPE.YES_NO);
        theMessageBox.gameObject.SetActive(true);
    }

    protected void _ShowYesNoMessageBox(System.Action<MessageBox.RESULT> cb, string msg)
    {
        if (messageBoxOwner != null)
        {
            print("ERROR!!!! ShowYesNoMessageBox without close previous one !!");
            messageBoxOwner.SendMessage("OnMessageBoxResult", MessageBox.RESULT.CLOSE);
        }
        if (messageCB != null)
        {
            print("ERROR!!!! ShowYesNoMessageBox without close previous one !!");
        }
        messageBoxOwner = null;
        messageCB = cb;
        theMessageBox.theText.text = msg;
        theMessageBox.SetType(MessageBox.TYPE.YES_NO);
        theMessageBox.gameObject.SetActive(true);
    }

    protected void _ShowMessageBox(System.Action<MessageBox.RESULT> cb, string msg)
    {
        if (messageBoxOwner != null)
        {
            print("ERROR!!!! ShowYesNoMessageBox without close previous one !!");
            messageBoxOwner.SendMessage("OnMessageBoxResult", MessageBox.RESULT.CLOSE);
        }
        if (messageCB != null)
        {
            print("ERROR!!!! ShowYesNoMessageBox without close previous one !!");
        }
        messageBoxOwner = null;
        messageCB = cb;
        theMessageBox.theText.text = msg;
        theMessageBox.SetType(MessageBox.TYPE.SIMPLE);
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
        if (messageCB != null)
        {
            messageCB(result);
        }
        messageBoxOwner = null;
        messageCB = null;
    }

}
