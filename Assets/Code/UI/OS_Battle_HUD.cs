using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OS_Battle_HUD : Battle_HUD
{
    public Text ScoreText;

    public void SetScore(int score)
    {
        if (ScoreText)
        {
            ScoreText.text = score.ToString();
        }
    }
}
