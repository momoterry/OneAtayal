using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcountMenu : MonoBehaviour
{
    public GameObject nickNameMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        SystemUI.ShowYesNoMessageBox(gameObject, "�A�T�w�зs�b��? �즳�O���N�Q�R��....");
    }

    public void OnMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            GameSystem.GetInstance().DeleteData();
        }
    }

    public void OnLoadAccountMenu()
    {

    }


    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}
