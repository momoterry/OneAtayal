using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDifficultTrigger: MonoBehaviour
{


    void OnTG(GameObject whoTG)
    {
        BattleSystem.GetInstance().OnAddLevelDifficulty();
    }
}
