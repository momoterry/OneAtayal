using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//=========================================================
//==     ���H���g�c�p�C���S�O�q��}�l����
//=========================================================

public class MazeStartMenu : MonoBehaviour
{
    public GameObject OptionMenu;
    public Text numText;
    //public TextAlignmen
    public int mazeSize = 25;
    public int minSize = 10;
    public int maxSize = 200;
    public int sizeStep = 5;

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
        MazeSizeRecorder.SetMazeSize(mazeSize);
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
