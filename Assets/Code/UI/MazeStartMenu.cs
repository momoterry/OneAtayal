using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//=========================================================
//==     為隨機迷宮小遊戲特別訂制的開始頁面
//=========================================================

public class MazeStartMenu : MonoBehaviour
{
    public GameObject OptionMenu;
    public Text numText;
    //public TextAlignmen
    protected int mazeSize = 25;
    protected int minSize = 10;
    protected int maxSize = 200;
    protected int sizeStep = 5;

    // Start is called before the first frame update
    void Start()
    {
        GameSystem.Ensure();


        SetupNumText();

        if (OptionMenu)
        {
            OptionMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void DoGameStart()
    {
        SceneManager.LoadScene("ForestMaze");

    }

    public void OnGameStart()
    {
        print("START!!");


        if (OptionMenu)
        {
            OptionMenu.SetActive(true);
        }
        else
        {
            DoGameStart();
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

    protected void SetupNumText()
    {
        if (numText != null)
        {
            numText.text = mazeSize.ToString();
        }
    }

    public void OnAddSize()
    {
        mazeSize = Mathf.Min(mazeSize+sizeStep, maxSize);
        SetupNumText();
    }

    public void OnReduceSize()
    {
        mazeSize = Mathf.Max(mazeSize - sizeStep, minSize);
        SetupNumText();
    }
}
