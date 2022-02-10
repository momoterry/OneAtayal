using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    //TODO: 這些資料應該被設定在更好的地方
    [System.Serializable]
    public struct PlayerSelectInfo{
        public GameObject objRef;
        public CardPlayerCharacter card;
    }
    public PlayerSelectInfo[] cardList;

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
        
    }

    public void OnGameStart()
    {
        print("START!!");

        GameSystem.GetInstance().SetPlayerCharacterRef(cardList[currPlayerCharacterIndex].objRef);
        SceneManager.LoadScene("DungeonAlpha");
        
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
