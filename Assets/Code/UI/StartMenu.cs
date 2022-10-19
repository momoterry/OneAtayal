using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    //TODO: 這些資料應該被設定在更好的地方
    [System.Serializable]
    public struct PlayerSelectInfo{
        public GameObject objRef;
        public CardPlayerCharacter card;
    }
    public PlayerSelectInfo[] cardList;

    public Text title;
    protected float titleTime = 0;

    private int currPlayerCharacterIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameSystem.Ensure();

        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i].card.SetupByStartmenu(this, i);
            cardList[i].card.SetSelected(i == currPlayerCharacterIndex);
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

    public void OnGameStart()
    {
        print("START!!");

        //GameSystem.GetInstance().SetPlayerCharacterRef(cardList[currPlayerCharacterIndex].objRef);
        SceneManager.LoadScene("HubV_Alpha");
        
    }

    public void OnPlayerCharacterCardSelected(int cardIndex)
    {
        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i].card.SetSelected(i == cardIndex);
        }
        currPlayerCharacterIndex = cardIndex;
    }

}
