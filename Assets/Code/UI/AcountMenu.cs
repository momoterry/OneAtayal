using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcountMenu : MonoBehaviour
{
    public GameObject nickNameMenu;
    public GameObject newAccountButton;
    public Text text_ID;

    protected void OnEnable()
    {
        Init();
    }


    protected void Init()
    {
        string online_id = GameSystem.GetInstance().GetID();
        if (text_ID)
        {
            text_ID.text = online_id;
        }
        if (newAccountButton)
        {
            newAccountButton.SetActive(online_id != "");
        }
    }

    public void OnSetNickNameMenu()
    {
        nickNameMenu.SetActive(true);
    }

    public void OnSetNickNameConfirm()
    {

    }

    public void OnSetNickNameCancel()
    {
        nickNameMenu.SetActive(false);
    }

    public void OnNewAccoount()
    {
        SystemUI.ShowYesNoMessageBox(OnNewAccoountConfirmResult, "�A�T�w�зs�b��? �즳�O���N�Q�R��....");
    }

    public void OnNewAccoountConfirmResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            GameSystem.GetInstance().DeleteData();
        }
        Init();
    }

    public void OnLoadAccountMenu()
    {
        Init();
    }


    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}
