using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject OptionMenu;
    public GameObject AccountMenu;
    public GameObject ResetButton;
    public GameObject AccountOptionButton;

    public Text title;
    protected float titleTime = 0;

    protected int checkLoadGameCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        GameSystem.Ensure();

        if (OptionMenu)
        {
            OptionMenu.SetActive(false);
        }

        if (ResetButton)
        {
            ResetButton.SetActive(false);
        }
        if (AccountOptionButton)
        {
            AccountOptionButton.SetActive(false);
        }
    }

    void InitSaveLoadStatus()
    {
        print("���S���s��? " + GameSystem.GetInstance().IsHasSaveGame());
        if (GameSystem.GetInstance().isOnlineSave)
        {
            AccountOptionButton.SetActive(true);
        }
        else
            ResetButton.SetActive(GameSystem.GetInstance().IsHasSaveGame());

        if (GameSystem.GetInstance().GetID() == GameSystem.INVALID_ID)
        {
            print("�u�W�s�ɵL�ġA��ܿ��~�T��");
            SystemUI.ShowMessageBox(LoadDataFailMessageCB, "�L�k���J�u�W�s�ɡA���ˬd�������A�Ϋطs�b��");
        }
    }

    public void LoadDataFailMessageCB(MessageBox.RESULT result)
    {
        print("�������~�T���� ......");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkLoadGameCount > 0)
        {
            checkLoadGameCount--;
            if (checkLoadGameCount == 0)
            {
                InitSaveLoadStatus();
            }
        }

    }


    protected void DoGameStart()
    {
        SceneManager.LoadScene("HubV_Alpha");

    }

    public void OnGameStart()
    {
        //print("START!!");


        if (OptionMenu)
        {
            OptionMenu.SetActive(true);
        }
        else
        {
            DoGameStart();
        }
    }

    public void OnResetData()
    {
        SystemUI.ShowYesNoMessageBox(gameObject, "�A�T�w�n���]�O��? �Ҧ��s�ɱN�Q�R��....");
    }

    public void OnAccountOption()
    {
        AccountMenu.SetActive(true);
    }

    public void OnMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            GameSystem.GetInstance().DeleteData();
            if (ResetButton)
            {
                ResetButton.SetActive(false);
            }
        }
    }

    public void OnSelectVPad()
    {
        GameSystem.SetUseVPad(true);
        DoGameStart();
    }

    public void OnSelectTouchControl()
    {
        GameSystem.SetUseVPad(false);
        DoGameStart();
    }



}
