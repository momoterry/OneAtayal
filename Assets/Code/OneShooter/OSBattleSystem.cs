using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSBattleSystem : BattleSystem
{
    protected int totalScore;

    public static new OSBattleSystem GetInstance() { return (OSBattleSystem)instance; }

    public void OnOSEKilled(GameObject OSEObj)
    {
        if (currState == BATTLE_GAME_STATE.BATTLE)
        {
            OSEnemy e = OSEObj.GetComponent<OSEnemy>();
            if (e)
            {
                totalScore += e.Score;
            }
            if (theBattleHUD)
                ((OS_Battle_HUD)theBattleHUD).SetScore(totalScore);
        }
    }

    protected override void InitBattleStatus()
    {
        base.InitBattleStatus();
        totalScore = 0;
    }

    protected override void SetUpHud()
    {
        base.SetUpHud();

        if(theBattleHUD)
            ((OS_Battle_HUD)theBattleHUD).SetScore(totalScore);
    }

    //void OnGUI()
    //{
    //    GUI.TextArea(new Rect(new Vector2(10.0f, 10.0f), new Vector2(100.0f, 40.0f)), totalScore.ToString());
    //}
}
