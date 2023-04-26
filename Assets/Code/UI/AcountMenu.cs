using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcountMenu : MonoBehaviour
{
    public GameObject nickNameMenu;
    public GameObject retrieveAccountMenu;
    public GameObject newAccountButton;
    public Text text_ID;
    public Text text_NickName;

    public Text NickNameToSet;
    public Text nickNameToRetrieveAccount;

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

    public void OnMessageBoxEmptyCB(MessageBox.RESULT _r) { }

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
                SystemUI.ShowMessageBox(OnMessageBoxEmptyCB, "無法設定這個暱稱 !!");
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

    //================  取回帳號
    public void OnRetrieveAccountMenu()
    {
        retrieveAccountMenu.SetActive(true);
    }

    public void OnRetrieveAccountClose()
    {
        retrieveAccountMenu.SetActive(false);
    }

    public void OnRetrieveAccountConfirm()
    {
        string nickname = nickNameToRetrieveAccount.text;
        if (nickname != "")
        {
            if (GameSystem.GetInstance().RetriveAccountByNickname(nickname))
            {
                Init();
                retrieveAccountMenu.SetActive(false);
                SystemUI.ShowMessageBox(OnMessageBoxEmptyCB, "取回帳號成功 !!");
            }
            else
            {
                print("ERROR!!!! OnRetrieveAccountConfirm Fail");
                SystemUI.ShowMessageBox(OnMessageBoxEmptyCB, "取回帳號失敗 ....");
            }
        }
    }

    //================  創新帳號
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
