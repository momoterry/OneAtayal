using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcountMenu : MonoBehaviour
{
    public GameObject nickNameMenu;
    public GameObject newAccountButton;
    public Text text_ID;
    public Text text_NickName;

    public Text NickNameToSet;

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
        if (text_NickName)
        {
            text_NickName.text = GameSystem.GetInstance().GetNickName();
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
        string online_id = GameSystem.GetInstance().GetID();
        string nickName = NickNameToSet.text;
        if (online_id != GameSystem.INVALID_ID && online_id != "")
        {
            if (GameSystem.GetInstance().SetNickName(nickName))
            {
                nickNameMenu.SetActive(false);
            }
            else
            {
                print("ERROR!! Set Nick Name Fail !!" + nickName);
            }
        }
        else
        {
            print("ERROR!! Wrong ID to set nick name !!");
        }
        //nickNameMenu.SetActive(false);
        Init();
    }

    public void OnSetNickNameCancel()
    {
        nickNameMenu.SetActive(false);
    }

    public void OnNewAccoount()
    {
        SystemUI.ShowYesNoMessageBox(OnNewAccoountConfirmResult, "你確定創新帳號? 原有記錄將被刪除....");
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
