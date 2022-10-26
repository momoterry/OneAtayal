using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject OptionMenu;
    //TODO: 這些資料應該被設定在更好的地方
    [System.Serializable]
    public struct PlayerSelectInfo{
        public GameObject objRef;
        public CardPlayerCharacter card;
    }
    public PlayerSelectInfo[] cardList;

    public Text title;
    protected float titleTime = 0;

    protected int currPlayerCharacterIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameSystem.Ensure();

        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i].card.SetupByStartmenu(this, i);
            cardList[i].card.SetSelected(i == currPlayerCharacterIndex);
        }

        if (OptionMenu)
        {
            OptionMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {


        UpdateTitle();
    }

    void UpdateTitle()
    {
        if (title == null)
            return;

        float Period = 10.0f;
        float wait = 2.0f;
        titleTime += Time.deltaTime;
        if (titleTime >= Period + wait)
        {
            titleTime = wait;
        }
        else if (titleTime > wait)
        {
            float hue = (titleTime - wait) / Period;

            title.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
        else if (titleTime < wait)
        {
            float sat = titleTime / wait;
            title.color = Color.HSVToRGB(0, sat, 1.0f);
        }
    }

    protected void DoGameStart()
    {
        SceneManager.LoadScene("HubV_Alpha");

    }

    public void OnGameStart()
    {
        print("START!!");

        //GameSystem.GetInstance().SetPlayerCharacterRef(cardList[currPlayerCharacterIndex].objRef);

        if (OptionMenu)
        {
            OptionMenu.SetActive(true);
        }
        else
        {
            DoGameStart();
        }
    }

    public void OnPlayerCharacterCardSelected(int cardIndex)
    {
        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i].card.SetSelected(i == cardIndex);
        }
        currPlayerCharacterIndex = cardIndex;
    }

    public void OnResetData()
    {
        //GameSystem.GetInstance().DeleteData();
        SystemUI.ShowMessageBox(gameObject, "你確定要重設記錄? 所有存檔將被刪除....");
    }

    public void OnMessageBoxResult(MessageBox.RESULT result)
    {
        if (result == MessageBox.RESULT.YES)
        {
            GameSystem.GetInstance().DeleteData();
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
